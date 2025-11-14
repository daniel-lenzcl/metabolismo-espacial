// Arquivo completo com pequena correção: se o molde existir mas não definir tarefas,
// caímos no fluxo padrão (rotina circular), evitando bloquear a instanciação das pessoas.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class cPessoa
{
    public UnityEvent OnCasaDefinida = new UnityEvent();

    public string identidade;
    public GameObject tipoPessoa; // prefab/tipo visual (opcional)
    public GameObject objPessoa;

    // campos originais (mantidos)
    public Predios minhaCasa;   // comentado conforme solicitado
    public Predios meuTrabalho; // comentado conforme solicitado
    public mPredios MminhaCasa;
    public mPredios MmeuTrabalho;
    public mPredios MmeuRestaurante;

    public Gerente_de_ambiente ambiente;
    public gestor_populacao gerente_populacao;
    public GameObject as_pessoas; //prefab do agente

    // novo campo: referência direta ao componente horas no GameObject 'ambiente'
    public horas horageral;

    // Novo: mapa camada -> prédio escolhido (endereços específicos por pessoa)
    // Nota: Dictionary não é serializável no Inspector, expomos uma lista serializável
    // para visualização/edição no editor e mantemos o dicionário em runtime.
    [System.NonSerialized]
    public Dictionary<string, mPredios> EnderecosPorCamada = new Dictionary<string, mPredios>();

    [System.Serializable]
    public struct EnderecoEntry
    {
        public string camada;
        public mPredios predio;
    }

    [Tooltip("Visualização / edição dos endereços por camada (sincronizado com EnderecosPorCamada)")]
    [SerializeField]
    public List<EnderecoEntry> enderecosEditor = new List<EnderecoEntry>();

    // Rotina desta pessoa (lista de tarefas horárias resolvidas a partir do molde)
    public List<TarefaRotina> rotinaListaTarefa = new List<TarefaRotina>();

    public cPessoa(string mnome)
    {
        this.identidade = mnome;
    }

    public cPessoa(GameObject prefab, string mnome)
    {
        this.tipoPessoa = prefab;
        this.identidade = mnome;
    }

    public void Inicializar()
    {
        DebugController.Log(DebugCategoria.tracking, $"START/Inicializar ▸ tracking");

        ambiente = GameObject.Find("ambiente")?.GetComponent<Gerente_de_ambiente>();
        gerente_populacao = GameObject.Find("ambiente")?.GetComponent<gestor_populacao>();
        as_pessoas = GameObject.Find("as_pessoas");

        // atribui horageral procurando o componente 'horas' no mesmo GameObject do ambiente,
        // com fallback para FindObjectOfType caso não encontre.
        if (ambiente != null)
            horageral = ambiente.GetComponent<horas>() ?? GameObject.FindObjectOfType<horas>();
        else
            horageral = GameObject.FindObjectOfType<horas>();

        // NÃO adicionamos InstanciaPessoa como listener antecipado.
        // Queremos garantir que InicializaEnderecos() rode antes de InstanciaPessoa().

        if (ambiente != null && ambiente.mlista_dos_predios != null && ambiente.mlista_dos_predios.Count > 0 && ambiente.mCamadas != null && ambiente.mCamadas.Count > 0)
        {
            // se prédios já carregados: inicializa endereços e em seguida instancia pessoa
            InicializaEnderecos();
            // após inicializar endereços, instanciamos a pessoa (garante ordem desejada)
            InstanciaPessoa();
        }
        else if (ambiente != null)
        {
            // espera evento de prédios carregados; quando ocorrer, inicializa endereços e instancia
            ambiente.OnPrediosCarregados.AddListener(() =>
            {
                InicializaEnderecos();
                InstanciaPessoa();
            });
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.cPessoa, "Inicializar ▸ ambiente não encontrado.");
        }

        DebugController.Log(DebugCategoria.cPessoa, $"START/Inicializar ▸ ");
    }

    public void InstanciaPessoa()
    {
        // validações básicas
        if (gerente_populacao == null)
        {
            DebugController.LogError(DebugCategoria.cPessoa, "InstanciaPessoa ▸ gerente_populacao é nulo.");
            return;
        }
        if (gerente_populacao.pfGente == null)
        {
            DebugController.LogError(DebugCategoria.cPessoa, "InstanciaPessoa ▸ prefab de gente (pfGente) não está atribuído em gestor_populacao.");
            return;
        }

        // Determina prédio de spawn: prefira o prédio da primeira tarefa ativa da rotina,
        // se disponível; senão use MminhaCasa (se definida) ou qualquer prédio atribuído.
        mPredios spawnPredio = null;

        // tenta tarefa ativa agora (se rotina definida)
        try
        {
            int horaAtual = -1;
            if (horageral != null)
            {
                // usa horageral (componente horas do GameObject 'ambiente')
                horaAtual = Mathf.FloorToInt((float)horageral.hora) % 24;
            }
            else
            {
                var relogio = GameObject.FindObjectOfType<horas>();
                if (relogio != null) horaAtual = Mathf.FloorToInt((float)relogio.hora) % 24;
            }

            TarefaRotina tarefaInicial = null;
            if (horaAtual >= 0 && rotinaListaTarefa != null && rotinaListaTarefa.Count > 0)
            {
                tarefaInicial = rotinaListaTarefa.FirstOrDefault(t => t != null && t.ContemHora(horaAtual));
            }
            if (tarefaInicial == null && rotinaListaTarefa != null && rotinaListaTarefa.Count > 0)
            {
                tarefaInicial = rotinaListaTarefa[0];
            }
            if (tarefaInicial != null)
            {
                // tenta resolver prédio do mapeamento EnderecosPorCamada
                if (!string.IsNullOrEmpty(tarefaInicial.atividadeId))
                {
                    EnderecosPorCamada.TryGetValue(tarefaInicial.atividadeId, out spawnPredio);
                }
            }
        }
        catch { /* não crítico */ }

        // fallbacks
        if (spawnPredio == null) spawnPredio = MminhaCasa;
        if (spawnPredio == null && EnderecosPorCamada.Count > 0) spawnPredio = EnderecosPorCamada.Values.FirstOrDefault();

        if (spawnPredio == null)
        {
            DebugController.LogWarning(DebugCategoria.cPessoa, "InstanciaPessoa ▸ Nenhum prédio disponível para spawn.");
            return;
        }

        DebugController.Log(DebugCategoria.cPessoa, $"InstanciaPessoa ▸ spawn em: '{spawnPredio.nomePredio}', coord {spawnPredio.enderecoXYZ}");

        // Instancia o prefab
        objPessoa = UnityEngine.Object.Instantiate(gerente_populacao.pfGente, spawnPredio.enderecoXYZ, Quaternion.identity);

        // Ajustar a posição para o ponto válido na NavMesh — TELEPORT (warp) para início da rotina
        UnityEngine.AI.NavMeshHit hit;
        Vector3 destino = spawnPredio.enderecoXYZ;
        if (UnityEngine.AI.NavMesh.SamplePosition(destino, out hit, 10.0f, UnityEngine.AI.NavMesh.AllAreas))
            destino = hit.position;

        // calcula altura do prefab para posicionamento correto
        float alturaPrefab = 0f;
        var col = objPessoa.GetComponent<Collider>();
        if (col != null) alturaPrefab = col.bounds.extents.y;
        else
        {
            var rend = objPessoa.GetComponent<Renderer>();
            if (rend != null) alturaPrefab = rend.bounds.extents.y;
        }

        Vector3 posFinal = destino + new Vector3(0, alturaPrefab > 0f ? alturaPrefab : 0.1f, 0);

        // Se o prefab tiver NavMeshAgent, use Warp para teleporte seguro; caso contrário set position
        var nav = objPessoa.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null)
        {
            // garante que agente não mova antes de estar posicionado
            nav.Warp(posFinal);
        }
        else
        {
            objPessoa.transform.position = posFinal;
        }

        Debug.Log($"Instanciado o agente {identidade} na posição válida da NavMesh: {posFinal}");

        // cria componente Rotina e associa pessoa (Rotina fará a execução quando o relógio avançar)
        Rotina rotina = objPessoa.AddComponent<Rotina>();
        rotina.pessoa = this;

        objPessoa.name = identidade;
        objPessoa.transform.parent = as_pessoas != null ? as_pessoas.transform : null;
        objPessoa.tag = "pessoas";

        // garante Rigidbody kinematic para triggers dos prédios
        Rigidbody rb = objPessoa.GetComponent<Rigidbody>();
        if (rb == null) rb = objPessoa.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;

        objPessoa.AddComponent<ParaConectar>();
        objPessoa.AddComponent<CaminhoTrail>();
    }

    // Escolhe aleatoriamente um prédio da lista dando preferência aos que ainda têm vaga
    // tenta reservar a vaga chamando AdicionarMorador() até encontrar uma com sucesso ou esgotar tentativas.
    private mPredios EscolherEReservarPredio(List<mPredios> lista)
    {
        if (lista == null || lista.Count == 0) return null;

        // 1) candidatos com vaga
        var candidatos = lista.Where(p => p != null && p.TemVaga()).ToList();
        // embaralha acesso ao randomizar por índice
        if (candidatos.Count > 0)
        {
            for (int attempt = 0; attempt < candidatos.Count; attempt++)
            {
                int idx = Random.Range(0, candidatos.Count);
                var candidato = candidatos[idx];
                if (candidato == null) { candidatos.RemoveAt(idx); continue; }
                if (candidato.AdicionarMorador())
                {
                    return candidato;
                }
                else
                {
                    // removemos candidato que falhou e tentamos outro
                    candidatos.RemoveAt(idx);
                }
            }
        }

        // 2) nenhum com vaga ou todas falharam: tenta reservar em qualquer prédio da lista
        var all = new List<mPredios>(lista);
        for (int attempt = 0; attempt < all.Count; attempt++)
        {
            int idx = Random.Range(0, all.Count);
            var p = all[idx];
            if (p == null) { all.RemoveAt(idx); continue; }
            if (p.AdicionarMorador())
            {
                return p;
            }
            else
            {
                all.RemoveAt(idx);
            }
        }

        // 3) se nada pôde reservar, retorna um aleatório (sem reserva)
        return lista[Random.Range(0, lista.Count)];
    }

    public void InicializaEnderecos()
    {
        DebugController.Log(DebugCategoria.tracking, $"InicializaEnderecos ▸ tracking");

        EnderecosPorCamada.Clear();
        MminhaCasa = null; MmeuTrabalho = null; MmeuRestaurante = null;
        rotinaListaTarefa.Clear();
        enderecosEditor.Clear();

        if (ambiente == null || ambiente.mCamadas == null || ambiente.mCamadas.Count == 0)
        {
            DebugController.LogWarning(DebugCategoria.cPessoa, "InicializaEnderecos ▸ nenhuma camada disponível no ambiente.");
            OnCasaDefinida.Invoke();
            return;
        }

        // Se existem modelos configurados pelo usuário, tente usar o tipo atribuído a esta pessoa
        if (ambiente.lista_tipos_pessoas != null && ambiente.lista_tipos_pessoas.Count > 0)
        {
            molde_pessoas chosenModel = null;

            // se tipoPessoa (prefab) estiver setado, tente casar pelo nome
            if (tipoPessoa != null)
            {
                string nomeTipoPrefab = tipoPessoa.name.ToLower();
                chosenModel = ambiente.lista_tipos_pessoas.FirstOrDefault(m => !string.IsNullOrEmpty(m.tipo_pessoa) && m.tipo_pessoa.ToLower() == nomeTipoPrefab);
            }

            // se não encontrou por prefab, tenta casar pela string identidade/tipo_pessoa
            if (chosenModel == null && !string.IsNullOrEmpty(identidade))
            {
                string nomeId = identidade.ToLower();
                chosenModel = ambiente.lista_tipos_pessoas.FirstOrDefault(m => !string.IsNullOrEmpty(m.tipo_pessoa) && nomeId.Contains(m.tipo_pessoa.ToLower()));
            }

            // fallback: pega primeiro modelo disponível
            if (chosenModel == null) chosenModel = ambiente.lista_tipos_pessoas[Random.Range(0, ambiente.lista_tipos_pessoas.Count)];

            DebugController.Log(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ Usando molde de pessoa '{chosenModel.tipo_pessoa}'");

            // obtém tarefas definidas no molde
            var tarefasDoMolde = chosenModel.GetTarefas();

            // Se o molde não fornecer tarefas, ignoramos o molde e caímos no fluxo padrão
            if (tarefasDoMolde == null || tarefasDoMolde.Count == 0)
            {
                DebugController.Log(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ molde '{chosenModel.tipo_pessoa}' não definiu tarefas — usando rotina padrão.");
            }
            else
            {
                foreach (var tarefa in tarefasDoMolde)
                {
                    if (tarefa == null || string.IsNullOrEmpty(tarefa.atividadeId)) continue;
                    string atividade = tarefa.atividadeId.ToLower();

                    // encontrar a camada que corresponde à atividade (substring)
                    string camadaMatch = ambiente.mCamadas.Keys.FirstOrDefault(k => k.ToLower().Contains(atividade));
                    if (camadaMatch == null)
                    {
                        camadaMatch = ambiente.mCamadas.Keys.FirstOrDefault(k => k.ToLower() == atividade);
                    }
                    if (camadaMatch == null)
                    {
                        DebugController.LogWarning(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ atividade '{atividade}' sem camada correspondente. Pulando.");
                        continue;
                    }

                    var listaPredios = ambiente.mCamadas[camadaMatch];
                    if (listaPredios == null || listaPredios.Count == 0)
                    {
                        DebugController.LogWarning(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ camada '{camadaMatch}' sem prédios.");
                        continue;
                    }

                    // escolhe prédio aleatoriamente entre os com vaga e tenta reservar
                    mPredios escolhido = EscolherEReservarPredio(listaPredios);

                    if (escolhido == null)
                    {
                        DebugController.LogWarning(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ falha ao selecionar prédio para camada '{camadaMatch}'.");
                        continue;
                    }

                    EnderecosPorCamada[camadaMatch] = escolhido;

                    string lowerCam = camadaMatch.ToLower();
                    if (lowerCam.Contains("casa") && MminhaCasa == null) MminhaCasa = escolhido;
                    if (lowerCam.Contains("trabalho") && MmeuTrabalho == null) MmeuTrabalho = escolhido;
                    if (lowerCam.Contains("restaurante") && MmeuRestaurante == null) MmeuRestaurante = escolhido;

                    // adiciona a tarefa (mantendo a hora/horaFim do molde)
                    var copia = new TarefaRotina
                    {
                        horaInicio = tarefa.horaInicio,
                        horaFim = tarefa.horaFim,
                        atividadeId = camadaMatch, // normaliza para a chave da camada encontrada
                        nomePredioPreferido = tarefa.nomePredioPreferido,
                        duracaoEsperadaHoras = tarefa.duracaoEsperadaHoras,
                        prioridade = tarefa.prioridade
                    };
                    rotinaListaTarefa.Add(copia);
                }

                DebugController.Log(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ endereços atribuídos pelo molde: {EnderecosPorCamada.Count}");
                // sincroniza lista serializável para exibição/edição no Inspector
                UpdateEditorListFromDict();
                OnCasaDefinida.Invoke();
                return;
            }
        }

        // Caso padrão: nenhum tipo definido pelo usuário — cria rotina circular com 1 prédio por camada
        var camadaNomes = ambiente.mCamadas.Keys
            .Where(k => { var lk = k.ToLower(); return lk != "rua" && lk != "escala"; })
            .Select(k => k.Trim())
            .Distinct(System.StringComparer.OrdinalIgnoreCase)
            .OrderBy(k => k.ToLower())
            .ToList();

        if (camadaNomes.Count == 0)
        {
            DebugController.LogWarning(DebugCategoria.cPessoa, "InicializaEnderecos ▸ nenhuma camada válida disponível para rotina circular.");
            OnCasaDefinida.Invoke();
            return;
        }

        // Seleciona um prédio por camada (preferência por vaga) — agora aleatório entre com vaga
        foreach (var camada in camadaNomes)
        {
            var lista = ambiente.mCamadas[camada];
            if (lista == null || lista.Count == 0) continue;

            mPredios escolhido = EscolherEReservarPredio(lista);
            if (escolhido == null)
            {
                DebugController.LogWarning(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ registro em '{camada}' falhou ao criar padrão.");
                continue;
            }

            EnderecosPorCamada[camada] = escolhido;

            string lower = camada.ToLower();
            if (lower.Contains("casa") && MminhaCasa == null) MminhaCasa = escolhido;
            if (lower.Contains("trabalho") && MmeuTrabalho == null) MmeuTrabalho = escolhido;
            if (lower.Contains("restaurante") && MmeuRestaurante == null) MmeuRestaurante = escolhido;
        }

        // Monta rotina circular dividindo 24h equally
        int n = EnderecosPorCamada.Count;
        if (n == 0)
        {
            DebugController.LogWarning(DebugCategoria.cPessoa, "InicializaEnderecos ▸ nenhuma camada com prédios disponível para rotina.");
            OnCasaDefinida.Invoke();
            return;
        }
        int baseBlock = 24 / n;
        int extra = 24 - baseBlock * n; // distribuir sobra nos primeiros
        int hour = 0;
        var camadaList = EnderecosPorCamada.Keys.ToList();
        for (int i = 0; i < camadaList.Count; i++)
        {
            int dur = baseBlock + (i < extra ? 1 : 0);
            int start = hour;
            int end = (hour + dur) % 24;
            rotinaListaTarefa.Add(new TarefaRotina { horaInicio = start, horaFim = end, atividadeId = camadaList[i] });
            hour += dur;
        }

        DebugController.Log(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ rotina circular criada com {rotinaListaTarefa.Count} entradas.");
        UpdateEditorListFromDict();
        OnCasaDefinida.Invoke();
    }

    // sincroniza a lista visível no Inspector a partir do dicionário em runtime
    private void UpdateEditorListFromDict()
    {
        enderecosEditor.Clear();
        if (EnderecosPorCamada == null) EnderecosPorCamada = new Dictionary<string, mPredios>();
        foreach (var kv in EnderecosPorCamada)
        {
            enderecosEditor.Add(new EnderecoEntry { camada = kv.Key, predio = kv.Value });
        }
    }

    // aplica mudanças feitas no Inspector (enderecosEditor) para o dicionário em runtime
    // Útil se você editar enderecos no Inspector para testar e querer que runtime reflita
    public void ApplyEditorListToDict()
    {
        EnderecosPorCamada.Clear();
        foreach (var e in enderecosEditor)
        {
            if (string.IsNullOrEmpty(e.camada) || e.predio == null) continue;
            EnderecosPorCamada[e.camada] = e.predio;
        }
        // reconstrói referências especiais (casa/trabalho/restaurante)
        AssignSpecialPrediosFromDict();
    }

    private void AssignSpecialPrediosFromDict()
    {
        // reset
        MminhaCasa = null; MmeuTrabalho = null; MmeuRestaurante = null;
        foreach (var kv in EnderecosPorCamada)
        {
            var lower = kv.Key.ToLower();
            if (lower.Contains("casa") && MminhaCasa == null) MminhaCasa = kv.Value;
            if (lower.Contains("trabalho") && MmeuTrabalho == null) MmeuTrabalho = kv.Value;
            if (lower.Contains("restaurante") && MmeuRestaurante == null) MmeuRestaurante = kv.Value;
        }
    }

    public void despedida()
    {
        DebugController.Log(DebugCategoria.tracking, $"despedida ▸ tracking");

        MminhaCasa?.RemoverMorador();
        MmeuTrabalho?.RemoverMorador();
        MmeuRestaurante?.RemoverMorador();
        foreach (var kv in EnderecosPorCamada)
        {
            // se o prédio não foi o mesmo já removido acima, remova a vaga
            if (kv.Value != null && kv.Value != MminhaCasa && kv.Value != MmeuTrabalho && kv.Value != MmeuRestaurante)
                kv.Value.RemoverMorador();
        }

        Debug.Log("fui destruido: " + identidade);
        DebugController.LogWarning(DebugCategoria.cPessoa, $"despedida ▸ fui destruido:'{ identidade}'");
    }
}



using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rotina : MonoBehaviour
{
    private HashSet<int> minutosAlarmeRegistrados = new HashSet<int>();

    public cPessoa pessoa;
    private NavMeshAgent agente;
    private horas relogio;
    private float hora_instantanea; // LEGADO

    // LEGADO: não usado na versão atual
    private Dictionary<string, NavMeshPath> pathCache = new Dictionary<string, NavMeshPath>();


    const float MIN_SPEED = 0.5f;
    const float MAX_SPEED = 30f;

    private string currentActivityId = null;
    private Vector3 currentDestination = Vector3.zero;
    private const float destinationTolerance = 1.0f;

    // Sequência de atividades ordenada (respeitando wrap) com destino e caminho pré-calculado para o próximo
    private class SequenceEntry
    {
        public TarefaRotina tarefa;
        public mPredios predio;
        public Vector3 pos;
        public NavMeshPath pathToNext;
        public string atividadeId;
        public int horaInicio;
        public int horaFim;
    }
    private List<SequenceEntry> sequence = new List<SequenceEntry>();
    // LEGADO: sequência antiga (não usada)
    //private List<object> sequence = new List<object>();

    // LEGADO: cache de próxima rota (não usado)
    private NavMeshPath cachedNextPath = null;
    private Vector3 cachedNextDestination = Vector3.zero;
    private string cachedNextActivityId = null;
    private bool nextPathReady = false;

    private bool hasArrived = false;

    void Awake()
    {
        DebugController.Log(DebugCategoria.tracking, $"Awake ▸ tracking");

        agente = GetComponent<NavMeshAgent>();
        if (agente == null)
        {
            DebugController.LogWarning(DebugCategoria.Rotina, $"Awake ▸ NavMeshAgent não encontrado! Criando um automaticamente.");
            agente = gameObject.AddComponent<NavMeshAgent>();
        }

        // parâmetros razoáveis (ajuste conforme necessário)
        agente.speed = 16f;
        agente.acceleration = 38f;
        agente.angularSpeed = 760f; // aumentei angularSpeed base para permitir curvas mais rápidas
        agente.radius = 0.25f;
        agente.stoppingDistance = 0.8f;
        agente.autoBraking = true;
        agente.updateRotation = true;

        // DESABILITA autoRepath para evitar recalculos automáticos que causam zigzag
        agente.autoRepath = false;

        // Ajuste inicial de avoidancePriority para reduzir jitter em alta velocidade (compatível com todas as versões)
        // avoidancePriority: 0 (mais alta prioridade) .. 99 (menor prioridade). Valores médios padrão = 50.
        agente.avoidancePriority = 50;
    }

    void Start()
    {
        DebugController.Log(DebugCategoria.tracking, $"Start ▸ tracking");

        // encontra relógio primeiro (se existir)
        relogio = FindObjectOfType<horas>();

        // monta sequência de atividades (ordenando considerando wrap pela meia-noite) e pré-calcula caminhos t_i -> t_{i+1}
        // LEGADO
        // BuildSequenceRespectingWrap();

        // Se relógio existe, liga listener e posiciona agente na tarefa ativa no horário inicial (o relógio começa em 00:00)
        if (relogio != null)
        {
            //            relogio.MudouHora.AddListener(AtualizaDestino);
            RegistrarAlarmesDaRotina();

            // hora atual do relógio (padrão 0 se por acaso inválido)
            //            int horaInicio = Mathf.FloorToInt(relogio.hora) % 24;
            //            MoveAgentToTaskForHour(horaInicio);
            MoveAgentToTaskInicial(); // MoveAgentToTaskForMinute(relogio.minutoDoDia);

            // opcional: chama para garantir que a lógica de destino seja aplicada conforme hora atual
            AtualizaDestino();
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "Relógio (horas) não encontrado na cena. Posicionando com base em 00:00.");
            // assume 00h se não há relógio
            //            MoveAgentToTaskForHour(0);
            MoveAgentToTaskInicial(); // MoveAgentToTaskForMinute(0);
        }
    }


    void Update()
    {
        /*
        // Checa chegada ao destino para preparar rota da próxima atividade (pré-cálculo)
        if (agente != null && agente.isOnNavMesh)
        {
            if (agente.hasPath && !agente.pathPending)
            {
                if (!hasArrived && agente.remainingDistance <= agente.stoppingDistance + destinationTolerance)
                {
                    // marcou chegada
                    hasArrived = true;
                    DebugController.Log(DebugCategoria.Rotina, $"Update ▸ {pessoa?.identidade} chegou ao destino '{currentActivityId}' → preparando próxima rota.");
                 //   PrepareNextPath();
                }
                else if (hasArrived && agente.remainingDistance > agente.stoppingDistance + destinationTolerance)
                {
                    // saiu do estado de "chegado" (recomeçou a andar)
                    hasArrived = false;
                }
            }
        }
        */
    }

    private void RegistrarAlarmesDaRotina()
    {
        if (relogio == null)
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "RegistrarAlarmesDaRotina ▸ relógio não encontrado.");
            return;
        }

        if (pessoa == null || pessoa.minhaRotinaBase == null || pessoa.minhaRotinaBase.slots == null || pessoa.minhaRotinaBase.slots.Count == 0)
        {
            DebugController.LogWarning(DebugCategoria.Rotina, $"RegistrarAlarmesDaRotina ▸ rotina base ausente para {pessoa?.identidade}.");
            return;
        }

        minutosAlarmeRegistrados.Clear();

        foreach (var slot in pessoa.minhaRotinaBase.slots)
        {
            int minuto = ((slot.inicioMin % 1440) + 1440) % 1440;

            if (minutosAlarmeRegistrados.Add(minuto))
            {
                relogio.RegistrarEventoNoMinuto(minuto, AoDispararAlarmeDeRotina);
                DebugController.Log(DebugCategoria.Rotina,
                    $"RegistrarAlarmesDaRotina ▸ {pessoa.identidade} inscrito no minuto {minuto}.");
            }
        }
    }

    private void LimparAlarmesDaRotina()
    {
        if (relogio == null) return;

        foreach (int minuto in minutosAlarmeRegistrados)
        {
            relogio.RemoverEventoNoMinuto(minuto, AoDispararAlarmeDeRotina);
        }

        minutosAlarmeRegistrados.Clear();
    }

    private void AoDispararAlarmeDeRotina()
    {
        DebugController.Log(DebugCategoria.Rotina, $"AoDispararAlarmeDeRotina ▸ alarme disparado para {pessoa.identidade} no minuto {relogio.minutoDoDia}.");
        AtualizaDestino();
    }

    // ===============================
    //  SLOT ATIVO
    // ===============================
    private bool GetSlotAtivo(int minuto, out RotinaSlot slot)
    {
        slot = default;

        if (pessoa?.minhaRotinaBase?.slots == null) return false;

        minuto = ((minuto % 1440) + 1440) % 1440;

        foreach (var s in pessoa.minhaRotinaBase.slots)
        {
            if (s.inicioMin <= minuto && minuto < s.fimMin)
            {
                slot = s;
                return true;
            }
        }

        return false;
    }

    // ===============================
    //  POSICIONAMENTO INICIAL
    // ===============================
    private void MoveAgentToTaskInicial()
    {
        if (relogio == null) return;

        if (!GetSlotAtivo(relogio.minutoDoDia, out RotinaSlot slot))
            return;

        var pred = ResolvePredio(slot.tipoLugarId);
        if (pred == null) return;

        Vector3 destino = pred.enderecoXYZ;

        if (NavMesh.SamplePosition(destino, out var hit, 10f, NavMesh.AllAreas))
            destino = hit.position;

        agente.Warp(destino);
        agente.ResetPath();

        currentActivityId = slot.tipoLugarId;
        currentDestination = destino;
        hasArrived = true;
    }

    // ===============================
    //  ATUALIZA DESTINO
    // ===============================
    // Atualiza destino com base na hora do relógio e nas tarefas da pessoa
    private void AtualizaDestino()
    {
        if (pessoa == null)
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "AtualizaDestino ▸ pessoa não atribuída ao componente Rotina.");
            return;
        }

        if (relogio == null)
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "AtualizaDestino ▸ relógio não encontrado.");
            return;
        }

        // Checa se estamos rodando o dia; se não, não altera destino
        if (!relogio.rodadia)
        {
            DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ rodadia == false, pulando atualização para {pessoa.identidade}");
            return;
        }

//        hora_instantanea = relogio.hora;
//        int horaInt = Mathf.FloorToInt(hora_instantanea) % 24;
        int minutoAtual = relogio.minutoDoDia;

        //var tarefa = GetTarefaAtivaPorMinuto(relogio.minutoDoDia);
        //if (tarefa == null)
        //{ DebugController.LogWarning(DebugCategoria.Rotina, $"AtualizaDestino ▸ nenhuma tarefa encontrada para {pessoa.identidade} às {horaInt}h.");
        //  return;}
        if (!GetSlotAtivo(relogio.minutoDoDia, out RotinaSlot slot))
            return;

        //string newActivityId = tarefa.atividadeId;
        //var predDestino = ResolvePredioParaTarefa(tarefa);
        //if (predDestino == null)
        //{ DebugController.LogWarning(DebugCategoria.Rotina, $"AtualizaDestino ▸ nenhum prédio encontrado para atividade '{tarefa.atividadeId}' de {pessoa.identidade}.");
        //  return; }
        string newActivityId = slot.tipoLugarId;

        var pred = ResolvePredio(newActivityId);
        if (pred == null) return;

        Vector3 destinoPos = pred.enderecoXYZ;// predDestino.enderecoXYZ;
        if (NavMesh.SamplePosition(destinoPos, out var hitTo, 10.0f, NavMesh.AllAreas))
            destinoPos = hitTo.position;

        DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ {pessoa.identidade} hora {relogio.minutoDoDia} → atividade '{newActivityId}' no '{pred.nomePredio}'");

        // Se a nova atividade é a mesma que a atual e o destino também, não altera
        //        if (!string.IsNullOrEmpty(currentActivityId) && currentActivityId == newActivityId &&
        //            (currentDestination - destinoPos).sqrMagnitude <= (destinationTolerance * destinationTolerance))
        //        {   DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ atividade inalterada para {pessoa.identidade} ('{currentActivityId}'), pulando.");
        //            return;        }
        bool mesmoDestino = (currentDestination - destinoPos).sqrMagnitude <= (destinationTolerance * destinationTolerance);
        bool mesmaAtividade = currentActivityId == newActivityId;
        if (mesmaAtividade && mesmoDestino) return;

        /*
        // Se agente está a caminho (ainda longe do destino), não forçar mudança agora — espera chegada.
//        if (agente != null && agente.hasPath && !agente.pathPending && agente.remainingDistance > agente.stoppingDistance + destinationTolerance)
//        {            DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ {pessoa.identidade} está em trânsito (remaining {agente.remainingDistance:F2}m) — adiando troca para chegada.");
//            return;        }

        // Se existe cached next path e corresponde à nova atividade, use-a
 //       if (nextPathReady && cachedNextActivityId == newActivityId)
 //       {
            // configure agent parameters for cached path before applying
            //       ConfigureAgentForTurning(cachedNextPath, agente.speed);
            //       agente.SetPath(cachedNextPath);
            //       currentActivityId = newActivityId;
            //       currentDestination = cachedNextDestination;
            //       nextPathReady = false;
            //       cachedNextPath = null;
            //       hasArrived = false;
            //       DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ Usando cachedNextPath para {pessoa.identidade} → {currentActivityId}");
 //           return;
 //       }
 */
        // Caso contrário calcula caminho agora e aplica
        // garante que agente esteja sobre NavMesh (warp se necessário)
        if (!agente.isOnNavMesh)
        {
        //    NavMeshHit sample;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit sample, 2f, NavMesh.AllAreas))
            {
                agente.Warp(sample.position);
            }
//            else
//            {
//                if (NavMesh.SamplePosition(destinoPos, out var s2, 10f, NavMesh.AllAreas))
 //                   agente.Warp(s2.position);
 //           }
        }

        agente.SetDestination(destinoPos);

        currentActivityId = newActivityId;
        currentDestination = destinoPos;
        hasArrived = false;

        /*
        var pathNow = new NavMeshPath();
        bool ok = agente.CalculatePath(destinoPos, pathNow);
        AjustarVelocidadeParaChegarRapido(pathNow, agente.transform.position, destinoPos, tarefa);

        if (ok && pathNow.status == NavMeshPathStatus.PathComplete)
        {
            // configure agent params for this path (curvatura) antes de aplicar
            ConfigureAgentForTurning(pathNow, agente.speed);
            agente.SetPath(pathNow);
            currentActivityId = newActivityId;
            currentDestination = destinoPos;
            hasArrived = false;
            nextPathReady = false;
            cachedNextPath = null;
            DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ path calculado e aplicado para {pessoa.identidade} → {currentActivityId}");
        }
        else
        {
            // fallback sem path completo: ainda aplique ajuste simples
            ConfigureAgentForTurning(null, agente.speed);
            agente.SetDestination(destinoPos);
            currentActivityId = newActivityId;
            currentDestination = destinoPos;
            hasArrived = false;
            nextPathReady = false;
            cachedNextPath = null;
            DebugController.LogWarning(DebugCategoria.Rotina, $"AtualizaDestino ▸ SetDestination aplicado (fallback) para {pessoa.identidade} → {currentActivityId}");
        } */
    }

    // ===============================
    //  RESOLVE PRÉDIO
    // ===============================
    private mPredios ResolvePredio(string atividadeKey)
    {
        if (string.IsNullOrEmpty(atividadeKey)) return null;

        if (pessoa.EnderecosPorCamada.TryGetValue(atividadeKey, out var pred) && pred != null)
            return pred;

        var env = pessoa.ambiente;

        if (env?.mCamadas != null)
        {
            if (env.mCamadas.TryGetValue(atividadeKey, out var lista) && lista.Count > 0)
                return lista[0];
        }

        return null;
    }

    // CONSTRUÇÃO DA SEQUÊNCIA: ordena por horaInicio e rotaciona para começar pela tarefa que contém 00:00 (se houver)
    private void BuildSequenceRespectingWrap()
    {
        sequence.Clear();

        if (pessoa == null || pessoa.rotinaListaTarefa == null || pessoa.rotinaListaTarefa.Count == 0) return;

        var tmp = new List<SequenceEntry>();
        foreach (var t in pessoa.rotinaListaTarefa)
        {
            if (t == null) continue;
            var pred = ResolvePredioParaTarefa(t);
            if (pred == null) continue;

            Vector3 pos = pred.enderecoXYZ;
            if (NavMesh.SamplePosition(pos, out var hit, 10f, NavMesh.AllAreas))
                pos = hit.position;

            tmp.Add(new SequenceEntry
            {
                tarefa = t,
                predio = pred,
                pos = pos,
                atividadeId = t.atividadeId,
                horaInicio = t.horaInicio,
                horaFim = t.horaFim,
                pathToNext = null
            });
        }

        if (tmp.Count == 0) return;

        // ordena por horaInicio ascendente
        tmp.Sort((a, b) => a.horaInicio.CompareTo(b.horaInicio));

        // procura index da tarefa que contém 00:00 (meia-noite). se nenhuma, começa pela menor horaInicio
        int startIdx = tmp.FindIndex(x => x.tarefa != null && x.tarefa.ContemHora(0));
        if (startIdx < 0) startIdx = 0;

        // rotaciona a lista para começar em startIdx
        sequence = new List<SequenceEntry>();
        for (int i = 0; i < tmp.Count; i++)
        {
            sequence.Add(tmp[(startIdx + i) % tmp.Count]);
        }

        // pré-calcula caminhos entre cada entry e a próxima (wrap)
        for (int i = 0; i < sequence.Count; i++)
        {
            int next = (i + 1) % sequence.Count;
            Vector3 from = sequence[i].pos;
            Vector3 to = sequence[next].pos;

            // garante amostragem em NavMesh
            if (NavMesh.SamplePosition(from, out var fHit, 2f, NavMesh.AllAreas))
                from = fHit.position;
            if (NavMesh.SamplePosition(to, out var tHit, 10f, NavMesh.AllAreas))
                to = tHit.position;

            var path = GetOrCalculatePath(from, to);
            sequence[i].pathToNext = path;
            if (path != null && path.status == NavMeshPathStatus.PathComplete)
                DebugController.Log(DebugCategoria.Rotina, $"BuildSequence ▸ cached path {sequence[i].atividadeId} -> {sequence[next].atividadeId} for {pessoa.identidade}");
            else
                DebugController.LogWarning(DebugCategoria.Rotina, $"BuildSequence ▸ cálculo parcial para {sequence[i].atividadeId} -> {sequence[next].atividadeId} de {pessoa.identidade}");
        }
    }

    // Move / posiciona o agente para a tarefa que corresponde à hora fornecida.
    // Preferência: evita cálculos caros — warpa o agente para a posição correta no Start.
    //VELHA, VAI SER DESCONTINUADA
    private void MoveAgentToTaskForHour(int hora)
    {
        if (sequence == null || sequence.Count == 0) return;

        hora = ((hora % 24) + 24) % 24;
        SequenceEntry target = null;
        foreach (var s in sequence)
        {
            if (s != null && s.tarefa != null && s.tarefa.ContemHora(hora)) { target = s; break; }
        }
        if (target == null) target = sequence[0];

        Vector3 destino = target.pos;
        // Se agente não está no NavMesh ou está longe do destino, warp para o destino (barato e consistente no Start)
        bool needWarp = true;
        if (agente != null && agente.isOnNavMesh)
        {
            float distSqr = (agente.transform.position - destino).sqrMagnitude;
            if (distSqr <= (destinationTolerance * destinationTolerance)) needWarp = false;
        }

        if (needWarp && agente != null)
        {
            if (NavMesh.SamplePosition(destino, out var sample, 10f, NavMesh.AllAreas))
            {
                agente.Warp(sample.position);
                agente.ResetPath();
                DebugController.Log(DebugCategoria.Rotina, $"MoveAgentToTaskForHour ▸ Warped {pessoa.identidade} para atividade '{target.atividadeId}' (hora {hora})");
            }
            else
            {
                DebugController.LogWarning(DebugCategoria.Rotina, $"MoveAgentToTaskForHour ▸ Não foi possível amostrar NavMesh para destino de '{target.atividadeId}'");
            }
        }

        currentActivityId = target.atividadeId;
        currentDestination = destino;
        hasArrived = true;
    }
 
    // NOVA MoveAgentToTask, baseada em minuto e na nova estrutura de rotina (minhaRotinaBase.slots)
    private void MoveAgentToTaskForMinute(int minuto)
    {
        var tarefa = GetTarefaAtivaPorMinuto(minuto);
        if (tarefa == null) return;

        var predDestino = ResolvePredioParaTarefa(tarefa);
        if (predDestino == null) return;

        Vector3 destino = predDestino.enderecoXYZ;

        if (NavMesh.SamplePosition(destino, out var sample, 10f, NavMesh.AllAreas))
            destino = sample.position;

        bool needWarp = true;
        if (agente != null && agente.isOnNavMesh)
        {
            float distSqr = (agente.transform.position - destino).sqrMagnitude;
            if (distSqr <= (destinationTolerance * destinationTolerance))
                needWarp = false;
        }

        if (needWarp && agente != null)
        {
            agente.Warp(destino);
            agente.ResetPath();
        }

        currentActivityId = tarefa.atividadeId;
        currentDestination = destino;
        hasArrived = true;
    }

    // Determina a tarefa ativa para a hora atual  VAI SER DESCONTINUADA (mantive para compatibilidade)
    private TarefaRotina GetTarefaAtiva(int hora)
    {
        if (pessoa == null || pessoa.rotinaListaTarefa == null || pessoa.rotinaListaTarefa.Count == 0)
            return null;

        foreach (var t in pessoa.rotinaListaTarefa)
        {
            if (t != null && t.ContemHora(hora)) return t;
        }

        // fallback: retorna a primeira
        return pessoa.rotinaListaTarefa.Count > 0 ? pessoa.rotinaListaTarefa[0] : null;
    }
    //NOVA GERTAREFA
    private TarefaRotina GetTarefaAtivaPorMinuto(int minuto)
    {
        if (pessoa == null || pessoa.minhaRotinaBase == null || pessoa.minhaRotinaBase.slots == null || pessoa.minhaRotinaBase.slots.Count == 0)
            return null;

        minuto = ((minuto % 1440) + 1440) % 1440;

        foreach (var slot in pessoa.minhaRotinaBase.slots)
        {
            if (slot.inicioMin <= minuto && minuto < slot.fimMin)
            {
                return new TarefaRotina
                {
                    horaInicio = slot.inicioMin,
                    horaFim = slot.fimMin,
                    atividadeId = slot.tipoLugarId
                };
            }
        }

        return null;
    }
    // resolve mPredios para uma atividade (tentando fallback quando necessário)
    private mPredios ResolvePredioParaTarefa(TarefaRotina tarefa)
    {
        if (tarefa == null) return null;
        string atividadeKey = tarefa.atividadeId;
        if (string.IsNullOrEmpty(atividadeKey)) return null;

        // tenta obter prédio específico atribuído à pessoa
        if (pessoa.EnderecosPorCamada.TryGetValue(atividadeKey, out var pred) && pred != null)
        {
            // se tem vaga preferencialmente -- aqui usamos TemVaga como heurística
            if (pred.TemVaga()) return pred;
            // se não tem vaga tente fallback abaixo
        }

        // fallback: procura outros prédios na camada (via ambiente.mCamadas)
        var env = pessoa.ambiente;
        if (env != null && env.mCamadas != null)
        {
            // se a chave exata existir, use lista; se não, tenta correspondência por substring
            if (!env.mCamadas.TryGetValue(atividadeKey, out var lista))
            {
                string match = null;
                foreach (var k in env.mCamadas.Keys)
                {
                    if (k.ToLower().Contains(atividadeKey.ToLower())) { match = k; break; }
                }
                if (match != null) env.mCamadas.TryGetValue(match, out lista);
            }

            if (lista != null && lista.Count > 0)
            {
                // tenta achar um com vaga
                foreach (var p in lista)
                {
                    if (p != null && p.TemVaga()) return p;
                }
                // se nenhum com vaga, retorna um aleatório como fallback
                return lista[Random.Range(0, lista.Count)];
            }
        }

        return null;
    }

    // Gera chave para cache a partir de duas posições (arredonda para evitar keys infinitas)
    private string PathKey(Vector3 a, Vector3 b)
    {
        string A = $"{Mathf.Round(a.x * 100f) / 100f}_{Mathf.Round(a.z * 100f) / 100f}";
        string B = $"{Mathf.Round(b.x * 100f) / 100f}_{Mathf.Round(b.z * 100f) / 100f}";
        return A + "->" + B;
    }

    // Calcula (ou retorna do cache) um NavMeshPath entre duas posições
    private NavMeshPath GetOrCalculatePath(Vector3 from, Vector3 to)
    {
        string key = PathKey(from, to);
        if (pathCache.TryGetValue(key, out var cached)) return cached;

        var path = new NavMeshPath();
        bool ok = NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);
        // guarda caminho (completo ou parcial) para não recalcular repetidamente
        pathCache[key] = path;
        return path;
    }

    // Calcula o comprimento de um NavMeshPath (soma distances entre corners)
    private float PathLength(NavMeshPath path)
    {
        if (path == null || path.corners == null || path.corners.Length < 2) return 0f;
        float len = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            len += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return len;
    }

    // Calcula duração da tarefa em horas (considerando wrap)
    private float DuracaoHorasDaTarefa(TarefaRotina tarefa)
    {
        if (tarefa == null) return 1f; // fallback 1h
        if (tarefa.duracaoEsperadaHoras > 0f) return tarefa.duracaoEsperadaHoras;
        int start = tarefa.horaInicio % 24;
        int end = tarefa.horaFim % 24;
        int delta = (end - start + 24) % 24;
        if (delta == 0) delta = 24;
        return delta;
    }

    // Ajusta a velocidade do agente para que chegue ao destino em 1/10 do tempo que ficará lá,
    // mas garantindo no máximo 1 hora simulada para a viagem (quando possível).
    private void AjustarVelocidadeParaChegarRapido(NavMeshPath path, Vector3 from, Vector3 to, TarefaRotina tarefa)
    {
        float durHoras = DuracaoHorasDaTarefa(tarefa);

        // Usa hora simulada: cada passo (tempoSimulado segundos) avança 20 minutos (1/3 de hora).
        // Portanto, 1 hora simulada = 3 * tempoSimulado segundos reais.
        float desiredTimeSeconds;
        float secondsPerSimHour = 3600f; // fallback real: 3600s por hora real
        if (relogio != null && relogio.tempoSimulado > 0.0)
        {
            float tempoSim = (float)relogio.tempoSimulado;
            secondsPerSimHour = 3f * tempoSim;
        }

        // objetivo original: 1/10 do tempo de permanência, mantido, mas limitado a no máximo 1 hora simulada
        float desiredSimHours = durHoras / 10f;
        desiredSimHours = Mathf.Min(desiredSimHours, 1f); // nunca mais que 1 hora simulada
        desiredTimeSeconds = desiredSimHours * secondsPerSimHour;

        float distance = 0f;
        if (path != null && path.corners != null && path.corners.Length >= 2)
        {
            distance = PathLength(path);
        }
        else
        {
            distance = Vector3.Distance(from, to);
        }

        if (desiredTimeSeconds <= 0f || distance <= 0f)
        {
            // fallback para velocidade padrão
            agente.speed = Mathf.Clamp(agente.speed, MIN_SPEED, MAX_SPEED);
            agente.acceleration = Mathf.Max(agente.speed * 4f, 8f);
            return;
        }

        float speed = distance / desiredTimeSeconds; // m/s necessário para cumprir objetivo
        // permite velocidades maiores para cumprir o objetivo — porém limitadas a MAX_SPEED plausível
        speed = Mathf.Clamp(speed, MIN_SPEED, MAX_SPEED);

        // se ainda não conseguimos atingir tempo desejado com MAX_SPEED, recompute desiredTime (mínimo possível)
        if (Mathf.Approximately(speed, MAX_SPEED) && distance / MAX_SPEED > desiredTimeSeconds)
        {
            // ajusta desiredTimeSeconds para o tempo necessário com MAX_SPEED — evita overflow de velocidade
            desiredTimeSeconds = distance / MAX_SPEED;
        }

        agente.speed = speed;
        // aceleração proporcional (quanto maior a velocidade, maior a aceleração requerida)
        agente.acceleration = Mathf.Clamp(speed * 15f, 8f, 1000f);

        // Ajusta parâmetros de giro/raio com base na curvatura do caminho
        ConfigureAgentForTurning(path, speed);
    }

    // Analisa path.corners para estimar quão fechadas são as curvas e configura angularSpeed/raio do agente.
    private void ConfigureAgentForTurning(NavMeshPath path, float desiredSpeed)
    {
        if (agente == null) return;

        // valores base
        float baseAngular = 360f; // deg/s
        float maxAngular = 1200f;
        float minRadius = 0.15f;
        float maxRadius = 0.5f;

        // sem path ou poucos corners -> valores conservadores
        if (path == null || path.corners == null || path.corners.Length < 3)
        {
            agente.angularSpeed = Mathf.Clamp(baseAngular * (1f + desiredSpeed / 4f), baseAngular, maxAngular);
            agente.radius = Mathf.Clamp(0.3f, minRadius, maxRadius);
            // ajuste de prioridade de avoidance (compatível com todas as versões do enum)
            agente.avoidancePriority = desiredSpeed > 5f ? 80 : 50;
            return;
        }

        // calcula ângulos entre segmentos — 0 = reta, 180 = retorno
        float maxSharpness = 0f; // 0..1 (1 = curva muito fechada)
        for (int i = 1; i < path.corners.Length - 1; i++)
        {
            Vector3 a = (path.corners[i] - path.corners[i - 1]).normalized;
            Vector3 b = (path.corners[i + 1] - path.corners[i]).normalized;
            float ang = Vector3.Angle(a, b); // 0..180
            float sharpness = Mathf.Clamp01(ang / 180f); // maior ang -> mais afiado
            if (sharpness > maxSharpness) maxSharpness = sharpness;
        }

        // converter sharpness para parâmetros:
        // quanto mais agudo (maxSharpness próximo de 1) -> maior angularSpeed, menor radius
        float angular = Mathf.Lerp(baseAngular, maxAngular, maxSharpness);
        // scale com velocidade: mais velocidade -> precisa de maior angular speed
        angular *= 1f + Mathf.Clamp01(desiredSpeed / 4f);

        float radius = Mathf.Lerp(maxRadius, minRadius, maxSharpness); // mais agudo -> menor radius (permite curva mais fechada)
        radius = Mathf.Clamp(radius, minRadius, maxRadius);

        // ajuste de aceleração adicional para garantir capacidade de rotação em alta velocidade
        float acceleration = Mathf.Clamp(desiredSpeed * 12f * (1f + maxSharpness), 8f, 2000f);

        agente.angularSpeed = Mathf.Clamp(angular, baseAngular, maxAngular);
        agente.radius = radius;
        agente.acceleration = acceleration;

        // Ajuste da prioridade de avoidance em vez de usar enum names que podem variar por versão do Unity
        agente.avoidancePriority = desiredSpeed > 5f ? 80 : 50;

        DebugController.Log(DebugCategoria.Rotina, $"ConfigureAgentForTurning ▸ speed={desiredSpeed:F2}m/s sharp={maxSharpness:F2} ang={agente.angularSpeed:F1} accel={agente.acceleration:F1} radius={agente.radius:F2} avoidancePri={agente.avoidancePriority}");
    }

    // Prepara (pré-calcula) o caminho para a próxima atividade quando a pessoa chega na atual.
    // NÃO muda a rota ativa — apenas armazena em cache para uso quando a mudança de hora exigir.
    private void PrepareNextPath()
    {
        // limpa cache anterior
        nextPathReady = false;
        cachedNextPath = null;
        cachedNextActivityId = null;
        cachedNextDestination = Vector3.zero;

        // preferimos usar a sequência pré-calculada se existir
        if (sequence != null && sequence.Count > 0 && !string.IsNullOrEmpty(currentActivityId))
        {
            int idx = sequence.FindIndex(s => s != null && s.atividadeId == currentActivityId);
            if (idx < 0) idx = 0;
            int nextIdx = (idx + 1) % sequence.Count;

            var nextEntry = sequence[nextIdx];
            if (nextEntry != null)
            {
                cachedNextPath = sequence[idx].pathToNext ?? nextEntry.pathToNext;
                cachedNextDestination = nextEntry.pos;
                cachedNextActivityId = nextEntry.atividadeId;
                nextPathReady = true;
                DebugController.Log(DebugCategoria.Rotina, $"PrepareNextPath ▸ cached next activity '{cachedNextActivityId}' for {pessoa.identidade} (from sequence)");
                return;
            }
        }

        // fallback: usa lista original (pessoa.rotinaListaTarefa) caso sequence não exista
        if (pessoa == null || pessoa.rotinaListaTarefa == null || pessoa.rotinaListaTarefa.Count == 0 || string.IsNullOrEmpty(currentActivityId))
            return;

        int idx2 = pessoa.rotinaListaTarefa.FindIndex(t => t != null && t.atividadeId == currentActivityId);
        if (idx2 < 0) idx2 = 0;
        int nextIdx2 = (idx2 + 1) % pessoa.rotinaListaTarefa.Count;
        var nextTarefa = pessoa.rotinaListaTarefa[nextIdx2];
        if (nextTarefa == null) return;

        var pred = ResolvePredioParaTarefa(nextTarefa);
        if (pred == null) return;

        Vector3 nextDest = pred.enderecoXYZ;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(nextDest, out hit, 10f, NavMesh.AllAreas))
            nextDest = hit.position;

        var path = new NavMeshPath();
        bool ok = agente.CalculatePath(nextDest, path);
        if (ok && path.status == NavMeshPathStatus.PathComplete)
        {
            cachedNextPath = path;
            cachedNextDestination = nextDest;
            cachedNextActivityId = nextTarefa.atividadeId;
            nextPathReady = true;
            DebugController.Log(DebugCategoria.Rotina, $"PrepareNextPath ▸ cached next activity '{cachedNextActivityId}' for {pessoa.identidade} (fallback)");
        }
        else
        {
            // guarda mesmo caminho parcial para fallback
            cachedNextPath = path;
            cachedNextDestination = nextDest;
            cachedNextActivityId = nextTarefa.atividadeId;
            nextPathReady = true;
            DebugController.LogWarning(DebugCategoria.Rotina, $"PrepareNextPath ▸ cálculo parcial para próxima atividade '{cachedNextActivityId}' de {pessoa.identidade} (fallback)");
        }
    }



    public void OnDestroy()
    {
        DebugController.Log(DebugCategoria.tracking, $"OnDestroy ▸ tracking");
        LimparAlarmesDaRotina();
        if (pessoa != null) pessoa.despedida();
    }
}

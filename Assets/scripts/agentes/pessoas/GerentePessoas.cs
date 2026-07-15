using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GerentePessoas : GerenteAgentes <cPessoa, TemplatePessoa, FabricaPessoas>
{
    public IReadOnlyDictionary<int, cPessoa> Individuos 
        => registroAgentes;

    public IReadOnlyDictionary<string, TemplatePessoa> Templates
        => colecaoTemplates;

    public string CaminhoCatalogo
    => repositorioCatalogo?.CaminhoArquivo;

    private RepositorioCatalogoPessoasJson repositorioCatalogo;

    // Start is called before the first frame update

    private void Awake()
    {
        fabrica = new FabricaPessoas();
        colecaoTemplates = new Dictionary<string, TemplatePessoa>();
        registroAgentes = new Dictionary<int, cPessoa>();
        repositorioCatalogo = new RepositorioCatalogoPessoasJson();

        //        percentualPorTemplate = new Dictionary<string, int>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool InicializarCatalogo()
    {
        if (repositorioCatalogo == null)
        {
            repositorioCatalogo =
                new RepositorioCatalogoPessoasJson();
        }

        colecaoTemplates.Clear();

        if (repositorioCatalogo.Existe())
        {
            if (!repositorioCatalogo.TentarCarregar(
                out CatalogoPessoasDados catalogo,
                out string erro))
            {
                Debug.LogError(erro);
                return false;
            }

            foreach (TemplatePessoa template in catalogo.pessoas)
            {
                if (template == null)
                    continue;

                if (string.IsNullOrWhiteSpace(template.tipoId))
                    continue;

                if (template.rotinaBase == null)
                    template.rotinaBase = new RotinaBase();

                colecaoTemplates[template.tipoId] = template;
            }

            Debug.Log(
                $"Catálogo carregado com " +
                $"{colecaoTemplates.Count} tipos de pessoa. " +
                $"Arquivo: {repositorioCatalogo.CaminhoArquivo}");

            VerificarDemografiaCarregada();

            return true;
        }

        InicializarTemplatesBase();

        if (!SalvarCatalogo(out string erroSalvar))
        {
            Debug.LogError(erroSalvar);
            return false;
        }

        Debug.Log(
            $"Catálogo inicial criado com " +
            $"{colecaoTemplates.Count} tipos de pessoa. " +
            $"Arquivo: {repositorioCatalogo.CaminhoArquivo}");

        VerificarDemografiaCarregada();

        return true;
    }

    public void InicializarTemplatesBase()
    {
        List<TemplatePessoa> templates = fabrica.CriarTemplatesBase();

        foreach (TemplatePessoa template in templates)
        {
            colecaoTemplates[template.tipoId] = template;
        }
        foreach (var kv in colecaoTemplates)
        {
            Debug.Log("nome: " + kv.Key);
        }
    }

    public bool SalvarCatalogo(out string erro)
    {
        erro = string.Empty;

        CatalogoPessoasDados catalogo =
            new CatalogoPessoasDados();

        foreach (TemplatePessoa template
            in colecaoTemplates.Values)
        {
            if (template != null)
                catalogo.pessoas.Add(template);
        }

        return repositorioCatalogo.TentarSalvar(
            catalogo,
            out erro);
    }

    private void VerificarDemografiaCarregada()
    {
        if (PercentuaisFecham100(
            out float percentualTotal))
        {
            Debug.Log(
                $"Distribuição populacional válida: " +
                $"{percentualTotal:F2}%.");

            return;
        }

        Debug.LogWarning(
            $"A distribuição populacional soma " +
            $"{percentualTotal:F2}%. " +
            $"A simulação permanecerá bloqueada " +
            $"até completar 100,00%.");
    }

    public bool TryGetTemplate(string tipoId, out TemplatePessoa template)
    {
        return colecaoTemplates.TryGetValue(tipoId, out template);
    }

    public void CriarPessoas(int total_pessoas)
    {
        if (colecaoTemplates == null || colecaoTemplates.Count == 0)
        {
            Debug.LogError(
                "Nenhum tipo de pessoa foi carregado.");
            return;
        }

        if (!PercentuaisFecham100(out float percentualTotal))
        {
            Debug.LogError(
                $"A distribuição populacional soma " +
                $"{percentualTotal:F2}%. É necessário completar 100%.");
            return;
        }

        Dictionary<string, int> quantidades = CalcularQuantidadeHibrida(total_pessoas);

        foreach (var item in quantidades)
        {
            string tipoId = item.Key;
            int quantidade = item.Value;

            TemplatePessoa template = colecaoTemplates[tipoId];
            //            int quantidade = Mathf.RoundToInt(total_pessoas * percentual);
            DebugController.Log(DebugCategoria.GestorPopulacao, $"popular ▸ {template.percentualPopulacao}%  gerando {quantidade} pessoas do tipo: {tipoId}");

            for (int i = 0; i < quantidade; i++)
            {
                int numeroPessoa = i;
                cPessoa novaPessoa = new cPessoa(tipoId, numeroPessoa);
                novaPessoa.Inicializar();
            }
        }

        if (colecaoTemplates == null)
        {
            Debug.LogError($"Tipos de pessoa não encontrado na coleção de templates.");
            return;
        }

        /*
        if (percentualPorTemplate == null)
        {
            Debug.LogError($"demografia nao definida");
            return;
        }
        */
    }
    public float CalcularPercentualTotal()
    {
        if (colecaoTemplates == null)
            return 0f;

        float total = 0f;

        foreach (TemplatePessoa template
            in colecaoTemplates.Values)
        {
            if (template == null)
                continue;

            total += template.percentualPopulacao;
        }

        return (float)System.Math.Round(total, 2);
    }

    public bool PercentuaisFecham100(
        out float percentualTotal)
    {
        percentualTotal = CalcularPercentualTotal();

        if (colecaoTemplates == null ||
            colecaoTemplates.Count == 0)
        {
            return false;
        }

        foreach (TemplatePessoa template
            in colecaoTemplates.Values)
        {
            if (template == null)
                return false;

            if (template.percentualPopulacao < 0f ||
                template.percentualPopulacao > 100f)
            {
                return false;
            }
        }

        const float tolerancia = 0.005f;

        return Mathf.Abs(percentualTotal - 100f) < tolerancia;
    }
    public Dictionary<string, int> CalcularQuantidadeHibrida(int totalAgentes)
    {
        Dictionary<string, int> quantidades = new Dictionary<string, int>();

        int totalBase = 0;

        // 1. Parte inteira
        foreach (var item in colecaoTemplates)
        {
            string tipoId = item.Key;
            TemplatePessoa template = item.Value;

            float percentual = template.percentualPopulacao/ 100f;

            int quantidadeBase = Mathf.FloorToInt(totalAgentes * percentual);

            quantidades[tipoId] = quantidadeBase;
            totalBase += quantidadeBase;
        }

        // 2. Calcula sobra
        int faltantes = totalAgentes - totalBase;

        // 3. Distribui sobra por sorteio ponderado
        for (int i = 0; i < faltantes; i++)
        {
            string tipoSorteado = SortearTipoPonderado();

            if (!quantidades.ContainsKey(tipoSorteado))
                quantidades[tipoSorteado] = 0;

            quantidades[tipoSorteado]++;
        }

        return quantidades;
    }
    private string SortearTipoPonderado()
    {
        float sorteio = Random.Range(0f, 100f);
        float acumulado = 0f;
        string ultimoTipoId = null;

        foreach (var item in colecaoTemplates)
        {
            string tipoId = item.Key;
            TemplatePessoa template = item.Value;

            ultimoTipoId = tipoId;
            acumulado += template.percentualPopulacao;

            if (sorteio < acumulado)
                return tipoId;
        }

        return ultimoTipoId;
    }


}

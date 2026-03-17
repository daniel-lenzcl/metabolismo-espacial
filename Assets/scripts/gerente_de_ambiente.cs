using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Gerente_de_ambiente : MonoBehaviour
{
    [SerializeField] private GerenteUI gerenteUI;
    public GerentePessoas gerentePessoas;
    public gestor_populacao gestor_Pop;
    //    public GerentePredios;
    public UnityEvent OnPrediosCarregados = new UnityEvent();

    public List<molde_pessoas> lista_tipos_pessoas = new List<molde_pessoas>();
    public painel_tipo_pessoa atualizacao_de_pessoas;

    public List<Predios> lista_dos_predios = new List<Predios>();
    public List<mPredios> mlista_dos_predios = new List<mPredios>();

    // Expondo mCamadas: nomeDaCamada -> lista de mPredios
    public Dictionary<string, List<mPredios>> mCamadas = new Dictionary<string, List<mPredios>>();

    public int Total_Pessoas = 0;
    public List<cPessoa> todas_as_pessoas;
    public List<CamadaInfo> todas_as_camadas;

    private void Awake()
    {
        gerenteUI.OnBotaoPopularClick += ChamaPopular;
    }

    void OnEnable()
    {
        painel_tipo_pessoa.OnPainelAtivado += TentarInscricao;
        DebugController.Log(DebugCategoria.GerenteAmbiente, "OnEnable ▸ inscrito no evento PainelAtivado");
    }

    void OnDisable()
    {
        painel_tipo_pessoa.OnPainelAtivado -= TentarInscricao;
        DebugController.Log(DebugCategoria.GerenteAmbiente, "OnDisable ▸ desinscrito do evento PainelAtivado");
    }

    void TentarInscricao()
    {
        DebugController.Log(DebugCategoria.GerenteAmbiente, "TentarInscricao ▸ procurando painel_tipo_pessoa");

        atualizacao_de_pessoas = FindObjectOfType<painel_tipo_pessoa>();
        if (atualizacao_de_pessoas == null)
        {
            DebugController.LogError(DebugCategoria.GerenteAmbiente, "painel_tipo_pessoa não encontrado na cena.");
            return;
        }

        // subscribe ao evento do painel 
        atualizacao_de_pessoas.OnAtualizaPessoas += Atualizar_lista;
        DebugController.Log(DebugCategoria.GerenteAmbiente, "Inscrição feita com sucesso!");
    }

    void Start()
    {
        lista_tipos_pessoas.Clear();
        lista_dos_predios.Clear();
        inicializar_pessoas();
        DebugController.Log(DebugCategoria.GerenteAmbiente, "Start ▸ listas limpas e pessoas iniciais criadas");
    }

    void Update() { }

    void ChamaPopular()
    {
        gestor_Pop = GetComponent<gestor_populacao>();
        gestor_Pop.popular();
        DebugController.Log(DebugCategoria.GerenteAmbiente, "ChamaPopular ▸ iniciando processo de popular pessoas");
//        Total_Pessoas = gerentePessoas.PopularPessoas(lista_tipos_pessoas, lista_dos_predios);
//        DebugController.Log(DebugCategoria.GerenteAmbiente, $"Total de pessoas populadas: {Total_Pessoas}");
    }
    private void Atualizar_lista(molde_pessoas pessoatemp)
    {
        DebugController.Log(DebugCategoria.GerenteAmbiente, $"Atualizando ambiente tipo: {pessoatemp.tipo_pessoa}");

        molde_pessoas pessoaExistente = lista_tipos_pessoas.FirstOrDefault(p => p.tipo_pessoa == pessoatemp.tipo_pessoa);

        if (pessoaExistente != null)
        {
            // Atualiza campos 
            pessoaExistente.atv_08_10 = pessoatemp.atv_08_10;
            pessoaExistente.atv_10_12 = pessoatemp.atv_10_12;
            pessoaExistente.atv_12_14 = pessoatemp.atv_12_14;
            pessoaExistente.atv_14_16 = pessoatemp.atv_14_16;
            pessoaExistente.atv_16_18 = pessoatemp.atv_16_18;
            pessoaExistente.atv_18_20 = pessoatemp.atv_18_20;
            pessoaExistente.atv_20_08 = pessoatemp.atv_20_08;

            DebugController.Log(DebugCategoria.GerenteAmbiente, $"Pessoa existente atualizada: {pessoatemp.tipo_pessoa}");
        }
        else
        {
            lista_tipos_pessoas.Add(pessoatemp);
            DebugController.Log(DebugCategoria.GerenteAmbiente, $"Nova pessoa adicionada: {pessoatemp.tipo_pessoa}");
        }

        atualizacao_de_pessoas.PreencheDropDownOpcoesPessoas();
    }

    void inicializar_pessoas()
    {
        gerentePessoas.InicializarTemplatesBase();

        var demografia = new Dictionary<string, int>
        {
            { "operario", 80 },
            { "cozinheiro", 20 }
        };
        gerentePessoas.DefinirQuantidadePorTemplate(demografia);

        molde_pessoas operario = new molde_pessoas("operario");
        operario.atv_08_10 = "trabalho";
        operario.atv_10_12 = "trabalho";
        operario.atv_12_14 = "restaurante";
        operario.atv_14_16 = "trabalho";
        operario.atv_16_18 = "trabalho";
        operario.atv_18_20 = "casa";
        operario.atv_20_08 = "casa";

        lista_tipos_pessoas.Add(operario);
        DebugController.Log(DebugCategoria.GerenteAmbiente, $"categorias de Pessoas iniciais: {lista_tipos_pessoas.Count}");
    }

    public void PrediosCarregados()
    {
        DebugController.Log(DebugCategoria.GerenteAmbiente, "Mapa carregado e lista de prédios preenchida.");
        OnPrediosCarregados.Invoke();
    }
}

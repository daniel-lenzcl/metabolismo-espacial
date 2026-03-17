using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class gestor_populacao : MonoBehaviour
{
    public InputField entrada_pessoas;
    public Gerente_de_ambiente Ambiente;
    public GerenteEncontros gerenteEncontros;

    public GameObject pfGente;
    private GameObject as_pessoas;

    public SalvarRedes salvador;

    [DllImport("__Internal")]
    private static extern void download_rede(string filename, string text);

    // ------------------- Unity ------------------- 
    void Start()
    {
        Ambiente = GetComponent<Gerente_de_ambiente>();
        gerenteEncontros = FindObjectOfType<GerenteEncontros>();

        DebugController.Log(DebugCategoria.GestorPopulacao, "Start ▸ referência ao Gerente_de_ambiente capturada");

        if (salvador == null)
            salvador = FindObjectOfType<SalvarRedes>();

    }

    void Update() { }

    // ------------------- UI ------------------- 
    public void popular()
    {
        ResetPopulacao();
        gerenteEncontros.DeletarMarcadores();
        as_pessoas = new GameObject("as_pessoas");

        int total = int.Parse(entrada_pessoas.text);
        Ambiente.Total_Pessoas = total;
        DebugController.Log(DebugCategoria.GestorPopulacao, $"popular ▸ gerando {total} pessoas");

        Ambiente.gerentePessoas.CriarPessoas(total); ///MIGRAR TUDO PARA GERENTE DE PESSOAS
        //        for (int i = 0; i < total; i++)
        //        {
        //            cPessoa nova = new cPessoa($"pessoa_{i}");
        //            nova.Inicializar();
        //            Ambiente.todas_as_pessoas.Add(nova);
        //        }
    }

    public void ResetPopulacao()
    {
        if (as_pessoas != null)
        {
            Destroy(as_pessoas);
            as_pessoas = null;
            gerenteEncontros.DeletarMarcadores();

            DebugController.Log(DebugCategoria.GestorPopulacao, "ResetPopulacao ▸ população destruída");
        }
    }

    public void SalvarRedeAtual(string nome_arquivo)
    {
        DebugController.Log(DebugCategoria.GestorPopulacao, "SalvarRedeAtual ▸ ");

        string caminho = @"C:\Users\danie\OneDrive\posdoc - ufc\atividades\metabolismo espacial\resultados das redes";
        var matriz = salvador.GerarMatrizSimplesDeCenas();

#if UNITY_EDITOR
        GetComponent<SalvarRedes>().SalvarRedeSimples(matriz, caminho);
        salvador.SalvarRedeSimples(matriz, caminho);
        salvador.SalvarMatrizBinaria(matriz, caminho);
        Debug.Log("SalvarRedeAtual -> arquivo salvo em: " + caminho);
#endif

        var matriz_download = salvador.GerarStringMatrizBinaria(matriz);
        Debug.Log("SalvarRedeAtual -> filename: " + nome_arquivo + ".csv");
        Debug.Log("SalvarRedeAtual -> csv length:" + matriz_download.Length);

        Debug.Log($"SalvarRedeAtual -> preview: " + matriz_download);

        // Apenas invoque o método nativo em build WebGL real (não no Editor)
#if UNITY_WEBGL && !UNITY_EDITOR
    try
    {
        download_rede(nome_arquivo + ".csv", matriz_download);
    }
    catch (System.EntryPointNotFoundException ex)
    {
        DebugController.LogWarning(DebugCategoria.GestorPopulacao, $"SalvarRedeAtual ▸ download_rede não encontrado no runtime: {ex.Message}");
    }
#else
        DebugController.LogWarning(DebugCategoria.GestorPopulacao, "SalvarRedeAtual ▸ download_rede disponível somente em build WebGL. Ignorando no Editor/Plataforma atual.");
#endif

    }
}

/*
public void PopularMapa() ///VINDO DO CARREGAR MAPA
{
    Debug.Log("total de casas: " + mCamadaCasa.Count);
    foreach (mPredios casa in mCamadaCasa)
    {
        //            Predios cada_child = new Predios(predio, child.name, centro_child, 3);
        //            lista.Add(cada_child);

        //sortear aleatoriamente entre mCamadaTrabalho e mCamadaRestaurante para colocar como trabalho
        mPredios trab_temp = mCamadaTrabalho[Random.Range(0, mCamadaTrabalho.Count)];
        //criar pessoa com cada como endereco de casa
        cPessoa nova_pessoa = new cPessoa("pessoa " + casa.nomePredio, levelgenerator_local.prefabP, casa, trab_temp);

        lista_das_pessoas.Add(nova_pessoa);
        Vector3 end_pessoa = new Vector3(nova_pessoa.MminhaCasa.enderecoXYZ.x, nova_pessoa.MminhaCasa.enderecoXYZ.y + 2, nova_pessoa.MminhaCasa.enderecoXYZ.z);

        Vector3 posInicial = end_pessoa; // posi��o pretendida
        UnityEngine.AI.NavMeshHit hit;

        if (UnityEngine.AI.NavMesh.SamplePosition(posInicial, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
        {
            GameObject agente = Instantiate(nova_pessoa.tipoPessoa);

            // Ajusta altura com base no NavMeshAgent ou CapsuleCollider
            float altura = 1.8f; // valor padr�o

            UnityEngine.AI.NavMeshAgent nav = agente.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (nav != null)
                altura = nav.height;
            else
            {
                CapsuleCollider cc = agente.GetComponent<CapsuleCollider>();
                if (cc != null)
                    altura = cc.height;
            }

            // Eleva a posi��o para que a base do agente fique no ch�o
            Vector3 posFinal = hit.position + new Vector3(0, altura / 2f, 0);
            agente.transform.position = posFinal;
            // Ativa movimenta��o
            //            nav?.SetDestination(alvo.transform.position);
            //                GameObject pessoa_temp = Instantiate(nova_pessoa.tipoPessoa, end_corrigido, Quaternion.identity);
            agente.GetComponent<pessoa>().euPessoa = nova_pessoa;
            Debug.Log("pessoa instanciada: " + nova_pessoa.identidade);

        }
        else
        {
            Debug.LogWarning("Nao encontrou ponto valido na NavMesh.");
        }



        //            GameObject pessoa_temp = Instantiate(nova_pessoa.tipoPessoa, end_corrigido, Quaternion.identity);
        //           pessoa_temp.GetComponent<pessoa>().euPessoa = nova_pessoa;
        //         Debug.Log("pessoa instanciada: " + nova_pessoa.identidade);

    }

    string nomedacasa;
    Predios tempcasa;
    //        Debug.Log("LEVEL-ATRIBUI PESSOAS: antes do foreach -> total de predios: " + predios.Count);// "endereco via predio:"+ tempcasa.enderecoXYZ);
    //       foreach (cPessoa tp in todasAsPessoas)
    //        {
    //           //        Find((x) => x.name == someString)
    //           nomedacasa = tp.minhaCasa.nomePredio;
    //           tempcasa = predios.Find((x) => x.nomePredio == nomedacasa);
    //           Instantiate(tp.tipoPessoa, tp.minhaCasa.enderecoXYZ, Quaternion.identity);
    //       }


}
*/

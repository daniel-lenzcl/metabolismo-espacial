using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class relacoes : MonoBehaviour
{
    public GameObject geral;
    public Text TXTtotalcasas;
    public Text TXTtotaltrabalhos;
    public Button gerarMapa;
    public InputField numeroCasas;

    public levelgenerator levelgenerator_local_relacoes;

    public string painelMenuId = "menuMapaAntigo";
    private UIReadyNotifier painelMenuNotifier;


    // Chamada para inscrever no Start (adicione no início do Start ou antes de usar os dropdowns)
    private void SubscribeToPainelMenu()
    {
        // procura notifiers na cena (inclui inativos). Filtra para objetos de cena.
        var all = Resources.FindObjectsOfTypeAll<UIReadyNotifier>();
        foreach (var n in all)
        {
            if (n == null || n.gameObject == null) continue;
            if (!n.gameObject.scene.isLoaded) continue; // descarta assets/prefabs
            if (n.id == painelMenuId)
            {
                painelMenuNotifier = n;
                break;
            }
        }
        if (painelMenuNotifier != null)
        {
            painelMenuNotifier.OnEnabled.AddListener(OnPainelMenuEnabled);

            // se o painel já estiver ativo, inicializa imediatamente
            if (painelMenuNotifier.gameObject.activeInHierarchy)
            {
                OnPainelMenuEnabled();
            }

            DebugController.Log(DebugCategoria.Mapas, $"SubscribeToPainelMenu ▸ inscrito em notifier '{painelMenuId}'");
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Mapas, $"SubscribeToPainelMenu ▸ UIReadyNotifier com id '{painelMenuId}' não encontrado na cena.");
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        geral = GameObject.Find("ambiente");// "Terrain");
        levelgenerator_local_relacoes = FindObjectOfType<levelgenerator>();

//        Debug.Log("RELACOES - START: total pessoas: " + geral.GetComponent<levelgenerator>().totalPessoas +
//                  "\n                total casas: " + geral.GetComponent<levelgenerator>().totalCasas +
//                  "\n                total trabalhos: " + geral.GetComponent<levelgenerator>().totalTrabalhos);

        //GetTotalPessoas(levelgenerator_local_relacoes.totalPessoas.ToString());
        // inscreve no notifier do painel (procura também notifiers inativos na cena)
        SubscribeToPainelMenu();


    }


    // chamado quando o painel for ativado (via UIReadyNotifier)
    private void OnPainelMenuEnabled()
    {
        DebugController.Log(DebugCategoria.Mapas, "OnPainelMenuEnabled ▸ painel ativado, inicializando getpessoas");
        GetTotalPessoas(levelgenerator_local_relacoes.totalPessoas.ToString());
    }

    // Handler com id recebido (caso o notifier dispare o id)
    private void OnPainelMenuEnabledById(string id)
    {
        if (id == painelMenuId)
        {
            OnPainelMenuEnabled();
        }
    }

    void OnDestroy()
    {
        if (painelMenuNotifier != null)
        {
            painelMenuNotifier.OnEnabled.RemoveListener(OnPainelMenuEnabled);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void bAtribuiRelacoes()
    {
        levelgenerator_local_relacoes.setaListas("predios");
        levelgenerator_local_relacoes.setaListas("enderecos");
        levelgenerator_local_relacoes.setaListas("pessoas");
        levelgenerator_local_relacoes.iniciaMapa();

        for (int i = 0; i < levelgenerator_local_relacoes.totalCasas; i++)
        {
///          FAZER A ADAPATACAO PARA *PREDIOS* COMO MONOBEHAVIOUR            
///          //Predios predio_temp 
            levelgenerator_local_relacoes.predios.Add(new Predios(levelgenerator_local_relacoes.casa, "casa" + i));
        }
//        for (int i = 0; i < totalEscolas; i++)
//        {
//            predios.Add(new Predios(escola, "escola" + i));
//        }
        for (int i = 0; i < levelgenerator_local_relacoes.totalTrabalhos; i++)
        {
            levelgenerator_local_relacoes.predios.Add(new Predios(levelgenerator_local_relacoes.trabalho, "trabalho" + i));
        }



        /////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////ISSO PRECISA ACONTECER DPS DE TER OS ENDERECOS DOS PREDIOS TODOS. REVER////////////////
        //////////////////////////////////////////////   OU MUDAR PARA A INSTANCIACAO PEGAR SO O NOME DO PREDIO E DAI REFERENCIAR

        List<Predios> asCasas = new List<Predios>();
        asCasas.AddRange(levelgenerator_local_relacoes.predios.FindAll((x) => x.nomePredio.Contains("casa")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
        List<Predios> osTrabalhos = new List<Predios>();
        osTrabalhos.AddRange(levelgenerator_local_relacoes.predios.FindAll((x) => x.nomePredio.Contains("trabalho")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
                                                                                                                                        //        Predios casat;
        int ttcasas = asCasas.Count;

        int pessoasNCasa = int.Parse(GameObject.Find("InputTotalCasa").GetComponent<InputField>().text);
        int indexCasa = 0;
        int contapessoasCasa = 0;

        int pessoasNtrab = int.Parse(GameObject.Find("InputTotalTrabalho").GetComponent<InputField>().text);
        int indexTrab = 0;
        int contapessoasTrab = 0;
        Debug.Log("RELACOES-B ATRIBUI RELACOES: pessoas na casa:" + pessoasNCasa + "pessoas no trab: "+ pessoasNtrab);
        for (int i = 0; i < levelgenerator_local_relacoes.totalPessoas; i++)
        {
            if (contapessoasCasa < pessoasNCasa)
            {
                //                index ++;
                contapessoasCasa++;
            } else
            {
                indexCasa++;
                contapessoasCasa = 1;
            }

            if (contapessoasTrab < pessoasNtrab)
            {
                //                index ++;
                contapessoasTrab++;
            }
            else
            {
                indexTrab++;
                contapessoasTrab = 1;
            }


            //            casat = asCasas[(int)Mathf.PingPong(i, asCasas.Count - 1)];
            //            casat = asCasas[(int)Mathf.Floor(i*fracao)];
//            Debug.Log("RELACOES-B ATRIBUI RELACOES: nome das casas: " + indexCasa + "contapessoas casa: " + contapessoasCasa +
//                "\n                 nome dos trab: " + indexTrab + " conta pessoas trab: " + contapessoasTrab);
//            Debug.Log("RELACOES-B ATRIBUI RELACOES: nome das casas: " + asCasas[indexCasa].nomePredio + "endereco casa: " + asCasas[indexCasa].enderecoXYZ);

            // CORREÇÃO: cPessoa não possui mais construtor com 4 args.
            // Criar instância com (prefab, nome) e atribuir campos depois.
            var novaPessoa = new cPessoa(levelgenerator_local_relacoes.prefabP, "pessoa" + i);
            novaPessoa.minhaCasa = asCasas[indexCasa];
            novaPessoa.meuTrabalho = osTrabalhos[indexTrab];
            // opcional: inicializar agora (assinar eventos, etc.)
            novaPessoa.Inicializar();

            levelgenerator_local_relacoes.todasAsPessoas.Add(novaPessoa);
//            index++;
        }
        Debug.Log("RELACOES-B ATRIBUI RELACOES: contagem todasaspessoas: " + levelgenerator_local_relacoes.todasAsPessoas.Count + "total de predios: "+ levelgenerator_local_relacoes.predios.Count);


        gerarMapa.interactable = true;
    }

    public void GetTotalPessoas(string tTPessoa)   /////pessoas vai ser recuperado na classe casa
    {
        gerarMapa.interactable = false;

        levelgenerator_local_relacoes.totalPessoas = int.Parse(tTPessoa);
        GetCasas(GameObject.Find("InputTotalCasa").GetComponent<InputField>().text);
        GetTrabalhos(GameObject.Find("InputTotalTrabalho").GetComponent<InputField>().text);

        //        Debug.Log("RELACOES - GET TOTAL PESSOAS: pessoas: " + tTPessoa);

    }


    public void GetCasas(string tCasa)
    {
        gerarMapa.interactable = false;

        //        Debug.Log("RELACOES - GET TOTAL CASAS: total casas (antes): " + geral.GetComponent<levelgenerator>().totalCasas);
        float pessoasPcasa = float.Parse(tCasa);
        float t =  Mathf.Ceil((float)levelgenerator_local_relacoes.totalPessoas / pessoasPcasa);
        
        levelgenerator_local_relacoes.totalCasas = (int)t;
        TXTtotalcasas.text = levelgenerator_local_relacoes.totalCasas.ToString();
  
//        Debug.Log("RELACOES - GET TOTAL CASAS: total quebrado: " + t +  
//                "\nRELACOES - GET TOTAL CASAS: total casas(depois): " + geral.GetComponent<levelgenerator>().totalCasas);
    }

    public void GetEscolas(string tEscola)
    {
        levelgenerator_local_relacoes.totalEscolas = int.Parse(tEscola);
    }

    public void GetTrabalhos(string tTrabalho)
    {
        gerarMapa.interactable = false;

        //        Debug.Log("RELACOES - GET TOTAL TRABALHOS: total trabalhos(antes): " + geral.GetComponent<levelgenerator>().totalTrabalhos);
        float pessoasPtrabalho = float.Parse(tTrabalho);
        levelgenerator_local_relacoes.totalTrabalhos = (int)Mathf.Ceil((float)levelgenerator_local_relacoes.totalPessoas / pessoasPtrabalho);
        //        geral.GetComponent<levelgenerator>().totalCasas = int.Parse(tCasa);
        //        GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
        TXTtotaltrabalhos.text = levelgenerator_local_relacoes.totalTrabalhos.ToString();

//        Debug.Log("RELACOES - GET TOTAL TRABALHOS: pessoas por trabalho: " + pessoasPtrabalho+
//                 "\nRELACOES - GET TOTAL TRABALHOS: total trabalhos(depois): " + geral.GetComponent<levelgenerator>().totalTrabalhos);
    }

    public void GetPessoas(string tPessoa)   /////pessoas vai ser recuperado na classe casa
    {
        levelgenerator_local_relacoes.maxPessoasI = int.Parse(tPessoa);
    }


}

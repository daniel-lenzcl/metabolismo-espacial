using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Mapas : MonoBehaviour
{

    public string tipoMapa;
    public string tipoDist;
    public GameObject geral;
    public levelgenerator levelgenerator_local_mapas;

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
        //        Debug.Log("MAPAS - START: comeco");
        geral = GameObject.Find("ambiente");// "Terrain");
        levelgenerator_local_mapas = FindObjectOfType<levelgenerator>();

        // inscreve no notifier do painel (procura também notifiers inativos na cena)
        SubscribeToPainelMenu();

        // fallback: se não existir notifier, tenta inicializar imediatamente caso os dropdowns já estejam ativos
        if (painelMenuNotifier == null)
        {
            var goDistrib = GameObject.Find("DdDISTRIBUICAO");
            if (goDistrib != null && goDistrib.activeInHierarchy)
                InicializarDropdownDistribuicao();

            var goMapa = GameObject.Find("DdMAPA");
            if (goMapa != null && goMapa.activeInHierarchy)
                InicializarDropdownMapa();
        }

        /*
        ///////////////////////////////////////////////////INICIALIZAR DROPDOWN DE TIPOS DE DISTRIBUICAO/////////////////////////////
        List<string> tiposDistribuicoes = new List<string>()
        {
            "continuo",
            "alternado", ///precisara ser implementado qd se colocar um 3o destino
            "aleatorio",
            "importar mapa",
        };
        var drpDistribuicao = GameObject.Find("DdDISTRIBUICAO").GetComponent<Dropdown>();
        drpDistribuicao.options.Clear();
        foreach (var tipo in tiposDistribuicoes)
        {
            drpDistribuicao.options.Add(new Dropdown.OptionData() { text = tipo });
            //            Debug.Log("MAPAS - START: opcoes das distribuicoes");
        }
        //geral.GetComponent<levelgenerator>()
            levelgenerator_local_mapas.tipoDistribuicao = drpDistribuicao.options[0].text;
//        Debug.Log("MAPAS START: distribuicao: " + geral.GetComponent<levelgenerator>().tipoDistribuicao);//tiposDistribuicoes[0]);
        DebugController.Log(DebugCategoria.Mapas, $"START ▸ distribuicao: '{levelgenerator_local_mapas.tipoDistribuicao}'");


        ///////////////////////////////////////////////////FIM INICIALIZAR DROPDOWN DE TIPOS DE DISTRIBUICAO/////////////////////////////

        ///////////////////////////////////////////////////INICIALIZAR DROPDOWN DE TIPOS DE MAPA/////////////////////////////
        //        List<string> tiposMapas = new List<string>() 
        //        {
        //        };
        List<string> tiposMapas = new List<string>()
        {
            "aleatorio", 
            "matriz",
            "cruz",
            "linha",
            "circulo"
        };

        var drpMapa = GameObject.Find("DdMAPA").GetComponent<Dropdown>();
        drpMapa.options.Clear();
        foreach (var tipo in tiposMapas)
        {
            drpMapa.options.Add(new Dropdown.OptionData() { text = tipo });
//            Debug.Log("MAPAS - START: opcoes dos tipos");
        }
        drpMapa.value = -1;
        tipoMapa = drpMapa.options[drpMapa.value].text;
//        tipoMapa = tiposMapas[0];
//        Debug.Log("MAPAS START: tipo de mapa: " + tipoMapa);
        DebugController.Log(DebugCategoria.Mapas, $"START ▸ tipo de mapa: '{tipoMapa}'");

        //        drpMapa.Label.text = "aleatorio";
        //        tipoMapa = dropMapa.options[dropMapa.value].text;

        ///////////////////////////////////////////////////FIM INICIALIZAR DROPDOWN DE TIPOS DE MAPA/////////////////////////////


        //        Debug.Log("MAPAS - START: final");
        */
    }

    void OnDestroy()
    {
        if (painelMenuNotifier != null)
        {
            painelMenuNotifier.OnEnabled.RemoveListener(OnPainelMenuEnabled);
        }
    }

    // chamado quando o painel for ativado (via UIReadyNotifier)
    private void OnPainelMenuEnabled()
    {
        DebugController.Log(DebugCategoria.Mapas, "OnPainelMenuEnabled ▸ painel ativado, inicializando dropdowns");
        InicializarDropdownDistribuicao();
        InicializarDropdownMapa();
    }


    // Handler com id recebido (caso o notifier dispare o id)
    private void OnPainelMenuEnabledById(string id)
    {
        if (id == painelMenuId)
        {
            OnPainelMenuEnabled();
        }
    }

    private void InicializarDropdownDistribuicao()
    {
        var drpDistribuicaoGO = GameObject.Find("DdDISTRIBUICAO");
        var drpDistribuicao = drpDistribuicaoGO?.GetComponent<Dropdown>();
        if (drpDistribuicao == null)
        {
            DebugController.LogError(DebugCategoria.Mapas, "InicializarDropdownDistribuicao ▸ 'DdDISTRIBUICAO' não encontrado");
            return;
        }

        var tiposDistribuicoes = new List<string>()
        {
            "continuo",
            "alternado",
            "aleatorio",
            "importar mapa",
        };

        drpDistribuicao.options.Clear();
        foreach (var tipo in tiposDistribuicoes)
            drpDistribuicao.options.Add(new Dropdown.OptionData() { text = tipo });

        drpDistribuicao.value = 0;
        drpDistribuicao.RefreshShownValue();

        if (levelgenerator_local_mapas != null)
        {
            levelgenerator_local_mapas.tipoDistribuicao = drpDistribuicao.options[0].text;
            DebugController.Log(DebugCategoria.Mapas, $"InicializarDropdownDistribuicao ▸ distribuicao: '{levelgenerator_local_mapas.tipoDistribuicao}'");
        }
    }

    private void InicializarDropdownMapa()
    {
        var drpMapaGO = GameObject.Find("DdMAPA");
        var drpMapa = drpMapaGO?.GetComponent<Dropdown>();
        if (drpMapa == null)
        {
            DebugController.LogError(DebugCategoria.Mapas, "InicializarDropdownMapa ▸ 'DdMAPA' não encontrado");
            return;
        }

        var tiposMapas = new List<string>()
        {
            "aleatorio",
            "matriz",
            "cruz",
            "linha",
            "circulo"
        };

        drpMapa.options.Clear();
        foreach (var tipo in tiposMapas)
            drpMapa.options.Add(new Dropdown.OptionData() { text = tipo });

        drpMapa.value = 0;
        drpMapa.RefreshShownValue();

        tipoMapa = drpMapa.options[drpMapa.value].text;
        DebugController.Log(DebugCategoria.Mapas, $"InicializarDropdownMapa ▸ tipo de mapa: '{tipoMapa}'");
    }


    public void selecionaMapa(Dropdown dropMapa)
    {
        //        GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
        tipoMapa = dropMapa.options[dropMapa.value].text;
        //        Debug.Log("MAPAS-SELECIONA TIPO MAPA: o tipo eh:" + tipoMapa);
        DebugController.Log(DebugCategoria.Mapas, $"selecionaMapa ▸ o tipo eh: '{tipoMapa}'");

    }

    public void selecionaDistribuicao(Dropdown dropDistribuicao)
    {
        levelgenerator_local_mapas.tipoDistribuicao = dropDistribuicao.options[dropDistribuicao.value].text;
        //string td
        //        Debug.Log("MAPAS-SELECIONA DISTRIBUICAO ATIVIDADES: distribuicao eh:" +geral.GetComponent<levelgenerator>().tipoDistribuicao);
        DebugController.Log(DebugCategoria.Mapas, $"selecionaDistribuicao ▸distribuicao eh: '{levelgenerator_local_mapas.tipoDistribuicao}'");

    }

    public void botaoMapa()
    {
        //        Debug.Log("MAPAS - BOTAO MAPA ->comeco: contagem de predios: " + geral.GetComponent<levelgenerator>().predios.Count);
        levelgenerator_local_mapas.iniciaMapa();
        levelgenerator_local_mapas.setaListas("enderecos");
        //        GameObject.Find("Terrain").GetComponent<levelgenerator>().mapaCruz();
        //        geral.GetComponent<levelgenerator>().atribuiPessoas();
        //        "aleatorio", 
        //            "matriz",
        //            "cruz",
        //            "linha",
        //            "circulo"

        switch (tipoMapa)
        {
            case "aleatorio":
                //                Debug.Log("MAPAS - BOTAOMAPA: aleatorio");
                levelgenerator_local_mapas.mapaAleatorio();
                //                geral.levelgenerator.mapaAleatorio();
                //                mapaAleatorio();
                break;
            case "matriz":
                //                Debug.Log("MAPAS - BOTAOMAPA: matriz");
                levelgenerator_local_mapas.mapaMatriz();
                //                mapaMatriz();
                break;
            case "cruz":
                //                Debug.Log("MAPAS - BOTAOMAPA: cruz");
                levelgenerator_local_mapas.mapaCruz();

                //              mapaCruz();
                break;
            case "linha":
                //                Debug.Log("MAPAS - BOTAOMAPA: linha");
                levelgenerator_local_mapas.mapaLinha();

                //            mapaLinha();
                break;
            case "circulo":
                //                Debug.Log("MAPAS - BOTAOMAPA: circulo");
                levelgenerator_local_mapas.mapaCirculo();
                //          mapaCirculo();
                break;
        }
        //        Debug.Log("MAPAS - BOTAO MAPA ->final: contagem de predios: " + geral.GetComponent<levelgenerator>().predios.Count);
        //        geral.GetComponent<levelgenerator>().colocaPessoas();
        GameObject.Find("Terrain").GetComponent<horas>().Start();


    }



}

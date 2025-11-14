using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class botoes : MonoBehaviour
{
    public Text BotaoDia;
    public bool comecaDia = false;

    public SalvarRedes salvador;  // atribuir via Inspector ou buscar via script

    private horas horas_computando;

    private gestor_populacao Gestor_populacao_local;
    private bool salvaRede = false;
    private Text textoBotaoSalvarRede;
    private TMP_InputField inputNomeRede;
    private Button BotaoSalvarRede;
    private gerente_paineis gerente_paineis_local;
    // ------------------------ Unity Methods ------------------------ 
    void Start()
    {
        DebugController.Log(DebugCategoria.Botoes, "botoes ▸ Start chamado");

        // inicio configurando botao de redes
        Gestor_populacao_local = FindObjectOfType<gestor_populacao>();

        GameObject go0 = GameObject.Find("bListaRedeNovo");
        if (go0 != null)
        {
            BotaoSalvarRede = go0.GetComponent<Button>();
            DebugController.Log(DebugCategoria.Botoes, $"BotaoSalvarRede encontrado: {BotaoSalvarRede.name}");
            //            if (BotaoSalvarRede != null)
            //            {
            //                BotaoSalvarRede.onClick.RemoveAllListeners();
            //                BotaoSalvarRede.onClick.AddListener(botaoListaRede);
            //            }

            textoBotaoSalvarRede = BotaoSalvarRede.GetComponentInChildren<Text>();
            if (textoBotaoSalvarRede != null)
            {
                textoBotaoSalvarRede.text = "listar rede";
                DebugController.Log(DebugCategoria.Botoes, $"textoBotaoSalvarRede encontrado: {textoBotaoSalvarRede.text}");

            }

            Transform go1 = BotaoSalvarRede.transform.Find("InputField_nome_rede");
            if (go1 != null)
            {
                DebugController.Log(DebugCategoria.Botoes, $"go1 encontrado: {go1.name}");
            }

            inputNomeRede = go1.GetComponentInChildren<TMP_InputField>();   //BotaoSalvarRede.transform.Find("InputField_nome_rede");
            if (inputNomeRede != null)
            {
                DebugController.Log(DebugCategoria.Botoes, $"inputNomeRede encontrado: {inputNomeRede.name}");
            }

            gerente_paineis_local = BotaoSalvarRede.GetComponent<gerente_paineis>();
            if (gerente_paineis_local != null)
            {
                DebugController.Log(DebugCategoria.Botoes, $"gerente_paineis_local encontrado: {gerente_paineis_local.botoes}");
            }
        }

        /*
        GameObject go1 = GameObject.Find("TextListaRede");
        if (go1 != null)
        {
            textoBotaoSalvarRede = go1.GetComponent<Text>();
            if (textoBotaoSalvarRede != null)
                textoBotaoSalvarRede.text = "listar rede";

        }

        GameObject go2 = GameObject.Find("InputField_nome_rede");
        if (go2 != null)
        {
            inputNomeRede = go2.GetComponent<TMP_InputField>();
        }
        */

        // fim configurando botao de redes
    }

        void Update() { }

    // ------------------------ UI Callbacks ------------------------ 
    public void botaDia()
    {
        horas_computando = FindObjectOfType<horas>();
        if (horas_computando == null)
        {
            DebugController.LogError(DebugCategoria.Botoes, "botaDia ▸ componente 'horas_computando' não encontrado em Terrain");
            return;
        }

        horas_computando.rodadia = !horas_computando.rodadia;
        BotaoDia.text = horas_computando.rodadia ? "dia rolando" : "começar dia";

        DebugController.Log(DebugCategoria.Botoes, $"botaDia ▸ rodadia agora = {horas_computando.rodadia}");
    }

    public void botaoSair()
    {
        DebugController.Log(DebugCategoria.Botoes, "botaoSair ▸ Quit");
        Application.Quit();
    }

    // ------------------------ Rede de contatos ------------------------ 
    public void botaoResetRede()
    {
        GameObject[] individuos = GameObject.FindGameObjectsWithTag("pessoas");
        DebugController.Log(DebugCategoria.Botoes, $"botaoResetRede ▸ total pessoas = {individuos.Length}");

        foreach (GameObject p in individuos)
        {
            var con = p.GetComponent<conectados>();
            if (con == null) continue;

            con.encontro.Clear();
            con.quemEncontrei.Clear();
            con.esbarrei = false;
            var col = p.GetComponent<Collider>();
            if (col)
            {
                col.enabled = false;
                col.enabled = true;
            }
        }

        //GameObject.Find("Terrain").GetComponent<populacao>().matrizEncontros();
        GameObject.Find("ambiente").GetComponent<populacao>().matrizEncontros();
        horas_computando.Start();//GameObject.Find("Terrain").GetComponent<horas>().Start();

    }

    public void botaoListaRede()
    {
        if (salvaRede)
        {
            DebugController.Log(DebugCategoria.Botoes, $"botaoListaRede ▸ salvando rede");
            /*
            GameObject[] individuos = GameObject.FindGameObjectsWithTag("pessoas");
            DebugController.Log(DebugCategoria.Botoes, $"botaoListaRede ▸ total pessoas = {individuos.Length}");

            if (individuos == null || individuos.Length == 0)
            {
                DebugController.Log(DebugCategoria.Botoes, "botaoListaRede ▸ nenhuma pessoa encontrada");
                return;
            }

            GameObject.Find("Terrain").GetComponent<populacao>().matrizEncontros();
            GameObject.Find("Terrain").GetComponent<populacao>().salvaMatriz();
            */

            string nomeRede = inputNomeRede.text;
            Debug.Log("nome da rede a salvar: " + nomeRede);
            Gestor_populacao_local.SalvarRedeAtual(nomeRede);
            salvaRede = false;
            textoBotaoSalvarRede.text = "listar rede";
            gerente_paineis_local.habilitaPainel();
            return;
        }
        if (!salvaRede)
        {
            DebugController.Log(DebugCategoria.Botoes, $"botaoNomeRede ▸ pedindo nome da rede");
            //            salvador.PedeNomeRede();
            salvaRede = true;
            textoBotaoSalvarRede.text = "Salvar";
            gerente_paineis_local.habilitaPainel();
            return;
        }
    }

}

    /*
    public void botaoMapaAleatorio()
    {
        GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
        GameObject.Find("Terrain").GetComponent<levelgenerator>().mapaAleatorio();
        GameObject.Find("Terrain").GetComponent<levelgenerator>().colocaPessoas();
    }
*/

    /*
        public void botaoMapaMatriz()
        {
            GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().mapaMatriz();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().colocaPessoas();

        }
    */

    /*
        public void botaoMapaLinha()
        {
            GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().mapaLinha();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().colocaPessoas();
        }
    */

    /*
        public void botaoMapaCirculo()
        {
            GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().mapaCirculo();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().colocaPessoas();
        }
    */

    /*
        public void botaoMapaCruz()
        {
            GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().mapaCruz();
            GameObject.Find("Terrain").GetComponent<levelgenerator>().colocaPessoas();
        }
    */


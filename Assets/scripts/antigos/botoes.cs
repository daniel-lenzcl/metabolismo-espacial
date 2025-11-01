using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class botoes : MonoBehaviour
{
    public Text BotaoDia;
    public bool comecaDia = false;

    public SalvarRedes salvador;  // atribuir via Inspector ou buscar via script

    private horas horas_computando;
    // ------------------------ Unity Methods ------------------------ 
    void Start()
    {
        DebugController.Log(DebugCategoria.Botoes, "botoes ▸ Start chamado");
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

        gestor_populacao gestor = FindObjectOfType<gestor_populacao>();
        gestor.SalvarRedeAtual();
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


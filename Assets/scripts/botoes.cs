using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class botoes : MonoBehaviour
{
    public Text BotaoDia;
    public bool comecaDia = false;
//    public GameObject[] individuos;


    // Start is called before the first frame update
    void Start()
    {
//        comecaDia = GameObject.Find("Terrain").GetComponent<horas>().rodadia;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void botaDia()
    {
        //    comecaDia = !comecaDia;
        //    GameObject.Find("Terrain").GetComponent<horas>().rodadia = comecaDia;
        //    if (comecaDia)
        GameObject.Find("Terrain").GetComponent<horas>().rodadia = !GameObject.Find("Terrain").GetComponent<horas>().rodadia;
        if (GameObject.Find("Terrain").GetComponent<horas>().rodadia)
        {
            BotaoDia.text = "dia rolando";
        } else
        {
            BotaoDia.text = "começar dia";
        }
    }

    public void botaoSair()
    {
        Application.Quit();
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
    /// <summary>
    /// /////////////////////////////////////dar um jeito de zerar a rede de contatos. provavelmente seja mais interessante criar uma matriz geral q faca a atualizacao
    ///                          tem q revisar na classe CONECTADOS como a coisa ta sendo feita. 
    /// </summary>
    public void botaoResetRede()
    {
        GameObject[] individuos = GameObject.FindGameObjectsWithTag("pessoas");

//        GameObject[] individuos = GameObject.FindAll("pessoa");
        Debug.Log("BOTOES-BOTAO reset REDE: total de pessoas: " + individuos.Length);
        foreach (GameObject p in individuos)
        {
//            Debug.Log("BOTOES-BOTAO reset REDE: total de pessoas(comeco): " + p.GetComponent<conectados>().quemEncontrei.Count);
            p.GetComponent<conectados>().encontro.Clear();
            p.GetComponent<conectados>().quemEncontrei.Clear();
            p.GetComponent<conectados>().esbarrei = false;
            p.GetComponent<Collider>().enabled = false;
            p.GetComponent<Collider>().enabled = true;
  //          Debug.Log("BOTOES-BOTAO reset REDE: total de pessoas(fim): " + p.GetComponent<conectados>().quemEncontrei.Count);
        }
        GameObject.Find("Terrain").GetComponent<populacao>().matrizEncontros();
        GameObject.Find("Terrain").GetComponent<horas>().Start();
    }

    public void botaoListaRede()
    {
        //------------COMO PEGAR OS CONTATOS TODOS? ONDE Q FICA ISSO? COMO LISTA NA CLASSE "CONECTATOS, CONTATOS"? OU PEGO PELOS PREFABS?
        GameObject[] individuos = GameObject.FindGameObjectsWithTag("pessoas");
        Debug.Log("BOTOES-BOTAO LISTA REDE: total de pessoas: " + individuos.Length);

        if (individuos == null)
        {
            Debug.Log("BOTOES-BOTAO LISTA REDE: nenhuma pessoa");
            return;
        }

//        foreach (GameObject i in individuos)
//        {
//            i.GetComponent<conectados>().dizEncontros();
//        }


        GameObject.Find("Terrain").GetComponent<populacao>().matrizEncontros();
        GameObject.Find("Terrain").GetComponent<populacao>().salvaMatriz();


    }
}

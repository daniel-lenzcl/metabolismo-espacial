using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class crianca : MonoBehaviour
{
    public cPessoa euMesmo;


    // Start is called before the first frame update
    void Start()
    {
        GameObject aBase = GameObject.Find("Terrain");
        euMesmo = aBase.GetComponent<levelgenerator>().tempPessoas[0];
        Debug.Log("CRIANCA-START: eu crianca" );
//        Debug.Log("eu crianca" + this.transform.position);
        //            tempPessoa[0];
        GetComponent<levelgenerator>().tempPessoas.RemoveAt(0);
        /*
        GameObject geral = GameObject.Find("Terrain");

        //   contatos = new List<GameObject>();
        identidadepessoa = geral.GetComponent<levelgenerator>().contador;
        geral.GetComponent<levelgenerator>().contador++;
        this.name = "pessoa" + identidadepessoa.ToString();


        //        identidadepessoa = geral.nomepessoa;
        Debug.Log("PESSOA: nome da pessoa: " + identidadepessoa);


        jogador = this.GetComponent<NavMeshAgent>();
        GameObject[] trabalhos = GameObject.FindGameObjectsWithTag("trabalho");

        //        tipoDaPessoa = tiposPessoa[Random.Range(0, tiposPessoa.Length)];
        tipoDaPessoa = "jovem";


        casa = this.transform.position;
        int num_trab = Random.Range(0, trabalhos.Length);
        trabalho = trabalhos[num_trab].transform.position;

        destino = casa;
        jogador.SetDestination(destino);
        jogador.stoppingDistance = 2f;

        //        Debug.Log("casa: " + casa + "    trabalho (" + num_trab + "): " + trabalho);

        //    lRede = GetComponent<LineRenderer>();
        */
    }


    // Update is called once per frame
    void Update()
    {
        /*
        bool rodadia = GameObject.Find("Terrain").GetComponent<horas>().rodadia;
        if (rodadia)
        {

            tempo = GameObject.Find("Terrain").GetComponent<horas>().hora;
            //   Debug.Log("horas: " + tempo);

            string t = "jovem";////////////////////////---------------argumento para uso de CASE variavel

            switch (tipoDaPessoa)
            {
                case string a when a.Contains(t):

                    switch (tempo)
                    {
                        //private string[] dest = { "jardins", "alameda", "fonteLinear", "fonteCircular", "fonteFemininas", "coreto", "plataforma" };
                        case 1:
                            destino = trabalho;
                            sdest = "trabalho";
                            break;
                        case 2:
                            destino = casa;
                            sdest = "casa";
                            break;
                        case 8:
                            destino = trabalho;
                            //registrando, como acessar via nome das paradas
                            //
                            //destino = GameObject.Find("fonteLinear").transform.position;
                            sdest = "trabalho";
                            break;
                        case 15:
                            destino = casa;
                            sdest = "casa";
                            break;
                    }

                    break;

            }
            
                        switch (tipoDaPessoa)
                        {
                            case "jovem":

                                int[] rotina = { 2, 6, 8, 15 };

                                if (tempo == rotina[0])
                                {
                                    switch (tempo)
                                    {
                                        //private string[] dest = { "jardins", "alameda", "fonteLinear", "fonteCircular", "fonteFemininas", "coreto", "plataforma" };
                                        case 1:
                                            destino = trabalho;
                                            sdest = "trabalho";
                                            break;
                                        case 2:
                                            destino = casa;
                                            sdest = "casa";
                                            break;
                                        case 8:
                                            destino = trabalho;
                                            //registrando, como acessar via nome das paradas
                                            //
                                            //destino = GameObject.Find("fonteLinear").transform.position;
                                            sdest = "trabalho";
                                            break;
                                        case 15:
                                            destino = casa;
                                            sdest = "casa";
                                            break;
                                    }
                                }

                                break;

                        }
            
   //         Debug.Log("pessoa " + identidadepessoa + " indo para " + sdest);
            //--------acrescentar mudanca de destino aki para atualizacao.

 //           jogador.SetDestination(destino);   //DESCOBRIR PQ O TERRENO E A NAVMESH TA SE MEXENDO JUNTO COM O JOGADOR -  terreno estava com funcao de agente tb (o q significa funcao de agente tb?)

            

        }
        */
    }

}

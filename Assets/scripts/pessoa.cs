using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class pessoa : MonoBehaviour
{
    //public GameObject linha;
    //    private int id;
    //    private List<int> encontros;
    private List<string> nomePredios; 


    private Vector3 casa;
    private Vector3 trabalho;
    private Vector3 escola;
    private NavMeshAgent jogador;
    public int velocidade;
    private Vector3 destino;
    private string dest;
    public List<GameObject> contatos = new List<GameObject>();
    private float tempo;
    //    public List<LineRenderer> linhaContato = new List<LineRenderer>();

    //preparacao para selecao de destino por case e switch. pegar na CASA onde gera pessoa ou crianca dps, para ver o tipo de pessoa gerada.
    private string[] tiposPessoa = { "jovem" };
    private string tipoDaPessoa;
    //private LineRenderer lRede = new LineRenderer();


    //private string nome;
    //    public pessoa (string nom)
    //    {
    //        this.nome = nom;
    //    }
    public int identidadepessoa;
//    private string sdest;
    public cPessoa euPessoa;
    private GameObject geral;

    // Start is called before the first frame update
    void Start()
    {
        geral = GameObject.Find("Terrain");

        //   contatos = new List<GameObject>();
        identidadepessoa = geral.GetComponent<levelgenerator>().contador;
        geral.GetComponent<levelgenerator>().contador++;
        euPessoa = geral.GetComponent<levelgenerator>().todasAsPessoas[identidadepessoa];//////////////////////////////////
        this.name = euPessoa.identidade;

        //        identidadepessoa = geral.nomepessoa;
//        Debug.Log("PESSOA-START: nome da pessoa: " + this.name);


        jogador = this.GetComponent<NavMeshAgent>();
        jogador.speed = 5 * geral.GetComponent<levelgenerator>().todasAsPessoas.Count;
//        GameObject[] trabalhos = GameObject.FindGameObjectsWithTag("trabalho");

        //        tipoDaPessoa = tiposPessoa[Random.Range(0, tiposPessoa.Length)];
        tipoDaPessoa = "jovem"; //APESAR DE EXISTIR, NAO ESTA SENDO COLOCADO EM USO

        casa = euPessoa.minhaCasa.enderecoXYZ;
        trabalho = euPessoa.meuTrabalho.enderecoXYZ;
//        casa = this.transform.position;
//        int num_trab = Random.Range(0, trabalhos.Length);
//        trabalho = trabalhos[num_trab].transform.position;

        destino = casa;
        jogador.SetDestination(destino);
        jogador.stoppingDistance = 2f;

        //        Debug.Log("casa: " + casa + "    trabalho (" + num_trab + "): " + trabalho);

        //    lRede = GetComponent<LineRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
//        bool rodadia = GameObject.Find("Terrain").GetComponent<horas>().rodadia;
        bool rodadia = geral.GetComponent<horas>().rodadia;
        if (rodadia)
        {

//            tempo = GameObject.Find("Terrain").GetComponent<horas>().hora;
            tempo = geral.GetComponent<horas>().hora;
            //   Debug.Log("horas: " + tempo);
            ////////////////////////////////////estrutura de destino poderia ser uma lista de destinos da classe. dai faria 'if' pra ver se deu a hora vigente, incrementa, e passa
            ///////////////////////////////////para o proximo da lista. mas nao implementado ainda, pensar a respeito para outra versao (pos testes)

            string t = "jovem";////////////////////////---------------argumento para uso de CASE variavel

            switch (tipoDaPessoa)
            {
                case string a when a.Contains(t):

                    switch (tempo)
                        {
                            //private string[] dest = { "jardins", "alameda", "fonteLinear", "fonteCircular", "fonteFemininas", "coreto", "plataforma" };
                            case 2:
                                destino = trabalho;
//                                sdest = "trabalho";
                                break;
//                            case 2:
//                                destino = casa;
//                                sdest = "casa";
//                                break;
//                            case 8:
//                                destino = trabalho;
                                //registrando, como acessar via nome das paradas
                                //
                                //destino = GameObject.Find("fonteLinear").transform.position;
//                                sdest = "trabalho";
//                                break;
                            case 18:
                                destino = casa;
//                                sdest = "casa";
                                break;
                        }

                    break;

            }
/*
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
*/
//            Debug.Log("PESSOA-UPDATE: pessoa " + this.name + " indo para " + sdest);
            //--------acrescentar mudanca de destino aki para atualizacao.

            jogador.SetDestination(destino);   //DESCOBRIR PQ O TERRENO E A NAVMESH TA SE MEXENDO JUNTO COM O JOGADOR -  terreno estava com funcao de agente tb (o q significa funcao de agente tb?)

 
            
        }
    }
}

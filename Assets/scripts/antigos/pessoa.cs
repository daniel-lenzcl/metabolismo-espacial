//fora de uso, so para referencia.
//fora de uso, so para referencia.
//fora de uso, so para referencia.
//fora de uso, so para referencia.
//fora de uso, so para referencia.
//fora de uso, so para referencia.

using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

//using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;
//using NavMeshModifier = Unity.AI.Navigation.NavMeshModifier;


public class pessoa : MonoBehaviour
{
    //public GameObject linha;
    //    private int id;
    //    private List<int> encontros;
    private List<string> nomePredios;


    private Vector3 casa;
    private Vector3 trabalho;
    private Vector3 escola;
    private Predios o_casa;
    private Predios o_trabalho;
    private Predios o_escola;

    private mPredios m_casa;
    private mPredios m_trabalho;
    private mPredios m_escola;


    private UnityEngine.AI.NavMeshAgent jogador;
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

    public int identidadepessoa;
    //    private string sdest;
    public cPessoa euPessoa;
    private GameObject geral;


    private GameObject predioAtual; // prédio atual (casa ou trabalho)
    private UnityEngine.AI.NavMeshObstacle obstaculoAtual; // obstáculo a ser desativado
                                                           //    private bool estaDentroDoPredio = false;
                                                           //
                                                           //    UnityEngine.AI.NavMeshAgent agente;    ///teste de custo do terreno - ja ta definido como jogador na linha 32
    Renderer rend;          ///teste de custo do terreno

    // Start is called before the first frame update
    void Start()
    {
        geral = GameObject.Find("Terrain");

        //   contatos = new List<GameObject>();
        identidadepessoa = geral.GetComponent<levelgenerator>().contador;
        geral.GetComponent<levelgenerator>().contador++;
        //        euPessoa = geral.GetComponent<levelgenerator>().todasAsPessoas[identidadepessoa];//////////////////////////////////
        this.name = euPessoa.identidade;

        //        identidadepessoa = geral.nomepessoa;
        //        Debug.Log("PESSOA-START: nome da pessoa: " + this.name);


        jogador = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        jogador.speed = 50;// * geral.GetComponent<levelgenerator>().todasAsPessoas.Count;
                           //        GameObject[] trabalhos = GameObject.FindGameObjectsWithTag("trabalho");

        //        tipoDaPessoa = tiposPessoa[Random.Range(0, tiposPessoa.Length)];
        tipoDaPessoa = "jovem"; //APESAR DE EXISTIR, NAO ESTA SENDO COLOCADO EM USO

        predioAtual = euPessoa.MminhaCasa.predioPreFab; //euPessoa.minhaCasa.predioPreFab;// euPessoa.minhaCasa.predioPreFab; // certifique-se de que essa referência está correta
        obstaculoAtual = predioAtual.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        Debug.Log("nome da predio ccom navObstacle" + predioAtual.name);
        //GameObject objCasa = euPessoa.minhaCasa.predioGameObject;

        //        var obstaculo = predioAtual.GetComponent<NavMeshObstacle>();
        //        if (obstaculo != null)
        //        {
        //            obstaculo.enabled = false;
        //        }


        //        casa = this.transform.position;
        //        int num_trab = Random.Range(0, trabalhos.Length);
        //        trabalho = trabalhos[num_trab].transform.position;

        destino = predioAtual.transform.position;// o_casa.enderecoXYZ;// casa;
                                                 //        jogador.SetDestination(destino);
        jogador.stoppingDistance = 1f;
        jogador.areaMask = UnityEngine.AI.NavMesh.AllAreas;

        jogador.SetAreaCost(0, 1f);     // Walkable
        jogador.SetAreaCost(3, 100000f); // Interior
        jogador.SetAreaCost(4, 1f);     // Caminhos

        //        Debug.Log("casa: " + casa + "    trabalho (" + num_trab + "): " + trabalho);

        //    lRede = GetComponent<LineRenderer>();

        //agente = GetComponent<UnityEngine.AI.NavMeshAgent>();   ///teste de custo do terreno
        rend = GetComponent<Renderer>();        ///teste de custo do terreno
    }


    // Update is called once per frame
    void Update()
    {
        //        bool rodadia = GameObject.Find("Terrain").GetComponent<horas>().rodadia;
        bool rodadia = geral.GetComponent<horas>().rodadia;
        if (rodadia)
        {

            tempo = geral.GetComponent<horas>().hora;
            //   Debug.Log("horas: " + tempo);
            ////////////////////////////////////estrutura de destino poderia ser uma lista de destinos da classe. dai faria 'if' pra ver se deu a hora vigente, incrementa, e passa
            ///////////////////////////////////para o proximo da lista. mas nao implementado ainda, pensar a respeito para outra versao (pos testes)
            string t = "jovem";////////////////////////---------------argumento para uso de CASE variavel, pode variar com o tipo de pessoa

            switch (tipoDaPessoa)
            {
                case string a when a.Contains(t):

                    switch (tempo)
                    {
                        //private string[] dest = { "jardins", "alameda", "fonteLinear", "fonteCircular", "fonteFemininas", "coreto", "plataforma" };
                        case 2:
                            destino = euPessoa.MmeuTrabalho.enderecoXYZ;//euPessoa.meuTrabalho.enderecoXYZ;//trabalho;
                                                                        //                            predioAtual = euPessoa.meuTrabalho.predioPreFab;
                                                                        //                                sdest = "trabalho";
                            break;

                        case 18:
                            destino = euPessoa.MminhaCasa.enderecoXYZ; //euPessoa.minhaCasa.enderecoXYZ; //casa;
                                                                       //                          predioAtual = euPessoa.minhaCasa.predioPreFab;
                                                                       //                                sdest = "casa";
                            break;
                    }
                    /*
                    // Se mudou de destino, garante que o obstáculo está ativado
                    if (predioAtual != null)
                    {
                        obstaculoAtual = predioAtual.GetComponent<NavMeshObstacle>();
                        if (obstaculoAtual != null)
                            obstaculoAtual.enabled = true;
                    }
                    estaDentroDoPredio = false; // novo destino = reset status
                    */
                    if (jogador.isOnNavMesh)
                    {
                        jogador.SetDestination(destino);
                    }
                    else
                    {
                        Debug.LogWarning($"{name} não está sobre a NavMesh.");
                    }
                    //                    jogador.SetDestination(destino);   //DESCOBRIR PQ O TERRENO E A NAVMESH TA SE MEXENDO JUNTO COM O JOGADOR -  terreno estava com funcao de agente tb (o q significa funcao de agente tb?)
                    break;
            }

            /*
            // Verifica distância ao destino
            if (!estaDentroDoPredio && Vector3.Distance(transform.position, destino) < jogador.stoppingDistance + 0.5f)
            {
                if (obstaculoAtual != null)
                {
                    obstaculoAtual.enabled = false; // permite entrada
                    estaDentroDoPredio = true;
                }
            }

            // Reativa o obstáculo se agente saiu do prédio
            if (estaDentroDoPredio && Vector3.Distance(transform.position, destino) > jogador.stoppingDistance + 1.5f)
            {
                if (obstaculoAtual != null)
                {
                    obstaculoAtual.enabled = true;
                    estaDentroDoPredio = false;
                }
            }
            */

            //            Debug.Log("PESSOA-UPDATE: pessoa " + this.name + " indo para " + sdest);
            //--------acrescentar mudanca de destino aki para atualizacao.

            ///monitoramento do custo do terreno
            ///

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                int areaIndex = hit.mask; // máscara com 1 << areaIndex
                Debug.Log("areaIndex: " + areaIndex);
                int areaId = -1;
                for (int i = 0; i < 32; i++)
                {
                    if ((areaIndex & (1 << i)) != 0)
                    {
                        areaId = i;
                        break;
                    }
                }

                if (areaId != -1)
                {
                    float custo = UnityEngine.AI.NavMesh.GetAreaCost(areaId);
                    Color cor = Color.gray;

                    // Altere as cores conforme custo
                    if (custo <= 1f)
                        cor = Color.green;
                    else if (custo <= 5f)
                        cor = Color.yellow;
                    else
                        cor = Color.red;

                    rend.material.color = cor;
                }
            }
            else
                Debug.Log("nao achei navmesh?");

            //            rend.material.color = Color.yellow;


        }
    }
}

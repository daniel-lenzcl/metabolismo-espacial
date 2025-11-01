using UnityEngine;
using UnityEngine.AI;

public class Rotina : MonoBehaviour
{
    public cPessoa pessoa; // referência a mPessoa
    private NavMeshAgent agente;
    public float custo;
    private GameObject geral; //deixa aki para atualizacao a partir de 'ambiente'
    private Vector3 destino;

    private horas relogio;
    private float hora_instantanea;

    private NavMeshPath caminhoCasaTrabalho, caminhoTrabalhoCasa, caminhoTrabalhoRestaurante, caminhoRestauranteTrabalho;//, caminhoCA;  // Armazenando os caminhos pré-calculados

    void Awake()
    {
        DebugController.Log(DebugCategoria.tracking, $"Awake ▸ tracking");

        geral = GameObject.Find("Terrain");
        configNavAgent();
    }

    void Start()
    {
        DebugController.Log(DebugCategoria.tracking, $"Start ▸ tracking");

        // Calcular os caminhos inicialmente
        CalcularCaminhos();

        if (pessoa != null && pessoa.MminhaCasa != null)
        {
            IrParaCasa();
        }
        pessoa.OnCasaDefinida.AddListener(IrParaCasa);
        //        hora_instantanea = geral.GetComponent<horas>().hora;

        relogio = FindObjectOfType<horas>();  // Encontre o Relógio
        if (relogio != null)
        {
            relogio.MudouHora.AddListener(AtualizaDestino);
        }

    }

    void Update()
    {
    }

    void CalcularCaminhos()
    {
        caminhoCasaTrabalho = new NavMeshPath();
        caminhoTrabalhoCasa = new NavMeshPath();
        caminhoTrabalhoRestaurante = new NavMeshPath();
        caminhoRestauranteTrabalho = new NavMeshPath();

        //        caminhoCA = new NavMeshPath();

        //        agente.SetDestination(pessoa.MminhaCasa.enderecoXYZ);

        // Calcular o caminho de A para B
        if (NavMesh.CalculatePath(pessoa.MminhaCasa.enderecoXYZ, pessoa.MmeuTrabalho.enderecoXYZ, NavMesh.AllAreas, caminhoCasaTrabalho))
        {
            DebugController.Log(DebugCategoria.Rotina, $"Caminho A → B calculado com sucesso.");
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "Não foi possível calcular o caminho A → B.");
        }

        // Calcular o caminho de B para C
        if (NavMesh.CalculatePath(pessoa.MmeuTrabalho.enderecoXYZ, pessoa.MminhaCasa.enderecoXYZ, NavMesh.AllAreas, caminhoTrabalhoCasa))
        {
            DebugController.Log(DebugCategoria.Rotina, "Caminho B → C calculado com sucesso.");
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "Não foi possível calcular o caminho B → C.");
        }

        // Calcular o caminho de B para C
        if (NavMesh.CalculatePath(pessoa.MmeuTrabalho.enderecoXYZ, pessoa.MmeuRestaurante.enderecoXYZ, NavMesh.AllAreas, caminhoTrabalhoRestaurante))
        {
            DebugController.Log(DebugCategoria.Rotina, "Caminho B → C calculado com sucesso.");
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "Não foi possível calcular o caminho B → C.");
        }
        // Calcular o caminho de B para C
        if (NavMesh.CalculatePath(pessoa.MmeuRestaurante.enderecoXYZ, pessoa.MmeuTrabalho.enderecoXYZ, NavMesh.AllAreas, caminhoRestauranteTrabalho))
        {
            DebugController.Log(DebugCategoria.Rotina, "Caminho B → C calculado com sucesso.");
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Rotina, "Não foi possível calcular o caminho B → C.");
        }
        /*
        // Calcular o caminho de C para A
        if (NavMesh.CalculatePath(C.position, A.position, NavMesh.AllAreas, caminhoCA))
        {
            Debug.Log("Caminho C → A calculado com sucesso.");
        }
        else
        {
            Debug.LogWarning("Não foi possível calcular o caminho C → A.");
        }
        */
    }

    private void AtualizaDestino()
    {
        DebugController.Log(DebugCategoria.tracking, $"Update ▸ tracking");
        Debug.Log("mudou hora");

        hora_instantanea = relogio.hora;
        string dest = "";
        ////////////////////////////////////estrutura de destino poderia ser uma lista de destinos da classe. dai faria 'if' pra ver se deu a hora vigente, incrementa, e passa
        ///////////////////////////////////para o proximo da lista. mas nao implementado ainda, pensar a respeito para outra versao (pos testes)
        if (hora_instantanea <= 2)
        {
            //                IrParaCasa();
//            destino = pessoa.MminhaCasa.enderecoXYZ; //euPessoa.minhaCasa.enderecoXYZ; //casa;
            dest = pessoa.MminhaCasa.nomePredio;
            agente.SetPath(caminhoTrabalhoCasa);
            //                Debug.Log($"{hora_instantanea} indo para casa {pessoa.MminhaCasa.nomePredio} no {destino}");
        }
        if (hora_instantanea >= 2 && hora_instantanea <= 8)
        {
            //                IrParaTrabalho();
//            destino = pessoa.MmeuTrabalho.enderecoXYZ;//euPessoa.meuTrabalho.enderecoXYZ;//trabalho;
            dest = pessoa.MmeuTrabalho.nomePredio;
            agente.SetPath(caminhoCasaTrabalho);

            //                Debug.Log($"{hora_instantanea} indo para trabalho {pessoa.MmeuTrabalho.nomePredio} no {destino}");
        }
        if (hora_instantanea >= 8 && hora_instantanea <= 11)
        {
            //                IrParaTrabalho();
            //            destino = pessoa.MmeuTrabalho.enderecoXYZ;//euPessoa.meuTrabalho.enderecoXYZ;//trabalho;
            dest = pessoa.MmeuRestaurante.nomePredio;
            agente.SetPath(caminhoTrabalhoRestaurante);

            //                Debug.Log($"{hora_instantanea} indo para trabalho {pessoa.MmeuTrabalho.nomePredio} no {destino}");
        }
        if (hora_instantanea >= 11 && hora_instantanea <= 16)
        {
            //                IrParaTrabalho();
            //            destino = pessoa.MmeuTrabalho.enderecoXYZ;//euPessoa.meuTrabalho.enderecoXYZ;//trabalho;
            dest = pessoa.MmeuTrabalho.nomePredio;
            agente.SetPath(caminhoRestauranteTrabalho);

            //                Debug.Log($"{hora_instantanea} indo para trabalho {pessoa.MmeuTrabalho.nomePredio} no {destino}");
        }
        if (hora_instantanea >= 16)// && hora_instantanea <= 23)
        {
            //                IrParaCasa();
//            destino = pessoa.MminhaCasa.enderecoXYZ; //euPessoa.minhaCasa.enderecoXYZ; //casa;
            dest = pessoa.MminhaCasa.nomePredio;
            agente.SetPath(caminhoTrabalhoCasa);

            //                Debug.Log($"{hora_instantanea} indo para casa {pessoa.MminhaCasa.nomePredio} no {destino}");
        }
        DebugController.Log(DebugCategoria.Rotina, $"AtualizaDestino ▸ tracking hora: {hora_instantanea} indo para {dest} no {destino}");
        
//        ChecaDestinoNavMesh();
    }

    private void ChecaDestinoNavMesh()
    {
        if (agente.isOnNavMesh)
        {
            // Verifica se o destino está válido e acessível
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destino, out hit, 100.0f, NavMesh.AllAreas))
            {
                // Ajusta a posição do destino para a altura correta da NavMesh
                Vector3 destinoAjustado = destino;// new Vector3(destino.x, hit.position.y, destino.z);

                // Calcula o caminho até o destino ajustado
                NavMeshPath path = new NavMeshPath();
                agente.CalculatePath(destinoAjustado, path);

                // Verifica se o caminho é válido
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    // O caminho é viável, então o agente pode ir para o destino
                    agente.SetDestination(destinoAjustado);
                    DebugController.Log(DebugCategoria.Rotina, $"Destino ajustado para {destinoAjustado}");
                }
                else
                {
                    DebugController.LogWarning(DebugCategoria.Rotina, $"Não foi possível calcular um caminho até {destinoAjustado}");
                }
            }
            else
            {
                DebugController.LogWarning(DebugCategoria.Rotina, $"Destino {destino} não está acessível na NavMesh!");
            }
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.Rotina, $"{pessoa.identidade} não está sobre a NavMesh.");
        }
    }

    private void configNavAgent()
    {
        agente = GetComponent<NavMeshAgent>();
        GameObject obj = GetComponent<GameObject>();
        if (agente == null)
        {
            DebugController.LogWarning(DebugCategoria.Rotina, $"Awake ▸ NavMeshAgent não encontrado!");
            agente = gameObject.AddComponent<NavMeshAgent>();
            DebugController.LogWarning(DebugCategoria.Rotina, $"Awake ▸ NavMeshAgent criado!");
        }

        // Ajusta a velocidade do agente
        agente.speed = 30.0f;  // A velocidade máxima do agente
        // Ajusta a aceleração (como rapidamente o agente atinge a velocidade máxima)
        agente.acceleration = 500.0f;  // Aceleração
        // Ajusta a velocidade angular (velocidade com que o agente vira)
        agente.angularSpeed = 120f;  // Quanto maior, mais rápido o agente gira
        // Ajusta o raio do agente (determinando o tamanho da colisão)
        agente.radius = 0.5f;  // O raio do agente (quanto maior, mais espaço o agente ocupa)

        agente.stoppingDistance = 3f;

    }

    public void IrParaCasa()
    {
        DebugController.Log(DebugCategoria.tracking, $"IrParaCasa ▸ tracking");

        //        if (agente != null)
        //            agente.SetDestination(pessoa.MminhaCasa.enderecoXYZ);

        agente.SetDestination(pessoa.MminhaCasa.enderecoXYZ);
    }

    public void IrParaTrabalho()
    {
        DebugController.Log(DebugCategoria.tracking, $"IrParaTrabalho ▸ tracking");

        //      if (agente != null)
        //          agente.SetDestination(pessoa.MmeuTrabalho.enderecoXYZ);
        agente.SetDestination(pessoa.MmeuTrabalho.enderecoXYZ);
    }

    public void OnDestroy()
    {
        DebugController.Log(DebugCategoria.tracking, $"OnDestroy ▸ tracking");

        pessoa.despedida();
    }
    // Aqui você pode criar um método para atualizar a rotina baseado na hora, etc.
}

/*using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DefinicaoPortas : MonoBehaviour
{
    [Tooltip("Pontos vazios (GameObjects) que representam possíveis locais de porta.")]
    public List<Transform> candidateDoorPoints;

    [Tooltip("Distância máxima para a NavMesh para considerar um ponto como 'de frente para a rua'.")]
    public float maxStreetDistance = 30.0f;

    [Tooltip("Tamanho do Box Collider que será o gatilho da porta.")]
    public Vector3 triggerSize = new Vector3(2f, 3f, 0.5f);

    [Tooltip("Offset para posicionar o gatilho da porta para fora do prédio.")]
    public float triggerOffset = 0.2f;

    [Tooltip("Referência ao ponto interno para onde o agente será movido ao entrar.")]
    public Transform buildingInsidePoint;

    [Tooltip("Nome da área da NavMesh que representa as ruas (ex: \"caminhos\").")]
    public string walkableAreaName = "caminhos";//"Walkable"; // Nome da sua área de rua

    private int _walkableAreaMask; // Variável interna para armazenar o bitmask


    // Propriedade para ser definida pelo sistema que gerencia os destinos e agentes
    // Por exemplo, um script que instancia os prédios e sabe qual agente pertence a qual prédio.
    [HideInInspector] // Esconde no Inspector, pois será setado via script
    public string assignedOwnerAgentID;

    void Start()
    {
        // PlaceDynamicDoor();
    }

    void Awake() // Use Awake para garantir que o bitmask seja calculado antes do Start
    {
        int areaIndex = NavMesh.GetAreaFromName(walkableAreaName);
        Debug.Log($"Índice da área '{walkableAreaName}': {areaIndex}");

        if (areaIndex == -1)
        {
            Debug.LogError($"Área da NavMesh '{walkableAreaName}' não encontrada! Verifique o nome da área em Window > AI > Navigation > Areas.", this);
            _walkableAreaMask = 3; // Define como 0 para evitar erros, mas a busca não funcionará
        }
        else
        {
            _walkableAreaMask = 1 << areaIndex;
        }
        Debug.Log("area da navmesh: " + _walkableAreaMask);
    }

    public void PlaceDynamicDoor()
    {
        Transform bestDoorCandidate = null;
        float minDistanceToNavMesh = float.MaxValue;

        if (candidateDoorPoints == null || candidateDoorPoints.Count == 0)
        {
            Debug.LogWarning($"Nenhum ponto candidato para porta encontrado no prédio {gameObject.name}. Não será possível colocar a porta dinâmica.", this);
            return;
        }
        Debug.Log("total de opcoes pra porta: " + candidateDoorPoints.Count);

        foreach (Transform candidatePoint in candidateDoorPoints)
        {
            NavMeshHit hit;
            // Tenta encontrar o ponto mais próximo na NavMesh a partir do ponto candidato
            if (NavMesh.SamplePosition(candidatePoint.position, out hit, maxStreetDistance, _walkableAreaMask))

            //                if (NavMesh.SamplePosition(candidatePoint.position, out hit, maxStreetDistance, NavMesh.AllAreas))
            {
                Debug.Log("distancia pra navmesh: " + hit.distance);
                // Se encontrou um ponto na NavMesh dentro da distância máxima
                if (hit.distance < minDistanceToNavMesh)
                {
                    minDistanceToNavMesh = hit.distance;
                    bestDoorCandidate = candidatePoint;
                }
            }
        }

        if (bestDoorCandidate != null)
        {
            // Cria um novo GameObject para o gatilho da porta
            GameObject doorTriggerGO = new GameObject($"DoorTrigger_{gameObject.name}");
            doorTriggerGO.transform.parent = transform; // Torna o gatilho filho do prédio
            doorTriggerGO.transform.position = bestDoorCandidate.position + bestDoorCandidate.forward * triggerOffset;
            doorTriggerGO.transform.rotation = bestDoorCandidate.rotation;

            // Adiciona e configura o BoxCollider
            BoxCollider doorCollider = doorTriggerGO.AddComponent<BoxCollider>();
            doorCollider.isTrigger = true;
            doorCollider.size = triggerSize;

            // Adiciona e configura o script DestinationAccessControl
            //            DestinationAccessControl accessControl = doorTriggerGO.AddComponent<DestinationAccessControl>();
            //            accessControl.ownerAgentID = assignedOwnerAgentID; // Define o ID do agente proprietário
            //            accessControl.insidePoint = buildingInsidePoint; // Define o ponto interno

            Debug.Log($"Porta dinâmica colocada para o prédio {gameObject.name} no ponto: {bestDoorCandidate.name}");
        }
        else
        {
            Debug.LogWarning($"Não foi possível encontrar um local adequado para a porta dinâmica no prédio {gameObject.name}. Verifique a NavMesh e os pontos candidatos.", this);
        }
    }
}

*/
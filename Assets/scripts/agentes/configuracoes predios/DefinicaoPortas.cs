using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefinicaoPortas : MonoBehaviour
{
    [Tooltip("Layer onde os outros prédios estão para detecção de vizinhos.")]
    public LayerMask prediosLayer;

    [Tooltip("Layer onde as ruas estão para detecção de vizinhos.")]
    public LayerMask caminhosLayer;

    [Tooltip("Distância máxima para verificar a presença de prédios ou ruas vizinhas.")]
    public float raycastDistance = 10.0f; // Distância para Raycast de vizinhos

    // Campos para uso futuro, não usados nesta versão de identificação de limites
    public Transform buildingInsidePoint;
    [HideInInspector]
    public string assignedOwnerAgentID;

    private Mesh _buildingMesh; // A mesh do prédio

    public levelgenerator d_levelgenerator_local;


    void Awake()
    {
        d_levelgenerator_local = FindObjectOfType<Terrain>().GetComponent<levelgenerator>();

        prediosLayer = LayerMask.GetMask("predios");
        caminhosLayer = LayerMask.GetMask("caminhos");

        // Obter a mesh do prédio
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            _buildingMesh = meshFilter.mesh;
        }
        else
        {
            Debug.LogError($"MeshFilter não encontrado no GameObject {gameObject.name}. Certifique-se de que o prédio tem um MeshFilter com uma mesh atribuída.", this);
        }
    }

    // Este método será chamado externamente após a identificação do objeto como prédio
    public void IdentifyAndMarkBoundaries()
    {
        if (_buildingMesh == null)
        {
            Debug.LogError("Mesh do prédio não encontrada. Não é possível identificar e marcar limites.", this);
            return;
        }

        Vector3[] vertices = _buildingMesh.vertices;
        int[] triangles = _buildingMesh.triangles;

        // Dicionário para contar quantas vezes cada aresta aparece (para encontrar arestas de contorno)
        Dictionary<Edge, int> edgeCount = new Dictionary<Edge, int>();

        // Percorre todos os triângulos para encontrar as arestas
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            AddEdge(edgeCount, vertices[v1], vertices[v2]);
            AddEdge(edgeCount, vertices[v2], vertices[v3]);
            AddEdge(edgeCount, vertices[v3], vertices[v1]);
        }

        // Filtra as arestas que aparecem apenas uma vez (arestas de contorno)
        List<Edge> boundaryEdges = edgeCount.Where(pair => pair.Value == 1).Select(pair => pair.Key).ToList();

        if (boundaryEdges.Count == 0)
        {
            Debug.LogWarning($"Nenhuma aresta de contorno encontrada para o prédio {gameObject.name}. Não será possível identificar limites.", this);
            return;
        }

        foreach (Edge edge in boundaryEdges)
        {
            // Converte as posições dos vértices da aresta para o espaço global
            Vector3 globalV1 = transform.TransformPoint(edge.v1);
            Vector3 globalV2 = transform.TransformPoint(edge.v2);

            Vector3 edgeMidpoint = (globalV1 + globalV2) / 2f;

            // Calcula a normal da aresta (apontando para fora do prédio no plano XZ)
            Vector3 edgeDirection = (globalV2 - globalV1).normalized; // Vetor ao longo da aresta
            // Para uma mesh plana no plano XZ, a normal para fora é a direção da aresta rotacionada 90 graus
            Vector3 outwardNormal = new Vector3(-edgeDirection.z, 0, edgeDirection.x).normalized; // Rotação 90 graus no plano XZ

            // Lança um Raycast para identificar o vizinho
            RaycastHit hitNeighbor;
            // Combina as layers de prédios e caminhos para o Raycast
            LayerMask combinedLayerMask = prediosLayer | caminhosLayer;

            // Adiciona um pequeno offset vertical para garantir que o raycast não fique "preso" no plano
            // e possa atingir o collider do vizinho que está no mesmo plano.
            // O offset deve ser pequeno o suficiente para não pular o vizinho.
            Vector3 raycastOrigin = edgeMidpoint + Vector3.up * 0.1f; // Pequeno offset para cima

            if (Physics.Raycast(raycastOrigin, outwardNormal, out hitNeighbor, raycastDistance, combinedLayerMask))
            {
                GameObject markerGO = new GameObject();
                markerGO.transform.parent = transform; // Torna o marcador filho do prédio
                markerGO.transform.position = hitNeighbor.point; // Posiciona no ponto de colisão
                markerGO.transform.forward = outwardNormal; // Orienta para fora

                if (((1 << hitNeighbor.collider.gameObject.layer) & prediosLayer) != 0) // Verifica se a layer do objeto atingido está na máscara de prédios
                {
                    // Atingiu outro prédio
                    markerGO.name = "Parede";
                    Debug.Log($"Aresta em {edgeMidpoint} do prédio {gameObject.name} é uma Parede (vizinho: {hitNeighbor.collider.name}).");
                }
                else if (((1 << hitNeighbor.collider.gameObject.layer) & caminhosLayer) != 0) // Verifica se a layer do objeto atingido está na máscara de caminhos
                {
                    // Atingiu uma rua
                    markerGO.name = "Porta";
                    Debug.Log($"Aresta em {edgeMidpoint} do prédio {gameObject.name} é uma Porta (vizinho: {hitNeighbor.collider.name}).");
                }
                else
                {
                    // Caso atinja algo inesperado (outra layer na combinedLayerMask)
                    markerGO.name = "VizinhoDesconhecido";
                    Debug.LogWarning($"Aresta em {edgeMidpoint} do prédio {gameObject.name} atingiu um vizinho desconhecido: {hitNeighbor.collider.name} na layer {LayerMask.LayerToName(hitNeighbor.collider.gameObject.layer)}.");
                }
            }
            else
            {
                // Se não atingiu nada dentro da raycastDistance, pode ser uma borda externa sem vizinho próximo
                // ou um erro de configuração de layers/distância.
                Debug.LogWarning($"Aresta em {edgeMidpoint} do prédio {gameObject.name} não encontrou vizinho dentro da distância de {raycastDistance}. Pode ser uma borda externa sem vizinho próximo.");
            }
        }
    }

    // Estrutura auxiliar para representar uma aresta
    private struct Edge : System.IEquatable<Edge>
    {
        public Vector3 v1; // Vértice 1 da aresta (em espaço local)
        public Vector3 v2; // Vértice 2 da aresta (em espaço local)

        public Edge(Vector3 p1, Vector3 p2)
        {
            // Garante uma ordem consistente para que {v1, v2} e {v2, v1} sejam considerados a mesma aresta
            if (p1.x < p2.x || (p1.x == p2.x && p1.y < p2.y) || (p1.x == p2.x && p1.y == p2.y && p1.z < p2.z))
            {
                v1 = p1;
                v2 = p2;
            }
            else
            {
                v1 = p2;
                v2 = p1;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Edge other && Equals(other);
        }

        public bool Equals(Edge other)
        {
            return (v1 == other.v1 && v2 == other.v2) || (v1 == other.v2 && v2 == other.v1);
        }

        public override int GetHashCode()
        {
            // Combina os hash codes dos dois vértices de forma simétrica
            return v1.GetHashCode() ^ v2.GetHashCode();
        }
    }

    // Adiciona uma aresta ao dicionário de contagem
    private void AddEdge(Dictionary<Edge, int> edgeCount, Vector3 p1, Vector3 p2)
    {
        Edge edge = new Edge(p1, p2);
        if (edgeCount.ContainsKey(edge))
        {
            edgeCount[edge]++;
        }
        else
        {
            edgeCount.Add(edge, 1);
        }
    }
}

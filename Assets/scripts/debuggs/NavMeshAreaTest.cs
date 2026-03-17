using Unity.AI.Navigation;
using UnityEngine;
using NavMeshModifier = Unity.AI.Navigation.NavMeshModifier;
using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;

public class NavMeshAreaTest : MonoBehaviour
{
    public int gridX = 3;
    public int gridZ = 3;
    public float tileSize = 2f;

    public GameObject customMeshPrefab; // ? arraste aqui um prefab importado (ex: plano .obj)

    public Material walkableMat;
    public Material interiorMat;
    public Material pathMat;

    void Start()
    {
        GameObject parent = new GameObject("NavMeshTestParent");

        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                GameObject tile;

                if (customMeshPrefab != null)
                {
                    tile = Instantiate(customMeshPrefab);
                }
                else
                {
                    tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                }

                tile.transform.parent = parent.transform;
                tile.transform.position = new Vector3(x * tileSize, 0, z * tileSize);
                tile.transform.localScale = Vector3.one * tileSize * 0.1f; // Ajuste para Plane (10x10 por padrão)

                tile.isStatic = true;

                // NavMesh Modifier
                var modifier = tile.AddComponent<NavMeshModifier>();
                modifier.overrideArea = true;

                int tipo = (x + z) % 3;
                switch (tipo)
                {
                    case 0:
                        modifier.area = 0;
                        if (walkableMat) tile.GetComponent<Renderer>().material = walkableMat;
                        break;
                    case 1:
                        modifier.area = 3;
                        if (interiorMat) tile.GetComponent<Renderer>().material = interiorMat;
                        break;
                    case 2:
                        modifier.area = 4;
                        if (pathMat) tile.GetComponent<Renderer>().material = pathMat;
                        break;
                }

                // Colisor
                if (!tile.GetComponent<Collider>())
                    tile.AddComponent<MeshCollider>();
            }
        }

        var surface = parent.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.ignoreNavMeshAgent = false;
        surface.ignoreNavMeshObstacle = false;

        surface.BuildNavMesh();
    }
}

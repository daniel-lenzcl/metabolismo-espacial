using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using static UnityEditor.ShaderData;
using Unity.AI.Navigation;
using UnityEngine.AI;
using NavMeshSurface = UnityEngine.AI.NavMeshSurface;

public class CarregarMapa : MonoBehaviour
{
    List<Predios> CamadaCasa;
    List<Predios> CamadaTrabalho;
    public GameObject terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Terrain");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Bounds Bounds_mapa_importado(GameObject mapa_importado)
    {
            Bounds totalBounds = new Bounds();
            Renderer[] renderers = mapa_importado.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                // Inicializar os bounds com os primeiros renderers
                totalBounds = renderers[0].bounds;

                foreach (Renderer renderer in renderers)
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
            }

            Debug.Log("Center: " + totalBounds.center);
            Debug.Log("Size: " + totalBounds.size);
        return totalBounds;
    }

    public void Locar_Predios(GameObject camada, GameObject predio, List<Predios> lista) 
    {
//        int filhos_c = casas.transform.childCount;

        for (int i = 0; i < camada.transform.childCount; i++)
        {
            Vector3 offset_altura =  new Vector3(0, predio.transform.GetComponent<Renderer>().bounds.size.y / 2, 0);
            GameObject child = camada.transform.GetChild(i).gameObject;
            Vector3 centro_child = child.GetComponent<Renderer>().bounds.center;
            //            Debug.Log("o q tem em camada casa [" + i + "]: " + CamadaCasa[i].name);
            Debug.Log("o child  [" + i + "]: " + child.name + "; na posicao: " + centro_child);
//            Instantiate(predio, centro_child + offset_altura, Quaternion.identity);
//            Instantiate(geral.GetComponent<levelgenerator>().casa, centro_child + offset_altura, Quaternion.identity);
            Predios cada_child = new Predios(predio, child.name, centro_child, 3);
            lista.Add(cada_child);
            terrain.GetComponent<levelgenerator>().predios.Add(cada_child);
        }
    }

    public void captura()
    {
        GameObject geral = GameObject.Find("Terrain");
        GameObject camada = GameObject.Find("ref_rua");
        //        Debug.Log("nome camada: " + camada.name);
        GameObject escala = GameObject.Find("escala");
        GameObject trabalhos = camada.transform.Find("trabalho").gameObject;
        GameObject casas = camada.transform.Find("casa").gameObject;
        //Debug.Log("child em geral: " + geral.transform.childCount +
        //          "\n child em camada: " + camada.transform.childCount +
        //          "\n child em escala: " + escala.transform.childCount +
        //          "\n child em trabalhos: " + trabalhos.transform.childCount +
        //          "\n child em cs: " + casas.transform.childCount
        //          );

                
        terrain.GetComponent<NavMeshSurface>().enabled = false;
                
        GameObject ruas = camada.transform.Find("rua").gameObject;

        NavMeshSurface mesh_rua = ruas.AddComponent<NavMeshSurface>();
        mesh_rua.agentTypeID = 0; // Ajuste o ID conforme o tipo de agente desejado; 0 é geralmente o default humanoide
        mesh_rua.collectObjects = UnityEngine.AI.CollectObjects.Children; // Coleta meshes dos filhos do GameObject

        mesh_rua.useGeometry = NavMeshCollectGeometry.RenderMeshes;

        // Configure outras propriedades conforme necessário
        // navMeshSurface.layerMask = LayerMask.GetMask("Default"); // Exemplo de configuração de quais camadas serão incluídas

        // Constroi a NavMesh usando a superfície
        mesh_rua.BuildNavMesh();



        terrain.GetComponent<levelgenerator>().setaListas("predios");
        terrain.GetComponent<levelgenerator>().setaListas("enderecos");
        terrain.GetComponent<levelgenerator>().setaListas("pessoas");
        terrain.GetComponent<levelgenerator>().iniciaMapa();


        CamadaCasa = new List<Predios>();
        CamadaTrabalho = new List<Predios>();



        //pegar o obj de referencia de escala e definir fator_de_escala
        float fator_de_escala = 1;
        Renderer ref_escala = escala.GetComponentInChildren<Renderer>();
        if (ref_escala!= null)
        {
            Vector3 tamanho_ref = ref_escala.bounds.size;
            fator_de_escala = 2 / tamanho_ref.x;
        }
        //        fator_de_escala *= 10f;
        camada.transform.localScale = new Vector3(fator_de_escala, fator_de_escala, fator_de_escala);

        Bounds limites_mapa_importado = Bounds_mapa_importado(camada);
        Vector3 centro = terrain.GetComponent<Terrain>().terrainData.bounds.center;
        camada.transform.position = centro + (limites_mapa_importado.size/2);
        //Debug.Log("posicao do mapa importado: " + cs.transform.position + 
        //            "; limites do mapa importado: " + limites_mapa_importado.size + 
        //            "; centro do mapa importado: " + limites_mapa_importado.center);
        //Debug.Log("posicao do mapa: " + terrain.transform.position);
//        terrain.GetComponent<levelgenerator>().maxPessoasI = 3;

        Locar_Predios(casas, geral.gameObject.GetComponent<levelgenerator>().casa, CamadaCasa);
        Locar_Predios(trabalhos, geral.gameObject.GetComponent<levelgenerator>().trabalho, CamadaTrabalho);

        //Debug.Log("casas adquiridas: " + casas.transform.childCount + 
        //        "; trabalhos adquiridos: " + trabalhos.transform.childCount);
        Debug.Log("lista casas adquiridas: " + CamadaCasa.Count + 
                "; lista trabalhos adquiridos: " + CamadaTrabalho.Count +
                "; total de predios level generator: " + terrain.GetComponent<levelgenerator>().predios);

        terrain.GetComponent<levelgenerator>().maxPessoasI = 1;
        terrain.GetComponent<levelgenerator>().todasAsPessoas.Clear();

        terrain.GetComponent<levelgenerator>().totalPessoas = CamadaCasa.Count * terrain.GetComponent<levelgenerator>().maxPessoasI;
//        for (int i = 0; i < CamadaCasa.Count; i++)

        ///definir as pessoas a partir das casas, ja atribuindo casa e trabalho
        for (int i = 0; i < terrain.GetComponent<levelgenerator>().totalPessoas; i++)
            {
            //lista de casas e trabalhos tem q ser do tipo Predio
                terrain.GetComponent<levelgenerator>().todasAsPessoas.Add(new cPessoa(terrain.GetComponent<levelgenerator>().prefabP, "pessoa" + i, CamadaCasa[i], CamadaTrabalho[i]));
        }
        //        this.GetComponent<relacoes>().bAtribuiRelacoes();
        //        terrain.GetComponent<levelgenerator>().colocaPessoas();





    }

    public void ReposicionaMapaTerreno()
    {

    }
}

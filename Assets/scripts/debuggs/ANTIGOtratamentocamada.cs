using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
/*
/// <summary>
/// recebe o mapa, reconhece e organiza por camadas, e ajusta o tamanho da escala e do terreno
/// </summary>
public class TratamentoMapaCarregado : MonoBehaviour
{
    [HideInInspector]
    public GameObject mapaImportadoRaiz;
    public Terrain terrain;
    public Bounds limites_mapa_importado;

    // Mapeia nome do objeto (ex: object_1) para nome da camada (ex: rua, casa)
    private Dictionary<string, string> mapaObjetoParaCamada;

    // Grupos de camadas acessíveis externamente
    public Dictionary<string, GameObject> GruposCamadas { get; private set; } = new();

    public UnityEvent OnMapaCarregado = new UnityEvent();

    // --------------------------------------------------------------
    void Start()
    {
        terrain = FindObjectOfType<Terrain>();

        if (mapaImportadoRaiz == null)
        {
            DebugController.LogError(DebugCategoria.TratamentoMapaCarregado, "Mapa importado não atribuído.");
            return;
        }
        if (mapaObjetoParaCamada == null)
        {
            DebugController.LogError(DebugCategoria.TratamentoMapaCarregado, "Mapa de objetos para camadas não inicializado.");
            return;
        }
    }

    // --------------------------------------------------------------
    /// <summary>Recebe o conteúdo do .OBJ, monta mapa de objeto?camada e organiza tudo.</summary>
    public void InicializarMapaCamadas(string conteudoOBJ)
    {
        mapaObjetoParaCamada = ObterMapaObjetoParaCamada(conteudoOBJ);
        OrganizarPorCamadas();
        AjusteEscalaPosicao();
        OnMapaCarregado.Invoke();
    }

    public void LimparMapaAnterior()
    {
        if (mapaImportadoRaiz != null)
        {
            Destroy(mapaImportadoRaiz);
            mapaImportadoRaiz = null;
            mapaObjetoParaCamada = null;
            DebugController.Log(DebugCategoria.TratamentoMapaCarregado, "Mapa anterior removido.");
        }
    }

    // --------------------------------------------------------------
    public void OrganizarPorCamadas()
    {
        GruposCamadas = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> grupos = new();

        // snapshot dos filhos
        List<Transform> filhos = new();
        foreach (Transform child in mapaImportadoRaiz.transform) filhos.Add(child);

        foreach (Transform filho in filhos)
        {
            string nomeObjeto = filho.name;
            if (!mapaObjetoParaCamada.TryGetValue(nomeObjeto, out string nomeCamada))
            {
                DebugController.LogWarning(DebugCategoria.TratamentoMapaCarregado, $"Objeto '{nomeObjeto}' sem entrada — usando camada 'default'.");
                nomeCamada = "default";
            }

            // cria grupo se necessário
            if (!grupos.ContainsKey(nomeCamada))
            {
                GameObject grupo = new GameObject(nomeCamada);
                grupo.transform.SetParent(mapaImportadoRaiz.transform);

                // layer
                grupo.layer = (nomeCamada.ToLower().Contains("rua") || nomeCamada.ToLower().Contains("terreno"))
                              ? LayerMask.NameToLayer("caminhos")
                              : LayerMask.NameToLayer("predios");

                grupo.AddComponent<CamadaInfo>().nomeCamada = nomeCamada;
                grupos[nomeCamada] = grupo;
                GruposCamadas[nomeCamada] = grupo;
            }

            // reparent & layer
            filho.SetParent(grupos[nomeCamada].transform);
            filho.gameObject.layer = grupos[nomeCamada].layer;
            // renomeia mantendo índice após "_"
            filho.name = nomeCamada + filho.name.Split('_')[1];
        }

        //COLOCAR AS CAMADAS NAS OPCOES

        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, "Organização por camadas concluída.");
    }

    // --------------------------------------------------------------
    /// <summary>Parse do OBJ: mapeia cada "o <nome>" ? grupo "g <camada>".</summary>
    private Dictionary<string, string> ObterMapaObjetoParaCamada(string conteudoOBJ)
    {
        var mapa = new Dictionary<string, string>();
        using StringReader reader = new StringReader(conteudoOBJ);
        string linha;
        string camadaAtual = "default";

        while ((linha = reader.ReadLine()) != null)
        {
            linha = linha.Trim();
            if (linha.StartsWith("g ")) camadaAtual = linha[2..].Trim();
            else if (linha.StartsWith("o "))
            {
                string nomeObj = linha[2..].Trim();
                mapa[nomeObj] = camadaAtual;
            }
        }
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Mapa OBJ processado — {mapa.Count} objetos");
        return mapa;
    }

    // --------------------------------------------------------------
    public void AjusteEscalaPosicao()
    {
        if (GruposCamadas.TryGetValue("escala", out GameObject refEscalaGO))
        {
            float fator = 1f;
            Renderer refRend = refEscalaGO.GetComponentInChildren<Renderer>();
            if (refRend != null)
            {
                fator = 2f / refRend.bounds.size.x;
            }
            mapaImportadoRaiz.transform.localScale = Vector3.one * fator;
            AjustarTerrainParaLimites();
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.TratamentoMapaCarregado, "Escala não encontrada — mantendo tamanho original.");
        }
    }

    // --------------------------------------------------------------
    void AjustarTerrainParaLimites()
    {
        limites_mapa_importado = Bounds_mapa_importado(mapaImportadoRaiz);
        if (terrain == null)
        {
            DebugController.LogError(DebugCategoria.TratamentoMapaCarregado, "Terrain não atribuído.");
            return;
        }

        TerrainData data = terrain.terrainData;
        data.size = new Vector3(limites_mapa_importado.size.x, data.size.y, limites_mapa_importado.size.z);
        terrain.transform.position = new Vector3(limites_mapa_importado.min.x, terrain.transform.position.y, limites_mapa_importado.min.z);

        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, "Terrain ajustado aos limites do mapa.");
    }

    // --------------------------------------------------------------
    public Bounds Bounds_mapa_importado(GameObject mapa)
    {
        Bounds total = new();
        Renderer[] rends = mapa.GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            total = rends[0].bounds;
            foreach (Renderer r in rends) total.Encapsulate(r.bounds);
        }
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Bounds center: {total.center} | size: {total.size}");
        return total;
    }
}

*/

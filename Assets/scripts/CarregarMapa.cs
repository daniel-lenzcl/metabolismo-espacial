using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.AI.Navigation;
using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;
using NavMeshModifier = Unity.AI.Navigation.NavMeshModifier;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarregarMapa : MonoBehaviour
{
    // removidas listas estáticas por tipo (casa/trabalho/restaurante)
    // agora usamos um dicionário dinâmico por nome de camada
    Dictionary<string, List<mPredios>> mCamadas = new Dictionary<string, List<mPredios>>();

    List<cPessoa> lista_das_pessoas;

    public Terrain terrain;
    public levelgenerator levelgenerator_local;

    GameObject mapa;                // raiz do mapa importado (.obj)
    TratamentoMapaCarregado mapa_propriedades;
    List<GameObject> gruposPredios = new List<GameObject>();
    Gerente_de_ambiente Ambiente;

    public NavMeshSurface surface;  // atribuir (ou será criado em runtime)

    private GameObject grupoRuas;
    private GameObject grupoEscala;

    // ===== Working set de clones (apenas para bake) =====
    private GameObject navmeshWorkingRoot; // pai de todos os clones usados só no bake

    const int AREA_CAMINHOS = 4;         // área NavMesh para 'rua'
    static readonly int maskCaminhos = 1 << AREA_CAMINHOS;

    void Start()
    {
        lista_das_pessoas = new List<cPessoa>();
        terrain = FindObjectOfType<Terrain>();
        levelgenerator_local = FindObjectOfType<levelgenerator>();//<Terrain>()?.GetComponent<levelgenerator>();
    }

    public void IniciarCaptura()
    {
        StartCoroutine(captura());
    }

    public IEnumerator captura()
    {
        // carrega referências principais
        Ambiente = GetComponent<Gerente_de_ambiente>();
        mapa_propriedades = Ambiente.GetComponent<TratamentoMapaCarregado>();
        mapa = mapa_propriedades.mapaImportadoRaiz ?? GameObject.Find("MapaImportado");

        // grupos conhecidos
        if (!mapa_propriedades.GruposCamadas.TryGetValue("rua", out grupoRuas))
            Debug.LogWarning("CarregarMapa ▸ grupo 'rua' não encontrado");
        mapa_propriedades.GruposCamadas.TryGetValue("escala", out grupoEscala);

        // todos os grupos que NÃO são rua nem escala → prédios (mantém a lista para bake)
        gruposPredios.Clear();
        gruposPredios = mapa_propriedades.GruposCamadas
            .Where(kv => kv.Key.ToLower() != "rua" && kv.Key.ToLower() != "escala")
            .Select(kv => kv.Value)
            .ToList();

        if (surface == null)
            surface = mapa.GetComponent<NavMeshSurface>() ?? mapa.AddComponent<NavMeshSurface>();

        // Bake completo usando clones e PhysicsColliders
        fazer_navmesh();
        yield return null;
        // Após o bake, constrói/atribui mPredios para que os agentes encontrem endereços
        ConstrucoesNoMapa();
    }

    public void fazer_navmesh()
    {
        SepararLotes();
    }

    private void SepararLotes()
    {
        // 0) Desativa originais (para que o bake não use os mesmos GOs)
        DesabilitarPrediosDuranteGeracaoNavMesh();

        // 0.1) Gera clones temporários apenas para bake (com MeshCollider habilitado)
        CriarClonesParaNavmesh(); // começam inativos

        // 1) Configura a surface para PhysicsColliders
        if (surface == null) surface = mapa.GetComponent<NavMeshSurface>() ?? mapa.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        ConfigureSurfaceGeometry(surface);
        surface.layerMask = ~0; // todas as layers

        // 2) Bake 1/2 – somente ruas
        if (grupoRuas != null) grupoRuas.SetActive(true);
        if (grupoEscala != null) grupoEscala.SetActive(false);
        foreach (var g in gruposPredios) g.SetActive(false);         // originais off
        if (navmeshWorkingRoot != null) navmeshWorkingRoot.SetActive(false); // clones off

        // --- PhysicsColliders precisam de COLLIDER nas ruas: adiciona temporário ---
        var tmpRoadCols = new List<Collider>();
        if (grupoRuas != null) AddTemporaryCollidersForBake(grupoRuas, tmpRoadCols);

        CriarModifier(grupoRuas);    // marca ruas com area 4
        surface.BuildNavMesh();

        // remove colliders temporários das ruas (prédios virão pelos clones)
        RemoveTemporaryColliders(tmpRoadCols);

        // 3) Bake 2/2 – ruas + prédios (clones)
        if (navmeshWorkingRoot != null) navmeshWorkingRoot.SetActive(true);

        foreach (Transform grupoClone in navmeshWorkingRoot.transform)
        {
            foreach (Transform predio in grupoClone)
            {
                // opcional: pequena folga lateral para melhor navegação
                var rend = predio.GetComponent<Renderer>();
                Vector3 centroAntes = rend ? rend.bounds.center : predio.position;
                predio.localScale *= 0.9f; // abre "beiral" entre prédio e rua
                if (rend)
                {
                    Vector3 centroDepois = rend.bounds.center;
                    predio.position += (centroAntes - centroDepois); // mantém o centro
                }

                // estica faces até a rua usando a NavMesh de ruas já pronta
                StretchFacesParaRua(predio.gameObject, 1f, 0f);

                // área de interior para o bake
                CriarModifier(predio.gameObject);
            }
        }

        if (grupoRuas != null) grupoRuas.SetActive(true);
        surface.collectObjects = CollectObjects.Children;
        ConfigureSurfaceGeometry(surface);
        surface.layerMask = ~0;
        surface.BuildNavMesh();

        // 4) Limpeza – descarta clones e reativa originais
        DestruirClonesParaNavmesh();
        ReabilitarPredios();
    }

    // ===== Construção de mPredios e listas para agentes =====
    public void ConstrucoesNoMapa()
    {
        if (Ambiente == null) Ambiente = GetComponent<Gerente_de_ambiente>();
        if (mapa_propriedades == null) mapa_propriedades = Ambiente.GetComponent<TratamentoMapaCarregado>();

        // Limpa dicionário anterior
        mCamadas.Clear();

        // itera dinamicamente todas as camadas agrupadas (exceto 'rua' e 'escala')
        foreach (var kv in mapa_propriedades.GruposCamadas)
        {
            string nomeCamada = kv.Key;
            if (nomeCamada == null) continue;
            string nomeBaixo = nomeCamada.ToLower();
            if (nomeBaixo == "rua" || nomeBaixo == "escala") continue;

            GameObject grupo = kv.Value;
            if (grupo == null) continue;

            // cria lista para esta camada
            var listaMp = new List<mPredios>();
            mCamadas[nomeCamada] = listaMp;

            // inicia coroutine que vai popular a lista com componentes mPredios
            StartCoroutine(Locar_Predios(grupo, listaMp));
        }

        // quando todas as corrotinas estiverem em andamento, finalize
        StartCoroutine(_FinalizarConstrucoes());
    }

    private IEnumerator _FinalizarConstrucoes()
    {
        // aguarda um frame para as corrotinas terminarem (padrão do design atual)
        yield return null;
        if (Ambiente != null)
        {
            if (Ambiente.mlista_dos_predios == null) Ambiente.mlista_dos_predios = new List<mPredios>();
            Ambiente.mlista_dos_predios.Clear();

            // agrega todas as listas dinâmicas
            foreach (var kv in mCamadas)
            {
                var lista = kv.Value;
                if (lista != null && lista.Count > 0)
                    Ambiente.mlista_dos_predios.AddRange(lista);
            }

            // expõe as camadas/dicionário dinâmico para o Gerente_de_ambiente
            Ambiente.mCamadas = mCamadas;

            try { Ambiente.PrediosCarregados(); } catch { }
        }
    }

    public IEnumerator Locar_Predios(GameObject camada, List<mPredios> mlista)
    {
        if (camada == null) yield break;
        for (int i = 0; i < camada.transform.childCount; i++)
        {
            GameObject child = camada.transform.GetChild(i).gameObject;
            var mp = child.GetComponent<mPredios>();
            if (mp == null) mp = child.AddComponent<mPredios>();
            if (mlista != null) mlista.Add(mp);
        }
        yield return null;
    }

    // ===== Helpers de preparação =====
    private void DesabilitarPrediosDuranteGeracaoNavMesh()
    {
        foreach (var predioGrupo in gruposPredios)
        {
            predioGrupo.SetActive(false); // desliga os grupos originais durante o bake

            foreach (Transform predio in predioGrupo.transform)
            {
                var col = predio.GetComponent<Collider>();
                if (col) col.enabled = false;      // desligados para não atrapalhar
                var mr = predio.GetComponent<MeshRenderer>();
                if (mr) mr.enabled = true;         // mantém visível na cena
            }
        }
    }

    private void ReabilitarPredios()
    {
        foreach (var predioGrupo in gruposPredios)
        {
            predioGrupo.SetActive(true);

            foreach (Transform predio in predioGrupo.transform)
            {
                // reabilitar renderer
                var mr = predio.GetComponent<MeshRenderer>();
                if (mr) mr.enabled = true;

                // opcional: manter colliders desligados para não bloquear agentes
                var col = predio.GetComponent<Collider>();
                if (col) col.enabled = false; // deixe true se quiser colisão física com agentes
            }
        }
    }

    private void CriarClonesParaNavmesh()
    {
        DestruirClonesParaNavmesh();

        navmeshWorkingRoot = new GameObject("_WorkingNavmesh");
        navmeshWorkingRoot.transform.SetParent(mapa.transform, false);
        navmeshWorkingRoot.SetActive(false); // ativado só no bake 2

        foreach (var grupo in gruposPredios)
        {
            if (grupo == null) continue;
            var cloneGrupo = new GameObject(grupo.name + "_NM");
            cloneGrupo.transform.SetParent(navmeshWorkingRoot.transform, false);
            cloneGrupo.transform.position = grupo.transform.position;
            cloneGrupo.transform.rotation = grupo.transform.rotation;
            cloneGrupo.transform.localScale = grupo.transform.localScale;

            foreach (Transform child in grupo.transform)
            {
                var src = child.gameObject;
                var go = Instantiate(src, cloneGrupo.transform);

                // mesh própria para deformações locais
                var mf = go.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    mf.mesh = Instantiate(mf.sharedMesh);
                }

                // Renderer do clone pode ficar OFF (não precisamos renderizar)
                var mr = go.GetComponent<MeshRenderer>();
                if (mr) mr.enabled = false;

                // PhysicsColliders: garantir MeshCollider ativo no clone
                var mf2 = go.GetComponent<MeshFilter>();
                if (mf2 != null && mf2.sharedMesh != null)
                {
                    var mc = go.GetComponent<MeshCollider>();
                    if (mc == null) mc = go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf2.sharedMesh;
                    mc.convex = false;
                    mc.enabled = true;
                }
                else
                {
                    var anyCol = go.GetComponent<Collider>();
                    if (anyCol != null) anyCol.enabled = true;
                }

                // marcar área de interior (3) por padrão
                CriarModifier(go);
            }
        }
    }

    private void DestruirClonesParaNavmesh()
    {
        if (navmeshWorkingRoot != null)
        {
            Destroy(navmeshWorkingRoot);
            navmeshWorkingRoot = null;
        }
    }

    /// <summary>
    /// Procura arestas expostas à rua e estica essas faces para encostar na NavMesh.
    /// </summary>
    public static void StretchFacesParaRua(GameObject predio, float maxBusca = 4f, float passo = 0f)
    {
        MeshFilter mf = predio.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Mesh mesh = mf.sharedMesh;
        Vector3[] vLocal = mesh.vertices;
        int[] tris = mesh.triangles;
        Transform tf = predio.transform;

        var arestas = new HashSet<(int, int)>();
        for (int i = 0; i < tris.Length; i += 3)
        {
            int a = tris[i];
            int b = tris[i + 1];
            int c = tris[i + 2];
            AddEdge(a, b); AddEdge(b, c); AddEdge(c, a);
        }

        Dictionary<int, Vector3> deslocPorVert = new Dictionary<int, Vector3>();

        foreach (var (iA, iB) in arestas)
        {
            Vector3 A = tf.TransformPoint(vLocal[iA]);
            Vector3 B = tf.TransformPoint(vLocal[iB]);
            if (Mathf.Abs(A.y - B.y) > 0.01f) continue; // evita telhado

            Vector3 center = (A + B) * 0.5f;
            Vector3 dirAB = (B - A).normalized;
            Vector3 normal2D = new Vector3(-dirAB.z, 0, dirAB.x).normalized;

            bool achou = false;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(center + normal2D * maxBusca, out hit, maxBusca + 0.5f, maskCaminhos))
                achou = true;
            else if (UnityEngine.AI.NavMesh.SamplePosition(center - normal2D * maxBusca, out hit, maxBusca + 0.5f, maskCaminhos))
            { normal2D *= -1; achou = true; }
            if (!achou) continue;

            Vector3 pontoRua = hit.position; pontoRua.y = center.y;
            Vector3 desloc = pontoRua - center + normal2D * passo; desloc.y = 0;
            Acumula(iA, desloc); Acumula(iB, desloc);
        }

        if (deslocPorVert.Count == 0) return;
        Vector3[] vNovoLocal = (Vector3[])vLocal.Clone();
        foreach (var kv in deslocPorVert)
        {
            int idx = kv.Key; Vector3 d = kv.Value;
            vNovoLocal[idx] = tf.InverseTransformPoint(tf.TransformPoint(vLocal[idx]) + d);
        }
        mesh.vertices = vNovoLocal;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        void AddEdge(int x, int y)
        { if (x < y) arestas.Add((x, y)); else arestas.Add((y, x)); }
        void Acumula(int i, Vector3 d)
        { if (deslocPorVert.ContainsKey(i)) deslocPorVert[i] += d; else deslocPorVert[i] = d; }
    }

    /// <summary>
    /// Cria/reaproveita um NavMeshModifier e define a área: rua→4, demais→3.
    /// </summary>
    public void CriarModifier(GameObject go)
    {
        if (go == null) return;
        string nome = go.name.ToLower();
        if (nome.Contains("escala")) return;
        var mod = go.GetComponent<NavMeshModifier>();
        if (mod == null) mod = go.AddComponent<NavMeshModifier>();
        mod.overrideArea = true;
        if (nome.Contains("rua") || nome.Contains("caminho")) mod.area = 4; else mod.area = 3;
    }
    private void ConfigureSurfaceGeometry(NavMeshSurface s)
    {
        try
        {
            var enumType = Type.GetType("Unity.AI.Navigation.CollectGeometry, Unity.AI.Navigation");
            if (enumType == null)
            {
                enumType = Type.GetType("UnityEngine.AI.NavMeshCollectGeometry, UnityEngine.AIModule");
            }
            if (enumType != null)
            {
                var physicsVal = Enum.Parse(enumType, "PhysicsColliders");
                var prop = typeof(NavMeshSurface).GetProperty("useGeometry");
                if (prop != null)
                {
                    prop.SetValue(s, physicsVal, null);
                }
            }
        }
        catch { }
    }
    private void RemoveTemporaryColliders(List<Collider> cols)
    {
        if (cols == null) return;
        foreach (var c in cols)
        {
            if (c == null) continue;
#if UNITY_EDITOR
            if (Application.isPlaying) Destroy(c);
            else DestroyImmediate(c);
#else
            Destroy(c);
#endif
        }
        cols.Clear();
    }

    private void AddTemporaryCollidersForBake(GameObject root, List<Collider> added)
    {
        if (root == null) return;
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            var mf = t.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                var col = t.GetComponent<Collider>();
                if (col == null)
                {
                    var mc = t.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = false;
                    mc.enabled = true;
                    if (added != null) added.Add(mc);
                }
                else
                {
                    // se já existe, garanta que esteja ativo durante o bake
                    col.enabled = true;
                }
            }
        }
    }
}

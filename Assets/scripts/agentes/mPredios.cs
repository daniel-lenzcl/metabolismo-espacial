using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class mPredios : MonoBehaviour
{
    public GameObject predioPreFab;
    public string nomePredio;
    public Vector3 enderecoXYZ;

    public int populacao = 0;
    public int capacidadeGente;
    public float modificador_tipo;

    [Header("Transparência")]
    public bool modo_transparencia = false;
    Toggle toggleTransp;

    [Tooltip("Cria um gatilho (BoxCollider isTrigger) para contar pessoas dentro em tempo real")]
    public bool criarGatilho = true;

    public int agentesDentro;
    float alpha = 0.2f;
    public Color cor;

    Renderer rend;
    Material instancedMat;
    public GameObject meshOrigem;
    public BoxCollider gat;

    [Header("Extrusão / Contorno")]
    [Tooltip("Percentual do raio médio usado para recuar a base (0 = sem recuo)")]
    public float offsetPercentXZ = 0f; // antes fixo em 5%
    [Tooltip("Tolerância (em unidades) para soldar posições XZ ao detectar o contorno externo")]
    public float contornoTolXZ = 0.0001f;

    // Controla se queremos criar um gatilho para contar agentes dentro.
    // Se false (default), NENHUM collider será criado e nada bloqueará a navegação.

    public bool TemVaga() => populacao < capacidadeGente;

    #region Inicializações
    public void inicializar(GameObject tipo, string nome, Vector3 end, int totalGente)
    {
        predioPreFab = tipo;
        nomePredio = nome;
        enderecoXYZ = end;
        capacidadeGente = totalGente;
    }

    public void inicializar(GameObject tipo, string nome, Vector3 end, GameObject meshO)
    {
        predioPreFab = tipo;
        nomePredio = nome;
        enderecoXYZ = end;
        meshOrigem = meshO;
    }

    public void inicializar(GameObject tipo, string nome, Vector3 end)
    {
        predioPreFab = tipo;
        nomePredio = nome;
        enderecoXYZ = end;
    }

    public void inicializar(GameObject tipo, string nome)
    {
        predioPreFab = tipo;
        nomePredio = nome;
        enderecoXYZ = default;
    }

    public void inicializar(int totalGente)
    {
        predioPreFab = gameObject;
        nomePredio = name;
        enderecoXYZ = transform.position;
        capacidadeGente = totalGente;
    }
    #endregion

    void Start()
    {
        // Toggle de transparência (se existir)
        toggleTransp = GameObject.Find("Toggle_modo_transparencia")?.GetComponent<Toggle>();
        if (toggleTransp != null)
        {
            modo_transparencia = toggleTransp.isOn;
            toggleTransp.onValueChanged.AddListener(v => { modo_transparencia = v; AtualizarTransparencia(); });
        }

        // mesh de origem padrão
        if (meshOrigem == null) meshOrigem = gameObject;

        // nome padrão
        if (string.IsNullOrEmpty(nomePredio))
        {
            int idxSibling = meshOrigem.transform.GetSiblingIndex();
            // Corrigido: expressão regular para remover dígitos corretamente
            string nomeLimpo = System.Text.RegularExpressions.Regex.Replace(meshOrigem.name.ToLower(), @"\d", "");
            nomePredio = nomeLimpo + "_" + idxSibling;
        }

        // tipo e cor
        string tipopredio = nomePredio.ToLower();
        if (tipopredio.Contains("casa")) { capacidadeGente = Random.Range(1, 10); modificador_tipo = 10; cor = Color.yellow; }
        else if (tipopredio.Contains("trabalho")) { capacidadeGente = Random.Range(3, 25); modificador_tipo = 5; cor = Color.red; }
        else if (tipopredio.Contains("restaurante")) { capacidadeGente = Random.Range(10, 50); modificador_tipo = 3; cor = Color.green; }

        // endereço base DA MALHA ORIGINAL (antes da extrusão)
        var rMeshStart0 = meshOrigem.GetComponent<Renderer>();
        if (rMeshStart0 != null) enderecoXYZ = rMeshStart0.bounds.center;
        AjustarEnderecoParaNavMesh();

        // cria extrusão se necessário
        if (predioPreFab == null)
        {
            predioPreFab = CriarExtrusaoComOffset();
            predioPreFab.name = nomePredio + "_Extrudido";
        }

        // parent/pose local limpa
        predioPreFab.transform.SetParent(meshOrigem.transform, false);
        predioPreFab.transform.localPosition = Vector3.zero;
        predioPreFab.transform.localRotation = Quaternion.identity;
        predioPreFab.transform.localScale = Vector3.one;

        AjustarDimensoes();
        EnsureTriggerCollider(); // cria BoxCollider isTrigger no objeto visual, não bloqueia navmesh

        // endereço base já foi definido ANTES da extrusão (mantido intocado)

        // material instanciado + transparência
        rend = predioPreFab.GetComponent<Renderer>();
        if (rend != null)
        {
            instancedMat = new Material(rend.sharedMaterial) { enableInstancing = true, color = cor };
            ConfigurarMaterialTransparente(instancedMat);
            rend.material = instancedMat;
            AtualizarTransparencia();
        }
    }

    GameObject CriarExtrusaoComOffset()
    {
        MeshFilter meshFilter = meshOrigem.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var col = cubo.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            return cubo;
        }

        GameObject extrudido = new GameObject();
        MeshFilter newMeshFilter = extrudido.AddComponent<MeshFilter>();
        MeshRenderer newMeshRenderer = extrudido.AddComponent<MeshRenderer>();

        MeshRenderer originalRenderer = meshOrigem.GetComponent<MeshRenderer>();
        if (originalRenderer != null) newMeshRenderer.sharedMaterial = originalRenderer.sharedMaterial;

        Mesh meshOriginal = meshFilter.sharedMesh;
        if (meshOriginal.vertices.Length == 0 || meshOriginal.triangles.Length == 0)
        {
            Object.DestroyImmediate(extrudido);
            GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var col = cubo.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            return cubo;
        }

        try
        {
            Mesh meshExtrudida = CriarMeshExtrudidaComOffset(meshOriginal);
            newMeshFilter.mesh = meshExtrudida;
        }
        catch
        {
            Object.DestroyImmediate(extrudido);
            GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var col = cubo.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            return cubo;
        }

        return extrudido;
    }

    Mesh CriarMeshExtrudidaComOffset(Mesh meshOriginal)
    {
        Vector3[] verticesOriginais = meshOriginal.vertices;
        int[] triangulosOriginais = meshOriginal.triangles;
        Vector2[] uvsOriginais = meshOriginal.uv;

        // centro local na base XZ
        Vector3 centroLocal = Vector3.zero;
        foreach (var v in verticesOriginais) centroLocal += v;
        centroLocal /= Mathf.Max(1, verticesOriginais.Length);

        float distanciaMedia = 0f;
        foreach (var v in verticesOriginais)
        {
            Vector3 d = new Vector3(v.x - centroLocal.x, 0, v.z - centroLocal.z);
            distanciaMedia += d.magnitude;
        }
        distanciaMedia /= Mathf.Max(1, verticesOriginais.Length);

        float offsetDistance = distanciaMedia * Mathf.Max(0f, offsetPercentXZ);

        // aplica offset em XZ para dentro
        Vector3[] verticesComOffset = new Vector3[verticesOriginais.Length];
        for (int i = 0; i < verticesOriginais.Length; i++)
        {
            Vector3 v = verticesOriginais[i];
            Vector3 dir = new Vector3(centroLocal.x - v.x, 0, centroLocal.z - v.z);
            if (dir.sqrMagnitude > 1e-6f) v += dir.normalized * offsetDistance;
            verticesComOffset[i] = new Vector3(v.x, verticesOriginais[i].y, v.z);
        }

        float alturaExtrusao = 1 + capacidadeGente * modificador_tipo;

        int numVerticesOriginais = verticesComOffset.Length;
        Vector3[] todosVertices = new Vector3[numVerticesOriginais * 2];
        for (int i = 0; i < numVerticesOriginais; i++) todosVertices[i] = verticesComOffset[i];
        for (int i = 0; i < numVerticesOriginais; i++) todosVertices[i + numVerticesOriginais] = verticesComOffset[i] + Vector3.up * alturaExtrusao;

        List<int> triangulos = new List<int>();
        // base (invertida)
        for (int i = 0; i < triangulosOriginais.Length; i += 3)
        {
            triangulos.Add(triangulosOriginais[i + 2]);
            triangulos.Add(triangulosOriginais[i + 1]);
            triangulos.Add(triangulosOriginais[i]);
        }
        // topo
        for (int i = 0; i < triangulosOriginais.Length; i += 3)
        {
            triangulos.Add(triangulosOriginais[i] + numVerticesOriginais);
            triangulos.Add(triangulosOriginais[i + 1] + numVerticesOriginais);
            triangulos.Add(triangulosOriginais[i + 2] + numVerticesOriginais);
        }

        // ---- laterais: contorno com orientação (baseia o winding no tri da base) ----
        // Proteção: evita tol extremamente pequeno que produz int gigantes ao arredondar
        float tol = Mathf.Max(1e-3f, contornoTolXZ);
        // mapeia índice original -> representante por posição (XZ)
        Dictionary<string, int> pos2rep = new Dictionary<string, int>();
        int[] repOf = new int[numVerticesOriginais];
        for (int i = 0; i < numVerticesOriginais; i++)
        {
            Vector3 p = todosVertices[i];
            int xi = Mathf.RoundToInt(p.x / tol);
            int zi = Mathf.RoundToInt(p.z / tol);
            string key = xi + "_" + zi;
            if (!pos2rep.TryGetValue(key, out int rep)) { rep = i; pos2rep[key] = rep; }
            repOf[i] = rep;
        }

        // Dedup de triângulos por posições representativas + coleta de arestas com orientação
        HashSet<string> triSet = new HashSet<string>();
        var edges = new Dictionary<string, EdgeInfo>();
        for (int i = 0; i < triangulosOriginais.Length; i += 3)
        {
            int a0 = triangulosOriginais[i];
            int b0 = triangulosOriginais[i + 1];
            int c0 = triangulosOriginais[i + 2];

            int ar = repOf[a0];
            int br = repOf[b0];
            int cr = repOf[c0];

            if (ar == br || br == cr || cr == ar) continue; // degenerado após solda

            // chave do tri representativo (ordenada) para deduplicar
            int ia = ar, ib = br, ic = cr;
            if (ia > ib) { int t = ia; ia = ib; ib = t; }
            if (ib > ic) { int t = ib; ib = ic; ic = t; }
            if (ia > ib) { int t = ia; ia = ib; ib = t; }
            string triKey = ia + "_" + ib + "_" + ic;
            if (triSet.Contains(triKey)) continue;
            triSet.Add(triKey);

            // Conta arestas por par representativo, mas guarda UM par orientado do tri
            string e1 = EdgeKey(ar, br);
            if (!edges.TryGetValue(e1, out EdgeInfo info1)) info1 = new EdgeInfo();
            info1.count++; info1.triIndex = i; info1.a0 = a0; info1.b0 = b0; edges[e1] = info1;

            string e2 = EdgeKey(br, cr);
            if (!edges.TryGetValue(e2, out EdgeInfo info2)) info2 = new EdgeInfo();
            info2.count++; info2.triIndex = i; info2.a0 = b0; info2.b0 = c0; edges[e2] = info2;

            string e3 = EdgeKey(cr, ar);
            if (!edges.TryGetValue(e3, out EdgeInfo info3)) info3 = new EdgeInfo();
            info3.count++; info3.triIndex = i; info3.a0 = c0; info3.b0 = a0; edges[e3] = info3;
        }

        // Gera paredes somente para arestas de contorno (count == 1) com winding correto
        foreach (var kvp in edges)
        {
            var e = kvp.Value;
            if (e.count != 1) continue; // internas têm contagem >=2
            if (e.a0 >= numVerticesOriginais || e.b0 >= numVerticesOriginais) continue;
            AddParedeOrientada(triangulos, e.a0, e.b0, e.triIndex, triangulosOriginais, verticesComOffset, numVerticesOriginais);
        }

        // UVs simples: copia base para topo
        Vector2[] uvs = new Vector2[todosVertices.Length];
        for (int i = 0; i < numVerticesOriginais; i++)
        {
            Vector2 uv = i < uvsOriginais.Length ? uvsOriginais[i] : Vector2.zero;
            uvs[i] = uv;
            uvs[i + numVerticesOriginais] = uv;
        }

        Mesh meshExtrudida = new Mesh();
        meshExtrudida.name = meshOriginal.name + "_Extrudida";
        if (todosVertices.Length > 65535) meshExtrudida.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshExtrudida.vertices = todosVertices;
        meshExtrudida.triangles = triangulos.ToArray();
        meshExtrudida.uv = uvs;
        meshExtrudida.RecalculateNormals();
        meshExtrudida.RecalculateBounds();
        return meshExtrudida;
    }

    // ===== helpers de paredes/orientação =====
    private struct EdgeInfo { public int triIndex; public int a0; public int b0; public int count; }

    private static string EdgeKey(int aRep, int bRep)
    {
        int lo = aRep < bRep ? aRep : bRep;
        int hi = aRep < bRep ? bRep : aRep;
        return lo + "_" + hi;
    }

    private void AddParedeOrientada(List<int> tris, int a, int b, int triIndex, int[] tBase, Vector3[] vBase, int n)
    {
        if (a == b) return;
        if (triIndex < 0 || triIndex + 2 >= tBase.Length) return;
        int i0 = tBase[triIndex];
        int i1 = tBase[triIndex + 1];
        int i2 = tBase[triIndex + 2];

        // normal do tri da BASE (em espaço local)
        Vector3 nTri = Vector3.Cross(vBase[i1] - vBase[i0], vBase[i2] - vBase[i0]).normalized;
        bool baseNormalUp = nTri.y >= 0f;

        int an = a + n;
        int bn = b + n;

        if (baseNormalUp)
        {
            // winding para fora quando base é CCW (vista de cima)
            tris.Add(a); tris.Add(b); tris.Add(an);
            tris.Add(b); tris.Add(bn); tris.Add(an);
        }
        else
        {
            // base invertida (normal -Y): inverta winding
            tris.Add(a); tris.Add(an); tris.Add(b);
            tris.Add(b); tris.Add(an); tris.Add(bn);
        }
    }

    // --- helpers ---

    // Relay para receber eventos do collider do filho (predioPreFab)
    public class TriggerRelay : MonoBehaviour
    {
        public mPredios owner;
        void OnTriggerEnter(Collider other) { if (owner != null) owner.OnTriggerEnter(other); }
        void OnTriggerExit(Collider other) { if (owner != null) owner.OnTriggerExit(other); }
    }
    void ContarAresta(int x, int y, Dictionary<string, int> counts)
    {
        if (x == y) return;
        int a = Mathf.Min(x, y);
        int b = Mathf.Max(x, y);
        string key = a + "_" + b;
        if (counts.TryGetValue(key, out int v)) counts[key] = v + 1; else counts[key] = 1;
    }

    void EnsureTriggerCollider()
    {
        if (predioPreFab == null) return;

        // Sempre usar gatilho (isTrigger) — não bloqueia navegação quando ativado
        if (!criarGatilho)
        {
            // Se existir algum collider, deixa desativado para garantir que não bloqueie
            var cols = predioPreFab.GetComponents<Collider>();
            foreach (var c in cols) c.enabled = false;
            var obst0 = predioPreFab.GetComponent<UnityEngine.AI.NavMeshObstacle>();
            if (obst0 != null) obst0.enabled = false;
            gat = null;
            return;
        }

        // Garante um BoxCollider isTrigger no objeto visual
        var col = predioPreFab.GetComponent<BoxCollider>();
        if (col == null) col = predioPreFab.AddComponent<BoxCollider>();
        col.isTrigger = true;
        gat = col;

        // Ajusta o tamanho/centro do gatilho com base no bounds do renderer
        var rendRef = predioPreFab.GetComponent<Renderer>();
        if (rendRef != null)
        {
            Bounds b = rendRef.bounds;
            gat.center = predioPreFab.transform.InverseTransformPoint(b.center);
            Vector3 lossy = predioPreFab.transform.lossyScale;
            gat.size = new Vector3(
                Mathf.Max(0.01f, b.size.x / Mathf.Max(0.0001f, lossy.x)),
                Mathf.Max(0.01f, b.size.y / Mathf.Max(0.0001f, lossy.y)),
                Mathf.Max(0.01f, b.size.z / Mathf.Max(0.0001f, lossy.z))
            );
        }

        // Desliga qualquer NavMeshObstacle por segurança
        var obstacle = predioPreFab.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (obstacle != null) obstacle.enabled = false;

        // Instala relay para encaminhar eventos do filho (predioPreFab) para este componente
        var relay = predioPreFab.GetComponent<TriggerRelay>();
        if (relay == null) relay = predioPreFab.AddComponent<TriggerRelay>();
        relay.owner = this;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pessoas"))
        {
            agentesDentro++;
            if (modo_transparencia) AtualizarTransparencia();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("pessoas"))
        {
            agentesDentro = Mathf.Max(0, agentesDentro - 1);
            if (modo_transparencia) AtualizarTransparencia();
        }
    }

    public bool AdicionarMorador()
    {
        if (!TemVaga()) return false;
        populacao++;
        AtualizarTransparencia();
        return true;
    }

    public void RemoverMorador()
    {
        populacao = Mathf.Max(0, populacao - 1);
        AtualizarTransparencia();
    }

    void ConfigurarMaterialTransparente(Material mat)
    {
        if (mat == null) return;
        mat.SetFloat("_Mode", 3f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void AtualizarTransparencia()
    {
        if (instancedMat == null || capacidadeGente == 0) return;
        float ratioBase = modo_transparencia ? (float)agentesDentro / Mathf.Max(1, capacidadeGente) : (float)populacao / Mathf.Max(1, capacidadeGente);
        float ratio = Mathf.Clamp01(ratioBase);
        alpha = Mathf.Lerp(0.2f, 1f, ratio);
        Color c = instancedMat.color; c.a = alpha; instancedMat.color = c;
    }

    public void AjustarDimensoes()
    {
        if (predioPreFab == null) return;
        predioPreFab.transform.localPosition = Vector3.zero;
        predioPreFab.transform.localRotation = Quaternion.identity;
        predioPreFab.transform.localScale = Vector3.one;
    }

    void AjustarEnderecoParaNavMesh()
    {
        Renderer rMesh = meshOrigem != null ? meshOrigem.GetComponent<Renderer>() : null;
        float alturaObjeto = 200f;
        if (rMesh != null) alturaObjeto = rMesh.bounds.extents.y;
        if (enderecoXYZ == default && rMesh != null) enderecoXYZ = rMesh.bounds.center;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(enderecoXYZ, out hit, alturaObjeto, UnityEngine.AI.NavMesh.AllAreas))
            enderecoXYZ = hit.position;
        else if (rMesh != null)
            enderecoXYZ = rMesh.bounds.center;
    }
}

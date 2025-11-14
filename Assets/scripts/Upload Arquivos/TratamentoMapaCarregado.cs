using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Linq;

/// <summary>
/// VERSÃO: garante que todas as meshes fiquem voltadas para CIMA (+Y)
/// - Corrige orientação por triângulo (apenas faces horizontais/semelhantes)
/// - Evita inverter paredes/verticais
/// - Preserva UVs/cores; instancia mesh apenas quando necessário
/// </summary>
public class TratamentoMapaCarregado : MonoBehaviour
{
    [HideInInspector]
    public GameObject mapaImportadoRaiz;
    public Terrain terrain;
    public Bounds limites_mapa_importado;

    [Header("Otimizações de Performance")]
    [Tooltip("Processar objetos aos poucos para evitar travamentos")] public bool usarCoroutineParaOrganizacao = true;
    [Tooltip("Quantos objetos processar por frame (menor = mais suave)")] public int objetosPorFrame = 10;
    [Tooltip("Liberar memória automaticamente durante o processamento")] public bool otimizarMemoriaAutomaticamente = true;

    [Header("Debug e Monitoramento")]
    public bool mostrarEstatisticas = true;
    public bool mostrarProgressoDetalhado = false;

    [Header("Configuração de Normais")]
    [Tooltip("Reorientar faces horizontais para +Y. Recomende ON.")]
    public bool corrigirNormais = true;
    [Range(0f, 1f)]
    [Tooltip("Triângulos com |ny| >= limiar são considerados 'horizontais' e podem ser invertidos")]
    public float limiarHorizontal = 0.25f; // 0.25 ~ 14 graus

    // Grupos de camadas acessíveis externamente
    public Dictionary<string, GameObject> GruposCamadas { get; private set; } = new();

    public UnityEvent OnMapaCarregado = new UnityEvent();

    // Stats para debug
    private int totalObjetos = 0;
    private int totalMeshes = 0;
    private int totalVertices = 0;

    void Start()
    {
        terrain = FindObjectOfType<Terrain>();
        corrigirNormais = true; // liga por padrão

        if (mapaImportadoRaiz == null)
        {
            DebugController.LogError(DebugCategoria.TratamentoMapaCarregado, "Mapa importado não atribuído.");
            return;
        }
    }

    /// <summary>Agrupa por nome base e processa mapa.</summary>
    public void InicializarMapaCamadas(string conteudoOBJ = "")
    {
        if (otimizarMemoriaAutomaticamente) LiberarMemoria();
        StartCoroutine(ProcessarMapaCoroutine());
    }

    private IEnumerator ProcessarMapaCoroutine()
    {
        var tempoInicio = System.DateTime.Now;

        if (mostrarEstatisticas) { ColetarEstatisticasOriginais(); yield return null; }
        yield return StartCoroutine(AgruparPorNomeBaseCoroutine());

        if (corrigirNormais) { yield return StartCoroutine(CorrigirNormaisDasMeshesCoroutine()); }

        AjusteEscalaPosicao();
        yield return null;

        if (mostrarEstatisticas)
        {
            var tempoTotal = System.DateTime.Now - tempoInicio;
            MostrarEstatisticasFinais(tempoTotal);
        }

        if (otimizarMemoriaAutomaticamente) LiberarMemoria();
        OnMapaCarregado.Invoke();
    }

    /// <summary>NOVO: Agrupa objetos por nome base (ca, ca_0, ca_1 → camada "ca")</summary>
    /// 
    //
    ///  Explicação das Mudanças
    ///      Cálculo do Volume da Mesh Individual:
    ///          Calculamos o volume de cada mesh individualmente, mantendo um registro da maior mesh encontrada durante o loop.
    ///      Ajuste da Layer da Maior Mesh:
    ///           Se nenhuma camada de rua for encontrada, ajustamos apenas a layer da maior mesh individual, não o grupo inteiro.      

    private IEnumerator AgruparPorNomeBaseCoroutine()
    {
        GruposCamadas = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> grupos = new();
        var contadorPorCamada = new Dictionary<string, int>();

        // Snapshot dos filhos
        List<Transform> filhos = new();
        foreach (Transform child in mapaImportadoRaiz.transform) filhos.Add(child);

        int contador = 0; int objetosProcessados = 0;
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"AGRUPAMENTO: Organizando {filhos.Count} objetos por nome base...");

        // Lista de nomes de camadas que podem ser consideradas como ruas
        string[] nomesDeRua = { "rua", "caminho", "passeio", "estrada", "calçada" };

        // Variáveis para identificar a maior mesh
        GameObject maiorMeshGameObject = null;
        float maiorVolume = 0f;

        foreach (Transform filho in filhos)
        {
            string nomeOriginal = filho.name;
            string nomeBase = nomeOriginal;
            if (nomeOriginal.Contains("_") && char.IsDigit(nomeOriginal.Split('_').Last()[0]))
                nomeBase = nomeOriginal.Substring(0, nomeOriginal.LastIndexOf('_'));

            if (!grupos.ContainsKey(nomeBase))
            {
                GameObject grupo = new GameObject(nomeBase);
                grupo.transform.SetParent(mapaImportadoRaiz.transform);
                grupo.AddComponent<CamadaInfo>().nomeCamada = nomeBase;

                // Verifica se o nome da camada corresponde a um dos nomes de rua
                string layerName = "predios"; // Valor padrão
                foreach (var nomeRua in nomesDeRua)
                {
                    if (nomeBase.ToLower().Contains(nomeRua))
                    {
                        layerName = "caminhos";
                        break;
                    }
                }

                grupo.layer = LayerMask.NameToLayer(layerName);

                grupos[nomeBase] = grupo; GruposCamadas[nomeBase] = grupo; contadorPorCamada[nomeBase] = 0;
            }

            filho.SetParent(grupos[nomeBase].transform);
            filho.gameObject.layer = grupos[nomeBase].layer;
            filho.name = $"{nomeBase}_{contadorPorCamada[nomeBase]}"; contadorPorCamada[nomeBase]++;

            // Calcula o volume da mesh para identificar a maior mesh
            MeshFilter meshFilter = filho.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Bounds bounds = meshFilter.sharedMesh.bounds;
                float volume = bounds.size.x * bounds.size.y * bounds.size.z;
                if (volume > maiorVolume)
                {
                    maiorVolume = volume;
                    maiorMeshGameObject = filho.gameObject;
                }
            }

            contador++; objetosProcessados++;
            if (usarCoroutineParaOrganizacao && contador >= objetosPorFrame)
            {
                contador = 0;
                if (mostrarProgressoDetalhado)
                {
                    float p = (float)objetosProcessados / filhos.Count * 100f;
                    DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Progresso: {p:F1}% ({objetosProcessados}/{filhos.Count})");
                }
                yield return null;
            }
            if (objetosProcessados % 100 == 0 && otimizarMemoriaAutomaticamente)
            { System.GC.Collect(); if (usarCoroutineParaOrganizacao) yield return null; }
        }

        // Verifica se nenhuma camada de rua foi encontrada
        bool encontrouRua = GruposCamadas.Values.Any(g => g.layer == LayerMask.NameToLayer("caminhos"));
        if (!encontrouRua && maiorMeshGameObject != null)
        {
            string maiorMeshNome = maiorMeshGameObject.name;
            string maiorMeshNomeBase = maiorMeshNome;
            if (maiorMeshNome.Contains("_") && char.IsDigit(maiorMeshNome.Split('_').Last()[0]))
                maiorMeshNomeBase = maiorMeshNome.Substring(0, maiorMeshNome.LastIndexOf('_'));

            DebugController.LogWarning(DebugCategoria.TratamentoMapaCarregado, $"Nenhuma camada de rua encontrada. Sugerindo a maior mesh '{maiorMeshNomeBase}' como camada de rua.");

            if (GruposCamadas.TryGetValue(maiorMeshNomeBase, out GameObject grupoMaiorMesh))
            {
                // Ajusta apenas a maior mesh individual, não o grupo inteiro
                maiorMeshGameObject.layer = LayerMask.NameToLayer("caminhos");
            }
        }

        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"AGRUPAMENTO CONCLUÍDO: {grupos.Count} camadas criadas: {string.Join(", ", grupos.Keys)}");
    }



    /// <summary>
    /// Garante normais voltadas para +Y em faces ~horizontais; corrige triângulos invertidos.
    /// Evita mexer em paredes (normais com |ny| pequeno).
    /// </summary>
    private IEnumerator CorrigirNormaisDasMeshesCoroutine()
    {
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, "Ajustando normais para cima (+Y)...");

        int meshesProcessadas = 0, meshesAlteradas = 0;
        MeshFilter[] meshFilters = mapaImportadoRaiz.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter mf in meshFilters)
        {
            var shared = mf.sharedMesh; if (shared == null) continue;
            var verts = shared.vertices; var tris = shared.triangles; if (tris == null || tris.Length < 3) continue;

            // Copia triângulos para possível edição por-face
            bool mudou = false;
            int[] novoTris = (int[])tris.Clone();

            for (int i = 0; i < tris.Length; i += 3)
            {
                int ia = tris[i], ib = tris[i + 1], ic = tris[i + 2];
                Vector3 a = verts[ia], b = verts[ib], c = verts[ic];
                Vector3 nLocal = Vector3.Cross(b - a, c - a); // magnitude ~ 2*área
                Vector3 nWorld = mf.transform.TransformDirection(nLocal);
                float ny = nWorld.y; // componente vertical da normal

                // Só corrigir faces que são significativamente horizontais
                if (Mathf.Abs(ny) >= limiarHorizontal && ny < 0f)
                {
                    // Inverte ordem do triângulo para apontar para +Y
                    novoTris[i] = ia; novoTris[i + 1] = ic; novoTris[i + 2] = ib;
                    mudou = true;
                }
            }

            if (mudou)
            {
                // Criar nova mesh instanciada com triângulos corrigidos
                Mesh nova = new Mesh();
                nova.name = shared.name + "_Up";
                nova.vertices = verts;
                nova.triangles = novoTris;
                if (shared.uv != null && shared.uv.Length > 0) nova.uv = shared.uv;
                if (shared.uv2 != null && shared.uv2.Length > 0) nova.uv2 = shared.uv2;
                if (shared.colors != null && shared.colors.Length > 0) nova.colors = shared.colors;
                nova.RecalculateNormals();
                nova.RecalculateBounds();
                mf.mesh = nova; // instancia local, não afeta outros
                meshesAlteradas++;
            }

            meshesProcessadas++;
            if (meshesProcessadas % objetosPorFrame == 0) yield return null;
        }

        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"NORMAIS: {meshesAlteradas}/{meshesProcessadas} meshes tiveram triângulos invertidos para +Y");
    }

    public void LimparMapaAnterior()
    {
        if (mapaImportadoRaiz != null)
        {
            Destroy(mapaImportadoRaiz);
            mapaImportadoRaiz = null;
            GruposCamadas.Clear();
            totalObjetos = totalMeshes = totalVertices = 0;
            if (otimizarMemoriaAutomaticamente) LiberarMemoria();
            DebugController.Log(DebugCategoria.TratamentoMapaCarregado, "Mapa anterior removido.");
        }
    }

    public void AjusteEscalaPosicao()
    {
        if (GruposCamadas.TryGetValue("escala", out GameObject refEscalaGO))
        {
            float fator = 1f; Renderer refRend = refEscalaGO.GetComponentInChildren<Renderer>();
            if (refRend != null) fator = 2f / refRend.bounds.size.x;
            mapaImportadoRaiz.transform.localScale = Vector3.one * fator;
            AjustarTerrainParaLimites();
        }
        else
        {
            DebugController.LogWarning(DebugCategoria.TratamentoMapaCarregado, "Escala não encontrada — mantendo tamanho original.");
        }
    }

    void AjustarTerrainParaLimites()
    {
        limites_mapa_importado = Bounds_mapa_importado(mapaImportadoRaiz);
        if (terrain == null)
        { DebugController.LogError(DebugCategoria.TratamentoMapaCarregado, "Terrain não atribuído."); return; }

        TerrainData data = terrain.terrainData;
        data.size = new Vector3(limites_mapa_importado.size.x, data.size.y, limites_mapa_importado.size.z);
        terrain.transform.position = new Vector3(limites_mapa_importado.min.x, terrain.transform.position.y, limites_mapa_importado.min.z);
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, "Terrain ajustado aos limites do mapa.");
    }

    public Bounds Bounds_mapa_importado(GameObject mapa)
    {
        Bounds total = new(); Renderer[] rends = mapa.GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            total = rends[0].bounds; foreach (Renderer r in rends) total.Encapsulate(r.bounds);
        }
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Bounds center: {total.center} | size: {total.size}");
        return total;
    }

    private void ColetarEstatisticasOriginais()
    {
        if (!mostrarEstatisticas) return;
        totalObjetos = 0; foreach (Transform child in mapaImportadoRaiz.transform) totalObjetos++;
        MeshFilter[] meshes = mapaImportadoRaiz.GetComponentsInChildren<MeshFilter>();
        totalMeshes = meshes.Length; totalVertices = 0;
        foreach (var mesh in meshes) if (mesh.sharedMesh != null) totalVertices += mesh.sharedMesh.vertexCount;
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Stats: {totalObjetos} objetos, {totalMeshes} meshes, {totalVertices:N0} vértices");
    }

    private void LiberarMemoria()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        if (mostrarProgressoDetalhado)
        {
            long memoria = System.GC.GetTotalMemory(false);
            DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Memória otimizada: {memoria / 1024 / 1024} MB");
        }
    }

    private void MostrarEstatisticasFinais(System.TimeSpan tempo)
    {
        if (!mostrarEstatisticas) return;
        DebugController.Log(DebugCategoria.TratamentoMapaCarregado, $"Processamento concluído em {tempo.TotalSeconds:F2}s - {GruposCamadas.Count} camadas criadas");
    }

    [ContextMenu("Configurar para Arquivo 4MB")]
    public void ConfigurarPara4MB()
    { usarCoroutineParaOrganizacao = true; objetosPorFrame = 10; otimizarMemoriaAutomaticamente = true; mostrarEstatisticas = true; mostrarProgressoDetalhado = false; corrigirNormais = true; }

    [ContextMenu("Configurar para Arquivo Grande")]
    public void ConfigurarParaArquivoGrande()
    { usarCoroutineParaOrganizacao = true; objetosPorFrame = 5; otimizarMemoriaAutomaticamente = true; mostrarEstatisticas = true; mostrarProgressoDetalhado = true; corrigirNormais = true; }

    [ContextMenu("Configurar para Processamento Rápido")]
    public void ConfigurarRapido()
    { usarCoroutineParaOrganizacao = false; objetosPorFrame = 20; otimizarMemoriaAutomaticamente = true; mostrarEstatisticas = true; mostrarProgressoDetalhado = false; corrigirNormais = true; }

    [ContextMenu("Testar Correção de Normais")]
    public void TestarCorrecaoNormais()
    {
        if (mapaImportadoRaiz != null) StartCoroutine(CorrigirNormaisDasMeshesCoroutine());
        else DebugController.LogError(DebugCategoria.TratamentoMapaCarregado, "Nenhum mapa carregado para testar.");
    }
}

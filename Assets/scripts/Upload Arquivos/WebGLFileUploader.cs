
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using Dummiesman;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WebGLFileUploader : MonoBehaviour
{
    public TratamentoMapaCarregado tratamentoMapa;
    public TratamentoMapaCarregado mapa_testando;

    [Header("Configurações de Otimização")]
    [Tooltip("Processar arquivo com coroutines (recomendado para 4MB+)")]
    public bool processarComCoroutines = true;

    [Tooltip("Otimizar memória automaticamente durante carregamento")]
    public bool otimizarMemoriaAutomaticamente = true;

    [Tooltip("Mostrar progresso detalhado no console")]
    public bool mostrarProgressoCarregamento = true;

    [Tooltip("Configurar automaticamente baseado no tamanho do arquivo")]
    public bool configuracaoAutomatica = true;

    [Header("Configurações do OBJ Loader")]
    [Tooltip("Como dividir objetos: Object (padrão), Material, ou None")]
    public SplitMode splitMode = SplitMode.Object;

    [Header("Análise Automática de Arquivo")]
    [Tooltip("Analisar arquivo antes de carregar para evitar problemas")]
    public bool analisarAntesCarregar = true;
    public DetectorTamanhoArquivo detector;

    [Header("Debug")]
    public bool debug = true;

    [Header("Fallback Shader (assign no Inspector se desejar)")]
    [Tooltip("Shader usado quando o shader do material importado não existe no build WebGL")]
    public Shader shaderFallback;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void logMessage(string message);
#else
    private void logMessage(string message)
    {
        Debug.Log(message);
    }
#endif

    public void MostrarNoConsole(string msg)
    {
        logMessage(msg);
    }

    void Start()
    {
        ConfigurarTratamentoMapa();

        if (detector == null && analisarAntesCarregar)
        {
            detector = FindObjectOfType<DetectorTamanhoArquivo>();
            if (detector == null)
            {
                GameObject detectorGO = new GameObject("DetectorTamanhoArquivo");
                detector = detectorGO.AddComponent<DetectorTamanhoArquivo>();
            }
        }
    }

    private void ConfigurarTratamentoMapa()
    {
        if (tratamentoMapa == null)
        {
            tratamentoMapa = GameObject.Find("ambiente")?.GetComponent<TratamentoMapaCarregado>();
        }

        if (tratamentoMapa != null && configuracaoAutomatica)
        {
            tratamentoMapa.usarCoroutineParaOrganizacao = processarComCoroutines;
            tratamentoMapa.otimizarMemoriaAutomaticamente = otimizarMemoriaAutomaticamente;
            tratamentoMapa.mostrarEstatisticas = debug;
            tratamentoMapa.mostrarProgressoDetalhado = debug && mostrarProgressoCarregamento;
            tratamentoMapa.objetosPorFrame = 10;
        }
    }

    public void CarregarMapa()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, "ReceberArquivoOBJ", ".obj");
#elif UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Selecione um arquivo OBJ", "", "obj");
        if (!string.IsNullOrEmpty(path))
        {
            string data = File.ReadAllText(path);
            ReceberArquivoOBJ("data:application/octet-stream;base64," +
                              System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data)));
        }
#endif
    }

    public void ReceberArquivoOBJ(string arquivoBase64)
    {
        DadosDoArquivo(arquivoBase64);

        if (tratamentoMapa == null)
        {
            tratamentoMapa = GameObject.Find("ambiente")?.GetComponent<TratamentoMapaCarregado>();
        }

        if (tratamentoMapa == null)
        {
            Debug.LogError("TratamentoMapaCarregado não encontrado no objeto 'ambiente'!");
            return;
        }

        // Garantir que o shader fallback esteja inicializado aqui, já que Start() pode ter rodado antes do upload
        EnsureFallbackShader();

        if (debug) Debug.Log("Recebendo arquivo OBJ para processamento otimizado...");

        if (processarComCoroutines)
        {
            StartCoroutine(ProcessarArquivoCoroutine(arquivoBase64));
        }
        else
        {
            ProcessarArquivoImediato(arquivoBase64);
        }
    }

    private void EnsureFallbackShader()
    {
        if (shaderFallback != null) return;

        // Tenta localizar shaders comuns disponíveis no build
        shaderFallback = Shader.Find("Standard");
        if (shaderFallback == null)
            shaderFallback = Shader.Find("Unlit/Texture");

        if (shaderFallback != null)
        {
            if (debug) Debug.Log($"Fallback shader definido dinamicamente: {shaderFallback.name}");
        }
        else
        {
            Debug.LogWarning("Fallback shader não encontrado. Adicione o shader em __Edit > Project Settings > Graphics__ > __Always Included Shaders__ ou atribua shaderFallback no Inspector.");
        }
    }

    public void DadosDoArquivo(string arquivo_base)
    {
        // Separar o cabeçalho dos dados
        string[] partes = arquivo_base.Split(',');
        string cabecalho = partes[0];
        string conteudoBase64 = partes[1];

        // Extrair o tipo MIME do cabeçalho
        string tipoMIME = cabecalho.Split(':')[1].Split(';')[0];

        Debug.Log($"Tipo MIME: {tipoMIME}");

        // Converter o conteúdo de base64 para bytes
        byte[] arquivoBytes = Convert.FromBase64String(conteudoBase64);

        // Obter o tamanho do arquivo
        int tamanhoArquivo = arquivoBytes.Length;

        Debug.Log($"Tamanho do arquivo: {tamanhoArquivo} bytes");

        // Aqui você pode processar o arquivo...
    }

    private IEnumerator ProcessarArquivoCoroutine(string arquivoBase64)
    {
        var tempoInicio = System.DateTime.Now;

        if (debug) Debug.Log("=== INICIANDO CARREGAMENTO OTIMIZADO COM COROUTINES ===");

        tratamentoMapa.LimparMapaAnterior();
        yield return new WaitForSeconds(0.1f);

        if (otimizarMemoriaAutomaticamente)
        {
            if (debug) Debug.Log("Otimizando memória antes do carregamento...");
            LiberarMemoria();
            yield return null;
        }

        var resultadoDecodificacao = DecodificarArquivo(arquivoBase64);
        if (!resultadoDecodificacao.sucesso)
        {
            yield break;
        }

        byte[] dados = resultadoDecodificacao.dados;
        string conteudoOBJ = resultadoDecodificacao.conteudoOBJ;

        yield return null;

        if (configuracaoAutomatica)
        {
            ConfigurarAutomaticoPorTamanho(dados.Length);
            yield return null;
        }

        if (dados.Length > 5 * 1024 * 1024)
        {
            if (debug) Debug.Log("Arquivo grande detectado - otimização extra de memória...");
            LiberarMemoriaAgressiva();
            yield return new WaitForSeconds(0.3f);
        }

        // Carrega o OBJ
        var resultadoCarregamento = CarregarOBJ(dados);
        if (!resultadoCarregamento.sucesso)
        {
            LiberarMemoriaAgressiva();
            yield return new WaitForSeconds(1f);

            resultadoCarregamento = CarregarOBJConservador(dados);
            if (!resultadoCarregamento.sucesso)
            {
                yield break;
            }
        }

        yield return null;

        GameObject mapa = resultadoCarregamento.mapa;
        if (mapa != null)
        {
            mapa.name = "MapaImportado";
            tratamentoMapa.mapaImportadoRaiz = mapa;

            // Corrige shaders ausentes antes de qualquer análise subsequente
            CorrigirShaders(mapa);

            if (debug)
            {
                MostrarEstatisticasMapa(mapa);
            }

            tratamentoMapa.InicializarMapaCamadas(conteudoOBJ);

            var tempoTotal = System.DateTime.Now - tempoInicio;
            if (debug) Debug.Log($"🎉 CARREGAMENTO CONCLUÍDO EM {tempoTotal.TotalSeconds:F2} SEGUNDOS 🎉");
        }

        dados = null;
        conteudoOBJ = null;
        if (otimizarMemoriaAutomaticamente)
        {
            LiberarMemoria();
        }
    }

    private struct ResultadoDecodificacao
    {
        public bool sucesso;
        public byte[] dados;
        public string conteudoOBJ;
    }

    private struct ResultadoCarregamento
    {
        public bool sucesso;
        public GameObject mapa;
    }

    private ResultadoDecodificacao DecodificarArquivo(string arquivoBase64)
    {
        try
        {
            if (debug) Debug.Log("Decodificando arquivo base64...");

            string base64Data = arquivoBase64.Substring(arquivoBase64.IndexOf(",") + 1);
            byte[] dados = System.Convert.FromBase64String(base64Data);
            string conteudoOBJ = System.Text.Encoding.UTF8.GetString(dados);

            float tamanhoMB = dados.Length / 1024f / 1024f;
            if (debug) Debug.Log($"Arquivo decodificado: {dados.Length:N0} bytes ({tamanhoMB:F1} MB)");

            if (analisarAntesCarregar && detector != null)
            {
                bool podeCarregar = detector.ValidarAntesCarregar(dados, this);
                if (!podeCarregar)
                {
                    Debug.LogError("❌ Carregamento cancelado após análise");
                    return new ResultadoDecodificacao { sucesso = false };
                }
            }

            return new ResultadoDecodificacao
            {
                sucesso = true,
                dados = dados,
                conteudoOBJ = conteudoOBJ
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao decodificar arquivo: {e.Message}");
            return new ResultadoDecodificacao { sucesso = false };
        }
    }

    private ResultadoCarregamento CarregarOBJ(byte[] dados)
    {
        try
        {
            if (debug) Debug.Log($"Carregando OBJ com SplitMode: {splitMode}...");

            using (MemoryStream stream = new MemoryStream(dados))
            {
                var loader = new OBJLoader();
                loader.SplitMode = splitMode;

                GameObject mapa = loader.Load(stream);

                if (mapa != null)
                {
                    if (debug) Debug.Log($"✅ OBJ carregado com sucesso: {mapa.name}");
                    return new ResultadoCarregamento { sucesso = true, mapa = mapa };
                }
                else
                {
                    Debug.LogError("OBJLoader retornou null - falha ao importar o modelo.");
                    return new ResultadoCarregamento { sucesso = false };
                }
            }
        }
        catch (System.OutOfMemoryException)
        {
            Debug.LogError("⚠️ OutOfMemoryException! Tentando carregamento conservador...");
            return new ResultadoCarregamento { sucesso = false };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao carregar OBJ: {e.ToString()}");
            return new ResultadoCarregamento { sucesso = false };
        }
    }

    private ResultadoCarregamento CarregarOBJConservador(byte[] dados)
    {
        try
        {
            if (debug) Debug.Log("Tentativa conservadora com SplitMode.None...");

            using (MemoryStream stream = new MemoryStream(dados))
            {
                var loader = new OBJLoader();
                loader.SplitMode = SplitMode.None;
                GameObject mapa = loader.Load(stream);

                if (mapa != null)
                {
                    if (debug) Debug.Log("✅ Carregamento conservador bem-sucedido!");
                    return new ResultadoCarregamento { sucesso = true, mapa = mapa };
                }
                else
                {
                    Debug.LogError("Carregamento conservador também falhou.");
                    return new ResultadoCarregamento { sucesso = false };
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Falha mesmo com configurações conservadoras: {e.Message}");
            Debug.LogError("Arquivo muito grande para a memória disponível. Considere:");
            Debug.LogError("1. Usar um arquivo menor");
            Debug.LogError("2. Dividir o modelo em partes");
            Debug.LogError("3. Otimizar o modelo no Blender");
            return new ResultadoCarregamento { sucesso = false };
        }
    }

    private void MostrarEstatisticasMapa(GameObject mapa)
    {
        MeshFilter[] meshes = mapa.GetComponentsInChildren<MeshFilter>();
        int totalVertices = 0;
        foreach (var mesh in meshes)
        {
            if (mesh.sharedMesh != null)
                totalVertices += mesh.sharedMesh.vertexCount;
        }

        Debug.Log($"📊 Mapa carregado: {mapa.transform.childCount} objetos filhos, " +
                 $"{meshes.Length} meshes, {totalVertices:N0} vértices totais");
    }

    private void ProcessarArquivoImediato(string arquivoBase64)
    {
        var tempoInicio = System.DateTime.Now;

        if (debug) Debug.Log("=== PROCESSAMENTO IMEDIATO (pode causar travamento momentâneo) ===");

        tratamentoMapa.LimparMapaAnterior();

        var resultadoDecodificacao = DecodificarArquivo(arquivoBase64);
        if (!resultadoDecodificacao.sucesso)
        {
            return;
        }

        byte[] dados = resultadoDecodificacao.dados;
        string conteudoOBJ = resultadoDecodificacao.conteudoOBJ;

        if (otimizarMemoriaAutomaticamente)
        {
            LiberarMemoria();
        }

        var resultadoCarregamento = CarregarOBJ(dados);
        if (!resultadoCarregamento.sucesso)
        {
            Debug.LogError("❌ Falha no carregamento imediato");
            return;
        }

        GameObject mapa = resultadoCarregamento.mapa;
        if (mapa != null)
        {
            mapa.name = "MapaImportado";
            tratamentoMapa.mapaImportadoRaiz = mapa;

            // Corrige shaders ausentes antes de continuar
            CorrigirShaders(mapa);

            if (debug)
            {
                MostrarEstatisticasMapa(mapa);
            }

            tratamentoMapa.InicializarMapaCamadas(conteudoOBJ);

            var tempoTotal = System.DateTime.Now - tempoInicio;
            if (debug) Debug.Log($"🎉 Processamento concluído em {tempoTotal.TotalSeconds:F2}s");
        }

        if (otimizarMemoriaAutomaticamente)
        {
            System.GC.Collect();
        }
    }

    private void ConfigurarAutomaticoPorTamanho(int tamanhoArquivo)
    {
        float tamanhoMB = tamanhoArquivo / 1024f / 1024f;

        if (debug) Debug.Log($"🔧 Auto-configurando para arquivo de {tamanhoMB:F1} MB...");

        if (tamanhoMB > 10f)
        {
            processarComCoroutines = true;
            splitMode = SplitMode.None;
            tratamentoMapa.usarCoroutineParaOrganizacao = true;
            tratamentoMapa.objetosPorFrame = 5;

            if (debug) Debug.Log("🛡️ Configuração CONSERVADORA aplicada (arquivo >10MB)");
        }
        else if (tamanhoMB > 2f)
        {
            processarComCoroutines = true;
            splitMode = SplitMode.Object;
            tratamentoMapa.usarCoroutineParaOrganizacao = true;
            tratamentoMapa.objetosPorFrame = 10;

            if (debug) Debug.Log("⚙️ Configuração PADRÃO aplicada (arquivo 2-10MB)");
        }
        else
        {
            if (debug) Debug.Log("⚡ Configuração atual mantida (arquivo pequeno <2MB)");
        }

        otimizarMemoriaAutomaticamente = true;
        tratamentoMapa.otimizarMemoriaAutomaticamente = true;
    }

    private void LiberarMemoria()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();

        if (debug && mostrarProgressoCarregamento)
        {
            long memoria = System.GC.GetTotalMemory(false);
            Debug.Log($"🧠 Memória após limpeza: {memoria / 1024 / 1024} MB");
        }
    }

    private void LiberarMemoriaAgressiva()
    {
        if (debug) Debug.Log("🧹 Executando limpeza agressiva de memória...");

        for (int i = 0; i < 3; i++)
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        System.GC.Collect();

        if (debug)
        {
            long memoria = System.GC.GetTotalMemory(false);
            Debug.Log($"🧠 Memória após limpeza agressiva: {memoria / 1024 / 1024} MB");
        }
    }

    // Substitui shaders inválidos/ausentes por um shader fallback para evitar falhas no WebGL
    private void CorrigirShaders(GameObject root)
    {
        if (root == null) return;
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        int corrigidos = 0;
        foreach (var r in renderers)
        {
            var shared = r.sharedMaterials;
            bool altered = false;
            for (int i = 0; i < shared.Length; i++)
            {
                var mat = shared[i];
                if (mat == null) continue;
                var sh = mat.shader;
                bool shaderInvalido = (sh == null) || (sh.name != null && sh.name.IndexOf("InternalError", StringComparison.OrdinalIgnoreCase) >= 0) || !sh.isSupported;
                if (shaderInvalido)
                {
                    // cria novo material com shader fallback e substitui
                    if (shaderFallback != null)
                    {
                        var novo = new Material(shaderFallback);
                        novo.name = mat.name + "_fallback";
                        shared[i] = novo;
                        altered = true;
                    }
                }
            }
            if (altered)
            {
                r.materials = shared; // aplica instâncias para esse renderer
                corrigidos++;
            }
        }

        if (debug) Debug.Log($"Corrigidos shaders em {corrigidos} renderers.");
    }

    [ContextMenu("🔧 Configurar para 4MB")]
    public void ConfigurarPara4MB()
    {
        processarComCoroutines = true;
        splitMode = SplitMode.Object;
        otimizarMemoriaAutomaticamente = true;
        configuracaoAutomatica = true;

        if (tratamentoMapa != null)
        {
            tratamentoMapa.ConfigurarPara4MB();
        }

        Debug.Log("✅ Configurado para arquivo de 4MB!");
    }

    [ContextMenu("🧠 Mostrar Status Memória")]
    public void MostrarStatusMemoria()
    {
        long memoriaAntes = System.GC.GetTotalMemory(false);
        Debug.Log($"=== STATUS DE MEMÓRIA ===");
        Debug.Log($"💾 Memória atual: {memoriaAntes / 1024 / 1024} MB");

        LiberarMemoria();

        long memoriaDepois = System.GC.GetTotalMemory(false);
        Debug.Log($"💾 Memória após limpeza: {memoriaDepois / 1024 / 1024} MB");
        Debug.Log($"♻️ Memória liberada: {(memoriaAntes - memoriaDepois) / 1024 / 1024} MB");

        Debug.Log($"⚙️ Configuração atual:");
        Debug.Log($"   - Coroutines: {processarComCoroutines}");
        Debug.Log($"   - Split Mode: {splitMode}");
        Debug.Log($"   - Otimização Automática: {otimizarMemoriaAutomaticamente}");
        Debug.Log($"   - Configuração Automática: {configuracaoAutomatica}");
    }

    [ContextMenu("🧹 Limpar Memória Agora")]
    public void LiberarMemoriaManual()
    {
        LiberarMemoria();
        Debug.Log("🧹 Memória limpa manualmente!");
    }

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void UploadFile(string gameObject, string methodName, string filter, bool multiple);
}
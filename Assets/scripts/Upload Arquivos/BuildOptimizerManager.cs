using UnityEngine;

public class BuildOptimizerManager : MonoBehaviour
{
    [Header("Configuração Automática")]
    public bool persistirEntreCenas = true;
    public bool configurarAutomaticamente = true;

    [Header("Status")]
    [SerializeField] private bool jaConfigurado = false;
    [SerializeField] private string ambienteDetectado = "";

    private static BuildOptimizerManager instance;
    public static BuildOptimizerManager Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if (persistirEntreCenas)
            {
                // Garantir que passamos um root GameObject para DontDestroyOnLoad
                var rootGO = transform.root != null ? transform.root.gameObject : gameObject;
                DontDestroyOnLoad(rootGO);
            }

            // NÃO executar a configuração automaticamente no Awake.
            // Use TriggerConfiguration() quando quiser configurar (ex.: no início do upload).
            // Se preferir manter execução imediata, descomente a linha abaixo:
            // ConfigurarAutomaticamente();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método público para disparar a configuração sob demanda
    public void TriggerConfiguration()
    {
        ConfigurarAutomaticamente();
    }

    void ConfigurarAutomaticamente()
    {
        if (jaConfigurado) return;

        ambienteDetectado = Application.isEditor ? "Editor" : "Build";

        BuildOptimizer optimizer = GetComponent<BuildOptimizer>();
        if (optimizer == null)
        {
            optimizer = gameObject.AddComponent<BuildOptimizer>();
        }

        if (gameObject.name == "GameObject")
        {
            gameObject.name = "BuildOptimizer_Manager";
        }

        jaConfigurado = true;

        Debug.Log($"🔧 BuildOptimizer configurado automaticamente para {ambienteDetectado}");

        if (configurarAutomaticamente)
        {
            ConfigurarOutrosComponentes();
        }
    }

    void ConfigurarOutrosComponentes()
    {
        ConfigurarWebGLFileUploader();
        ConfigurarTratamentoMapa();
        ConfigurarDetectorTamanho();
    }

    void ConfigurarWebGLFileUploader()
    {
        WebGLFileUploader uploader = FindObjectOfType<WebGLFileUploader>();
        if (uploader != null)
        {
            if (Application.isEditor)
            {
                uploader.debug = true;
                uploader.mostrarProgressoCarregamento = true;
            }
            else
            {
                uploader.debug = false;
                uploader.mostrarProgressoCarregamento = false;
            }

            uploader.otimizarMemoriaAutomaticamente = true;
            uploader.configuracaoAutomatica = true;

            Debug.Log("✅ WebGLFileUploader configurado automaticamente");
        }
    }

    void ConfigurarTratamentoMapa()
    {
        TratamentoMapaCarregado tratamento = FindObjectOfType<TratamentoMapaCarregado>();
        if (tratamento != null)
        {
            if (Application.isEditor)
            {
                tratamento.objetosPorFrame = 8;
                tratamento.mostrarEstatisticas = true;
                tratamento.mostrarProgressoDetalhado = true;
            }
            else
            {
                tratamento.objetosPorFrame = 15;
                tratamento.mostrarEstatisticas = false;
                tratamento.mostrarProgressoDetalhado = false;
            }

            tratamento.usarCoroutineParaOrganizacao = true;
            tratamento.otimizarMemoriaAutomaticamente = true;

            Debug.Log("✅ TratamentoMapaCarregado configurado automaticamente");
        }
    }

    void ConfigurarDetectorTamanho()
    {
        DetectorTamanhoArquivo detector = FindObjectOfType<DetectorTamanhoArquivo>();
        if (detector != null)
        {
            if (Application.isEditor)
            {
                detector.limiteMedioMB = 4;
                detector.limiteGrandeMB = 8;
                detector.mostrarSugestoes = true;
            }
            else
            {
                detector.limiteMedioMB = 6;
                detector.limiteGrandeMB = 12;
                detector.mostrarSugestoes = false;
            }

            Debug.Log("✅ DetectorTamanhoArquivo configurado automaticamente");
        }
    }

    public void ReconfigurarTodos()
    {
        jaConfigurado = false;
        ConfigurarAutomaticamente();
    }

    public bool IsOtimizadoParaProducao()
    {
        return !Application.isEditor;
    }

    [ContextMenu("Mostrar Info do Ambiente")]
    public void MostrarInfoAmbiente()
    {
        Debug.Log("=== INFORMAÇÕES DO AMBIENTE ===");
        Debug.Log($"🔍 Ambiente: {ambienteDetectado}");
        Debug.Log($"📱 Plataforma: {Application.platform}");
        Debug.Log($"🔧 É Editor: {Application.isEditor}");
        Debug.Log($"🚀 É Build: {!Application.isEditor}");
        Debug.Log($"🌐 É WebGL: {Application.platform == RuntimePlatform.WebGLPlayer}");
        Debug.Log($"⚙️ Já configurado: {jaConfigurado}");

        long memoria = System.GC.GetTotalMemory(false);
        Debug.Log($"💾 Memória atual: {memoria / 1024 / 1024} MB");

        if (Application.isEditor)
        {
            Debug.Log("💡 LEMBRETE: Performance será 40-60% melhor no build!");
        }
        else
        {
            Debug.Log("🎉 Rodando em build otimizado!");
        }
    }

    [ContextMenu("Testar Configuração")]
    public void TestarConfiguracao()
    {
        MostrarInfoAmbiente();

        var uploader = FindObjectOfType<WebGLFileUploader>();
        var tratamento = FindObjectOfType<TratamentoMapaCarregado>();
        var detector = FindObjectOfType<DetectorTamanhoArquivo>();

        Debug.Log("=== COMPONENTES ENCONTRADOS ===");
        Debug.Log($"📤 WebGLFileUploader: {(uploader != null ? "✅ Encontrado" : "❌ Não encontrado")}");
        Debug.Log($"🗺️ TratamentoMapaCarregado: {(tratamento != null ? "✅ Encontrado" : "❌ Não encontrado")}");
        Debug.Log($"🔍 DetectorTamanhoArquivo: {(detector != null ? "✅ Encontrado" : "❌ Não encontrado")}");

        if (uploader == null || tratamento == null)
        {
            Debug.LogWarning("⚠️ Alguns componentes essenciais não foram encontrados!");
            Debug.LogWarning("💡 Certifique-se de que estão na mesma cena ou em GameObjects ativos");
        }
        else
        {
            Debug.Log("🎉 Todos os componentes essenciais encontrados!");
        }
    }
}
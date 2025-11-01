using UnityEngine;

public class BuildOptimizer : MonoBehaviour
{
    [Header("Detecção Automática de Ambiente")]
    [SerializeField] private bool isEditor;
    [SerializeField] private bool isBuild;
    [SerializeField] private bool isWebGL;
    [SerializeField] private string plataforma;

    [Header("Configurações por Ambiente")]
    public ConfiguracaoAmbiente configEditor;
    public ConfiguracaoAmbiente configBuild;
    public ConfiguracaoAmbiente configWebGL;

    [System.Serializable]
    public class ConfiguracaoAmbiente
    {
        [Header("Limites de Memória")]
        public int limitePequenoMB = 2;
        public int limiteMedioMB = 5;
        public int limiteGrandeMB = 10;
        public int limiteMaximoMB = 20;

        [Header("Performance")]
        public int objetosPorFrame = 10;
        public bool usarCoroutinesSempre = true;
        public bool otimizacaoAgressiva = false;

        [Header("Debug")]
        public bool mostrarEstatisticas = true;
        public bool mostrarProgressoDetalhado = false;
    }

    void Awake()
    {
        DetectarAmbiente();
        AplicarConfiguracaoOtima();
    }

    void DetectarAmbiente()
    {
        isEditor = Application.isEditor;
        isBuild = !Application.isEditor;
        isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;

        plataforma = Application.platform.ToString();

        if (isEditor)
        {
            DebugController.Log(DebugCategoria.BuildOptimizer, "🔧 Ambiente: Unity Editor (performance reduzida)");
        }
        else if (isWebGL)
        {
            DebugController.Log(DebugCategoria.BuildOptimizer, "🌐 Ambiente: WebGL Build (performance otimizada)");
        }
        else
        {
            DebugController.Log(DebugCategoria.BuildOptimizer, $"💻 Ambiente: {plataforma} Build (performance máxima)");
        }
    }

    public ConfiguracaoAmbiente GetConfiguracaoAtual()
    {
        if (isEditor)
            return configEditor;
        else if (isWebGL)
            return configWebGL;
        else
            return configBuild;
    }

    void AplicarConfiguracaoOtima()
    {
        var config = GetConfiguracaoAtual();

        var tratamento = FindObjectOfType<TratamentoMapaCarregado>();
        if (tratamento != null)
        {
            tratamento.objetosPorFrame = config.objetosPorFrame;
            tratamento.mostrarEstatisticas = config.mostrarEstatisticas;
            tratamento.mostrarProgressoDetalhado = config.mostrarProgressoDetalhado;
            tratamento.otimizarMemoriaAutomaticamente = true;
        }

        var uploader = FindObjectOfType<WebGLFileUploader>();
        if (uploader != null)
        {
            uploader.processarComCoroutines = config.usarCoroutinesSempre;
            uploader.otimizarMemoriaAutomaticamente = true;
            uploader.debug = config.mostrarEstatisticas;
        }

        var detector = FindObjectOfType<DetectorTamanhoArquivo>();
        if (detector != null)
        {
            detector.limitePequenoMB = config.limitePequenoMB;
            detector.limiteMedioMB = config.limiteMedioMB;
            detector.limiteGrandeMB = config.limiteGrandeMB;
        }

        DebugController.Log(DebugCategoria.BuildOptimizer, $"⚙️ Configuração aplicada para {(isEditor ? "Editor" : "Build")}:");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"   • Objetos por frame: {config.objetosPorFrame}");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"   • Limite médio: {config.limiteMedioMB}MB");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"   • Limite grande: {config.limiteGrandeMB}MB");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"   • Limite máximo: {config.limiteMaximoMB}MB");
    }

    void Start()
    {
        if (isEditor)
        {
            ConfigurarParaEditor();
        }
        else if (isWebGL)
        {
            ConfigurarParaWebGL();
        }
        else
        {
            ConfigurarParaBuildNativo();
        }
    }

    void ConfigurarParaEditor()
    {
        configEditor = new ConfiguracaoAmbiente
        {
            limitePequenoMB = 2,
            limiteMedioMB = 4,
            limiteGrandeMB = 8,
            limiteMaximoMB = 15,
            objetosPorFrame = 8,
            usarCoroutinesSempre = true,
            otimizacaoAgressiva = true,
            mostrarEstatisticas = true,
            mostrarProgressoDetalhado = true
        };

        DebugController.Log(DebugCategoria.BuildOptimizer, "🔧 Configuração CONSERVADORA aplicada para Editor");
    }

    void ConfigurarParaWebGL()
    {
        configWebGL = new ConfiguracaoAmbiente
        {
            limitePequenoMB = 3,
            limiteMedioMB = 6,
            limiteGrandeMB = 12,
            limiteMaximoMB = 25,
            objetosPorFrame = 15,
            usarCoroutinesSempre = true,
            otimizacaoAgressiva = false,
            mostrarEstatisticas = false,
            mostrarProgressoDetalhado = false
        };

        DebugController.Log(DebugCategoria.BuildOptimizer, "🌐 Configuração OTIMIZADA aplicada para WebGL Build");
    }

    void ConfigurarParaBuildNativo()
    {
        configBuild = new ConfiguracaoAmbiente
        {
            limitePequenoMB = 5,
            limiteMedioMB = 10,
            limiteGrandeMB = 20,
            limiteMaximoMB = 50,
            objetosPorFrame = 20,
            usarCoroutinesSempre = false,
            otimizacaoAgressiva = false,
            mostrarEstatisticas = false,
            mostrarProgressoDetalhado = false
        };

        DebugController.Log(DebugCategoria.BuildOptimizer, "💻 Configuração MÁXIMA aplicada para Build Nativo");
    }

    [ContextMenu("Mostrar Status Atual")]
    public void MostrarStatus()
    {
        DebugController.Log(DebugCategoria.BuildOptimizer, "=== STATUS DO AMBIENTE ===");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"🔍 Plataforma: {plataforma}");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"📝 É Editor: {isEditor}");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"🚀 É Build: {isBuild}");
        DebugController.Log(DebugCategoria.BuildOptimizer, $"🌐 É WebGL: {isWebGL}");

        long memoria = System.GC.GetTotalMemory(false);
        DebugController.Log(DebugCategoria.BuildOptimizer, $"💾 Memória atual: {memoria / 1024 / 1024}MB");

        var config = GetConfiguracaoAtual();
        DebugController.Log(DebugCategoria.BuildOptimizer, $"⚙️ Limite máximo atual: {config.limiteMaximoMB}MB");

        if (isEditor)
        {
            DebugController.Log(DebugCategoria.BuildOptimizer, "💡 DICA: Performance será 40-60% melhor no build!");
        }
    }
}
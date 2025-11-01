/*

using UnityEngine;

/// <summary>
/// Script opcional para facilitar a configuração dos scripts originais otimizados
/// Adicione este script a qualquer GameObject para ter controles fáceis no Inspector
/// </summary>
public class AssistenteConfiguracaoOBJ : MonoBehaviour
{
    [Header("📋 Configuração Rápida para seu Arquivo")]
    [Space]
    [Tooltip("Clique para configurar automaticamente para arquivo de 4MB")]
    public bool configurarPara4MB = true;

    [Tooltip("Para arquivos pequenos (<1MB) - processamento rápido")]
    public bool configurarRapido = false;

    [Tooltip("Para arquivos muito grandes (>10MB) - processamento conservador")]
    public bool configurarConservador = false;

    [Header("🔍 Encontrar Scripts Automaticamente")]
    [Space]
    public TratamentoMapaCarregado tratamentoMapa;
    public WebGLFileUploader webGLUploader;

    [Header("📊 Informações")]
    [Space]
    [SerializeField] private string statusAtual = "Clique em 'Aplicar Configuração'";
    [SerializeField] private bool scriptsEncontrados = false;

    void Start()
    {
        EncontrarScripts();
        AplicarConfiguracao();
    }

    void EncontrarScripts()
    {
        // Encontrar TratamentoMapaCarregado
        if (tratamentoMapa == null)
        {
            tratamentoMapa = FindObjectOfType<TratamentoMapaCarregado>();
            if (tratamentoMapa == null)
            {
                // Tentar encontrar no GameObject "ambiente"
                GameObject ambiente = GameObject.Find("ambiente");
                if (ambiente != null)
                {
                    tratamentoMapa = ambiente.GetComponent<TratamentoMapaCarregado>();
                }
            }
        }

        // Encontrar WebGLFileUploader
        if (webGLUploader == null)
        {
            webGLUploader = FindObjectOfType<WebGLFileUploader>();
        }

        scriptsEncontrados = (tratamentoMapa != null && webGLUploader != null);

        if (scriptsEncontrados)
        {
            statusAtual = "✅ Scripts encontrados e prontos";
            Debug.Log("✅ AssistenteConfiguracaoOBJ: Scripts encontrados com sucesso!");
        }
        else
        {
            statusAtual = "❌ Scripts não encontrados";
            Debug.LogWarning("⚠️ AssistenteConfiguracaoOBJ: Alguns scripts não foram encontrados.");

            if (tratamentoMapa == null)
                Debug.LogWarning("   - TratamentoMapaCarregado não encontrado");
            if (webGLUploader == null)
                Debug.LogWarning("   - WebGLFileUploader não encontrado");
        }
    }

    public void AplicarConfiguracao()
    {
        if (!scriptsEncontrados)
        {
            EncontrarScripts();
            if (!scriptsEncontrados)
            {
                Debug.LogError("❌ Não é possível aplicar configuração - scripts não encontrados!");
                return;
            }
        }

        if (configurarPara4MB)
        {
            ConfigurarPara4MB();
            statusAtual = "✅ Configurado para 4MB";
        }
        else if (configurarRapido)
        {
            ConfigurarRapido();
            statusAtual = "⚡ Configurado para processamento rápido";
        }
        else if (configurarConservador)
        {
            ConfigurarConservador();
            statusAtual = "🛡️ Configurado para processamento conservador";
        }

        Debug.Log($"🔧 {statusAtual}");
    }

    private void ConfigurarPara4MB()
    {
        // Configurar WebGLFileUploader
        if (webGLUploader != null)
        {
            webGLUploader.processarComCoroutines = true;
            webGLUploader.otimizarMemoriaAutomaticamente = true;
            webGLUploader.mostrarProgressoCarregamento = false; // Não muito verboso
            webGLUploader.configuracaoAutomatica = true;
            webGLUploader.splitMode = Dummiesman.SplitMode.Object;
            webGLUploader.debug = true;
        }

        // Configurar TratamentoMapaCarregado
        if (tratamentoMapa != null)
        {
            tratamentoMapa.usarCoroutineParaOrganizacao = true;
            tratamentoMapa.objetosPorFrame = 10;
            tratamentoMapa.otimizarMemoriaAutomaticamente = true;
            tratamentoMapa.mostrarEstatisticas = true;
            tratamentoMapa.mostrarProgressoDetalhado = false;
        }

        Debug.Log("✅ Configuração para arquivo de 4MB aplicada:");
        Debug.Log("   • Coroutines ativadas");
        Debug.Log("   • 10 objetos por frame");
        Debug.Log("   • Otimização de memória ativa");
        Debug.Log("   • Split por objetos");
    }

    private void ConfigurarRapido()
    {
        // Para arquivos pequenos - processamento mais rápido
        if (webGLUploader != null)
        {
            webGLUploader.processarComCoroutines = false;
            webGLUploader.otimizarMemoriaAutomaticamente = true;
            webGLUploader.mostrarProgressoCarregamento = false;
            webGLUploader.splitMode = Dummiesman.SplitMode.Object;
        }

        if (tratamentoMapa != null)
        {
            tratamentoMapa.usarCoroutineParaOrganizacao = false;
            tratamentoMapa.objetosPorFrame = 20;
            tratamentoMapa.otimizarMemoriaAutomaticamente = true;
            tratamentoMapa.mostrarEstatisticas = true;
            tratamentoMapa.mostrarProgressoDetalhado = false;
        }

        Debug.Log("⚡ Configuração rápida aplicada (arquivos <1MB)");
    }

    private void ConfigurarConservador()
    {
        // Para arquivos grandes - processamento mais conservador
        if (webGLUploader != null)
        {
            webGLUploader.processarComCoroutines = true;
            webGLUploader.otimizarMemoriaAutomaticamente = true;
            webGLUploader.mostrarProgressoCarregamento = true;
            webGLUploader.splitMode = Dummiesman.SplitMode.None; // Menos divisões
        }

        if (tratamentoMapa != null)
        {
            tratamentoMapa.usarCoroutineParaOrganizacao = true;
            tratamentoMapa.objetosPorFrame = 5; // Menos objetos por frame
            tratamentoMapa.otimizarMemoriaAutomaticamente = true;
            tratamentoMapa.mostrarEstatisticas = true;
            tratamentoMapa.mostrarProgressoDetalhado = true;
        }

        Debug.Log("🛡️ Configuração conservadora aplicada (arquivos >10MB)");
    }

    // ========================================
    // MÉTODOS PARA O INSPECTOR
    // ========================================

    [ContextMenu("✅ Aplicar Configuração")]
    public void AplicarConfiguracaoManual()
    {
        AplicarConfiguracao();
    }

    [ContextMenu("🔍 Encontrar Scripts")]
    public void EncontrarScriptsManual()
    {
        EncontrarScripts();
    }

    [ContextMenu("📊 Mostrar Status")]
    public void MostrarStatus()
    {
        Debug.Log("=== STATUS DO ASSISTENTE DE CONFIGURAÇÃO ===");
        Debug.Log($"📋 Status: {statusAtual}");
        Debug.Log($"🔍 Scripts encontrados: {scriptsEncontrados}");
        Debug.Log($"🔧 TratamentoMapaCarregado: {(tratamentoMapa != null ? "✅ Encontrado" : "❌ Não encontrado")}");
        Debug.Log($"🔧 WebGLFileUploader: {(webGLUploader != null ? "✅ Encontrado" : "❌ Não encontrado")}");

        if (scriptsEncontrados)
        {
            Debug.Log("--- Configuração Atual dos Scripts ---");
            if (webGLUploader != null)
            {
                Debug.Log($"WebGL - Coroutines: {webGLUploader.processarComCoroutines}");
                Debug.Log($"WebGL - Split Mode: {webGLUploader.splitMode}");
                Debug.Log($"WebGL - Otimização: {webGLUploader.otimizarMemoriaAutomaticamente}");
            }
            if (tratamentoMapa != null)
            {
                Debug.Log($"Mapa - Coroutines: {tratamentoMapa.usarCoroutineParaOrganizacao}");
                Debug.Log($"Mapa - Objetos/Frame: {tratamentoMapa.objetosPorFrame}");
                Debug.Log($"Mapa - Otimização: {tratamentoMapa.otimizarMemoriaAutomaticamente}");
            }
        }
    }

    [ContextMenu("🧠 Testar Memória")]
    public void TestarMemoria()
    {
        if (webGLUploader != null)
        {
            webGLUploader.MostrarStatusMemoria();
        }
        else
        {
            long memoria = System.GC.GetTotalMemory(false);
            Debug.Log($"💾 Memória atual: {memoria / 1024 / 1024} MB");
        }
    }

    [ContextMenu("🚀 Carregar Arquivo")]
    public void CarregarArquivo()
    {
        if (webGLUploader != null)
        {
            webGLUploader.CarregarMapa();
        }
        else
        {
            Debug.LogError("❌ WebGLFileUploader não encontrado!");
        }
    }

    // ========================================
    // VALIDAÇÃO NO INSPECTOR
    // ========================================

    void OnValidate()
    {
        // Garantir que apenas uma configuração esteja ativa
        int count = 0;
        if (configurarPara4MB) count++;
        if (configurarRapido) count++;
        if (configurarConservador) count++;

        if (count > 1)
        {
            // Manter apenas a última selecionada
            if (configurarConservador)
            {
                configurarPara4MB = false;
                configurarRapido = false;
            }
            else if (configurarRapido)
            {
                configurarPara4MB = false;
                configurarConservador = false;
            }
            else if (configurarPara4MB)
            {
                configurarRapido = false;
                configurarConservador = false;
            }
        }

        // Se nenhuma estiver selecionada, usar 4MB por padrão
        if (count == 0)
        {
            configurarPara4MB = true;
        }

        // Aplicar configuração em runtime se algo mudou
        if (Application.isPlaying && scriptsEncontrados)
        {
            AplicarConfiguracao();
        }
    }
}

*/
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DetectorTamanhoArquivo : MonoBehaviour
{
    [Header("Análise de Arquivo")]
    public bool analisarAntesCarregar = true;
    public bool mostrarSugestoes = true;

    [Header("Limites Recomendados")]
    public int limitePequenoMB = 2;
    public int limiteMedioMB = 5;
    public int limiteGrandeMB = 10;

    [Header("Configurações de Fallback")]
    public bool usarModoConservadorSeGrande = true;

    public struct AnaliseArquivo
    {
        public int tamanhoBytes;
        public float tamanhoMB;
        public int numeroVertices;
        public int numeroFaces;
        public int numeroObjetos;
        public bool objetosDuplicados;
        public float complexidadeEstimada;
        public NivelComplexidade nivel;
        public List<string> sugestoes;
    }

    public enum NivelComplexidade
    {
        Pequeno,
        Medio,
        Grande,
        MuitoGrande
    }

    public AnaliseArquivo AnalisarArquivoOBJ(byte[] dadosArquivo)
    {
        string conteudo = System.Text.Encoding.UTF8.GetString(dadosArquivo);
        return AnalisarConteudoOBJ(conteudo, dadosArquivo.Length);
    }

    private AnaliseArquivo AnalisarConteudoOBJ(string conteudo, int tamanhoBytes)
    {
        var analise = new AnaliseArquivo();
        analise.tamanhoBytes = tamanhoBytes;
        analise.tamanhoMB = tamanhoBytes / 1024f / 1024f;
        analise.sugestoes = new List<string>();

        Dictionary<string, int> objetosUnicos = new Dictionary<string, int>();
        string objetoAtual = "default";

        using (StringReader reader = new StringReader(conteudo))
        {
            string linha;
            while ((linha = reader.ReadLine()) != null)
            {
                linha = linha.Trim();

                if (linha.StartsWith("v "))
                {
                    analise.numeroVertices++;
                }
                else if (linha.StartsWith("f "))
                {
                    analise.numeroFaces++;
                }
                else if (linha.StartsWith("o ") || linha.StartsWith("g "))
                {
                    objetoAtual = linha.Substring(2).Trim();

                    if (objetosUnicos.ContainsKey(objetoAtual))
                    {
                        objetosUnicos[objetoAtual]++;
                        analise.objetosDuplicados = true;
                    }
                    else
                    {
                        objetosUnicos[objetoAtual] = 1;
                    }
                }
            }
        }

        analise.numeroObjetos = objetosUnicos.Count;

        float verticesRAM = analise.numeroVertices * 32f / 1024f / 1024f;
        float facesRAM = analise.numeroFaces * 12f / 1024f / 1024f;
        float overhead = analise.numeroObjetos * 0.1f;

        analise.complexidadeEstimada = verticesRAM + facesRAM + overhead;

        if (analise.complexidadeEstimada <= limitePequenoMB)
            analise.nivel = NivelComplexidade.Pequeno;
        else if (analise.complexidadeEstimada <= limiteMedioMB)
            analise.nivel = NivelComplexidade.Medio;
        else if (analise.complexidadeEstimada <= limiteGrandeMB)
            analise.nivel = NivelComplexidade.Grande;
        else
            analise.nivel = NivelComplexidade.MuitoGrande;

        GerarSugestoes(ref analise, objetosUnicos);

        return analise;
    }

    private void GerarSugestoes(ref AnaliseArquivo analise, Dictionary<string, int> objetosUnicos)
    {
        switch (analise.nivel)
        {
            case NivelComplexidade.Pequeno:
                analise.sugestoes.Add("✅ Arquivo pequeno - carregamento rápido recomendado");
                break;

            case NivelComplexidade.Medio:
                analise.sugestoes.Add("⚙️ Arquivo médio - usar processamento com coroutines");
                break;

            case NivelComplexidade.Grande:
                analise.sugestoes.Add("🛡️ Arquivo grande - usar modo conservador");
                analise.sugestoes.Add("💡 Considere otimizar o modelo no software original");
                break;

            case NivelComplexidade.MuitoGrande:
                analise.sugestoes.Add("⚠️ Arquivo muito grande - risco de OutOfMemoryException");
                analise.sugestoes.Add("🔧 RECOMENDADO: dividir em partes menores");
                break;
        }

        if (analise.objetosDuplicados)
        {
            analise.sugestoes.Add("🔄 Objetos duplicados detectados - considere usar instanciamento");

            foreach (var obj in objetosUnicos)
            {
                if (obj.Value > 1)
                {
                    analise.sugestoes.Add($"   • '{obj.Key}' aparece {obj.Value} vezes");
                }
            }
        }

        if (analise.numeroVertices > 100000)
        {
            analise.sugestoes.Add("📐 Muitos vértices - considere reduzir resolução da malha");
        }

        if (analise.numeroObjetos > 100)
        {
            analise.sugestoes.Add("📦 Muitos objetos - considere combinação de meshes");
        }

        if (analise.nivel >= NivelComplexidade.Grande)
        {
            analise.sugestoes.Add("⚙️ Configurações recomendadas:");
            analise.sugestoes.Add("   • processarComCoroutines = true");
            analise.sugestoes.Add("   • objetosPorFrame = 5");
            analise.sugestoes.Add("   • splitMode = SplitMode.None");
        }
    }

    public void MostrarAnalise(AnaliseArquivo analise)
    {
        Debug.Log("=== ANÁLISE DE ARQUIVO OBJ ===");
        Debug.Log($"📁 Tamanho do arquivo: {analise.tamanhoMB:F1} MB ({analise.tamanhoBytes:N0} bytes)");
        Debug.Log($"🧠 RAM estimada: {analise.complexidadeEstimada:F1} MB");
        Debug.Log($"📊 Vértices: {analise.numeroVertices:N0}");
        Debug.Log($"📊 Faces: {analise.numeroFaces:N0}");
        Debug.Log($"📦 Objetos: {analise.numeroObjetos}");
        Debug.Log($"🔄 Objetos duplicados: {(analise.objetosDuplicados ? "SIM" : "NÃO")}");
        Debug.Log($"⚡ Complexidade: {analise.nivel}");

        Debug.Log("\n💡 SUGESTÕES:");
        foreach (string sugestao in analise.sugestoes)
        {
            Debug.Log($"   {sugestao}");
        }
    }

    public void ConfigurarCarregamentoBaseadoAnalise(AnaliseArquivo analise, WebGLFileUploader uploader)
    {
        switch (analise.nivel)
        {
            case NivelComplexidade.Pequeno:
                uploader.processarComCoroutines = false;
                uploader.splitMode = Dummiesman.SplitMode.Object;
                break;

            case NivelComplexidade.Medio:
                uploader.processarComCoroutines = true;
                uploader.splitMode = Dummiesman.SplitMode.Object;
                break;

            case NivelComplexidade.Grande:
                uploader.processarComCoroutines = true;
                uploader.splitMode = Dummiesman.SplitMode.None;
                break;

            case NivelComplexidade.MuitoGrande:
                uploader.processarComCoroutines = true;
                uploader.splitMode = Dummiesman.SplitMode.None;
                Debug.LogWarning("⚠️ Arquivo muito grande - carregamento pode falhar");
                break;
        }

        uploader.otimizarMemoriaAutomaticamente = true;
        uploader.configuracaoAutomatica = true;
    }

    public bool ValidarAntesCarregar(byte[] dadosArquivo, WebGLFileUploader uploader)
    {
        if (!analisarAntesCarregar) return true;

        var analise = AnalisarArquivoOBJ(dadosArquivo);

        if (mostrarSugestoes)
        {
            MostrarAnalise(analise);
        }

        ConfigurarCarregamentoBaseadoAnalise(analise, uploader);

        if (analise.nivel == NivelComplexidade.MuitoGrande)
        {
            Debug.LogWarning("⚠️ AVISO: Arquivo muito grande!");
            Debug.LogWarning("💡 Recomendação: Use o pipeline do Unity (arraste para Inspector)");

            if (!usarModoConservadorSeGrande)
            {
                Debug.LogError("❌ Carregamento cancelado - arquivo muito grande");
                return false;
            }
        }

        return true;
    }
}
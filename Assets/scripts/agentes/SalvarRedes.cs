using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
/// <summary>
/// Salva matrizes de redes sociais entre agentes.
/// </summary>
public class SalvarRedes : MonoBehaviour
{
    public enum TipoRede { Simples, Detalhada, Personalizada }


    private string encloseIfNeeded(string value, string separador)
    {
        // coloca aspas se o separador aparecer no texto
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Contains(separador) || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            var escaped = value.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }
        return value;
    }

    /// <summary>
    /// Salva uma matriz simples de contatos: agenteA, agenteB, total de encontros.
    /// </summary>
    public void SalvarRedeSimples(Dictionary<string, Dictionary<string, int>> matriz, string pasta)
    {
        DebugController.Log(DebugCategoria.SalvarRedes, $"SalvarRedeSimples ▸ tracking");

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string caminho = Path.Combine(pasta, $"rede_simples_{timestamp}.csv");

        using StreamWriter writer = new StreamWriter(caminho);
        writer.WriteLine("AgenteA,AgenteB,TotalEncontros");

        foreach (var agenteA in matriz)
        {
            foreach (var agenteB in agenteA.Value)
            {
                writer.WriteLine($"{agenteA.Key},{agenteB.Key},{agenteB.Value}");
            }
        }

    }

    /// <summary>
    /// Gera string CSV da matriz simples (sem salvar em disco).
    /// </summary>
    public string GerarStringRedeSimples(Dictionary<string, Dictionary<string, int>> matriz, string separador = ",")
    {
        var sb = new StringBuilder();
        sb.Append("AgenteA").Append(separador).Append("AgenteB").Append(separador).AppendLine("TotalEncontros");

        foreach (var agenteA in matriz)
        {
            foreach (var agenteB in agenteA.Value)
            {
                sb.Append(agenteA.Key).Append(separador)
                  .Append(agenteB.Key).Append(separador)
                  .Append(encloseIfNeeded(agenteB.Value.ToString(), separador))
                  .AppendLine();
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Salva uma matriz detalhada de encontros: agenteA, agenteB, hora, posicao.
    /// </summary>
    public void SalvarRedeDetalhada(Dictionary<string, List<EncontroInfo.Encontro>> redeDetalhada, string pasta)
    {
        DebugController.Log(DebugCategoria.SalvarRedes, $"SalvarRedeDetalhada ▸ tracking");

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string caminho = Path.Combine(pasta, $"rede_detalhada_{timestamp}.csv");

        using StreamWriter writer = new StreamWriter(caminho);
        writer.WriteLine("AgenteA,AgenteB,Dia,Hora,PosicaoX,PosicaoY,PosicaoZ");

        foreach (var par in redeDetalhada)
        {
            string agenteA = par.Key;
            foreach (var encontro in par.Value)
            {
                Vector3 pos = encontro.posicao;
                writer.WriteLine($"{agenteA},{encontro.outro.identidade},{encontro.dia},{encontro.hora},{pos.x},{pos.y},{pos.z}");
            }
        }
    }

    /// <summary>
    /// Salva uma matriz simples personalizada com campos definidos.
    /// </summary>
    public void SalvarRedeCustomizada(Dictionary<string, Dictionary<string, int>> matriz, string pasta, string separador = ",")
    {
        DebugController.Log(DebugCategoria.SalvarRedes, $"SalvarRedeCustomizada ▸ tracking");

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string caminho = Path.Combine(pasta, $"rede_customizada_{timestamp}.csv");

        using StreamWriter writer = new StreamWriter(caminho);
        writer.WriteLine("Origem" + separador + "Destino" + separador + "ForcaLigacao");

        foreach (var origem in matriz)
        {
            foreach (var destino in origem.Value)
            {
                writer.WriteLine(origem.Key + separador + destino.Key + separador + destino.Value);
            }
        }

    }

    /// <summary>
    /// Salva uma matriz binária simétrica de encontros (formato de matriz adjacente).
    /// </summary>
    public void SalvarMatrizBinaria(Dictionary<string, Dictionary<string, int>> matriz, string pasta)
    {
        DebugController.Log(DebugCategoria.SalvarRedes, $"SalvarMatrizBinaria ▸ tracking");

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string caminho = Path.Combine(pasta, $"rede_binaria_{timestamp}.csv");

        var agentes = new List<string>(matriz.Keys);
        agentes.Sort();

        using StreamWriter writer = new StreamWriter(caminho);

        writer.Write(";");
        writer.WriteLine(string.Join(";", agentes));

        foreach (string agenteA in agentes)
        {
            writer.Write(agenteA);
            foreach (string agenteB in agentes)
            {
                int valor = 0;
                if (agenteA == agenteB)
                {
                    valor = 1;
                }
                else if (matriz.ContainsKey(agenteA) && matriz[agenteA].ContainsKey(agenteB))
                {
                    valor = matriz[agenteA][agenteB];
                }
                writer.Write($";{valor}");
            }
            writer.WriteLine();
        }
        DebugController.Log(DebugCategoria.SalvarRedes, $"SalvarMatrizBinaria ▸ salva");

    }

    public string GerarStringMatrizBinaria(Dictionary<string, Dictionary<string, int>> matriz, string separador = ";")
    {
        var agentes = new List<string>(matriz.Keys);
        agentes.Sort();

        var sb = new StringBuilder();
        // cabeçalho
        sb.Append(separador);
        sb.AppendLine(string.Join(separador, agentes));

        // linhas
        foreach (string agenteA in agentes)
        {
            sb.Append(encloseIfNeeded(agenteA, separador));
            foreach (string agenteB in agentes)
            {
                int valor = 0;
                if (agenteA == agenteB)
                {
                    valor = 1;
                }
                else if (matriz.ContainsKey(agenteA) && matriz[agenteA].ContainsKey(agenteB))
                {
                    valor = matriz[agenteA][agenteB];
                }
                sb.Append(separador).Append(valor);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }


    /// <summary>
    /// Gera a matriz simples (AgenteA → AgenteB → total de encontros) a partir dos objetos na cena.
    /// </summary>
    public Dictionary<string, Dictionary<string, int>> GerarMatrizSimplesDeCenas()
    {
        DebugController.Log(DebugCategoria.SalvarRedes, $"GerarMatrizSimplesDeCenas ▸ tracking");

        var matriz = new Dictionary<string, Dictionary<string, int>>();

        GameObject[] agentes = GameObject.FindGameObjectsWithTag("pessoas");
        foreach (var agente in agentes)
        {
            var conect = agente.GetComponent<ParaConectar>();
            if (conect == null || conect.encontrosRegistrados == null) continue;

            string nomeA = conect.minhaPessoa.identidade;

            if (!matriz.ContainsKey(nomeA))
                matriz[nomeA] = new Dictionary<string, int>();

            foreach (var encontro in conect.encontrosRegistrados)
            {
                string nomeB = encontro.outro.identidade;
                if (!matriz[nomeA].ContainsKey(nomeB))
                    matriz[nomeA][nomeB] = 0;

                matriz[nomeA][nomeB] += 1;
            }
        }

        return matriz;
    }
}

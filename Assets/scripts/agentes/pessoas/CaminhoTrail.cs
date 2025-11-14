using System.Collections.Generic;
using UnityEngine;

/// <summary>Desenha o caminho percorrido por um agente usando tempo da simulação.</summary>
public class CaminhoTrail : MonoBehaviour
{
    public enum Duracao { Dia = 1, Semana = 7, Mes = 30 }

    [Header("Parâmetros")]
    public Duracao janela = Duracao.Dia;          // janela visível
    public float distanciaMin = 0.5f;             // só grava ponto se andar isso
    public float largura = 1f;                    // grossura da linha

    [Header("Referência ao relógio da simulação")]
    public horas relogio;                         // arraste o script horas

    LineRenderer lr;
    readonly List<Vector3> pontos = new();
    readonly List<double> tempos = new();         // tempoSimulado (dias + hora/24f)

    // Novo dicionário para armazenar a quantidade de passagens por cada ponto
    private Dictionary<Vector3, int> passagensPorPonto = new();

    void Awake()
    {
        relogio = FindObjectOfType<horas>();// GameObject.Find("Terrain").GetComponent<horas>();

        // Cria um filho para o LineRenderer
        GameObject linhaGO = new GameObject("TrailRendererObj");
        linhaGO.transform.SetParent(this.transform, false); // mantém como filho local

        // Adiciona o LineRenderer no objeto filho
        lr = linhaGO.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = largura;
        lr.numCapVertices = 4;                   // cantos arredondados
        lr.alignment = LineAlignment.View;       // sempre de frente pra câmera
        lr.textureMode = LineTextureMode.Tile;   // repetição de textura
        lr.startColor = lr.endColor = new Color(0f, 1f, 1f, 0.2f); //azul claro //new Color(1f, 1f, 0f, 0.2f);   // vermelho claro

        // Definindo as chaves de cor (color keys)
        GradientColorKey[] colorKeys = new GradientColorKey[1];  // Apenas uma cor (vermelha)
        colorKeys[0].color = lr.endColor;      // Cor vermelha
        colorKeys[0].time = 0f;              // A cor vermelha no início (0%)

        // Definindo as chaves de transparência (alpha keys)
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2]; // Duas chaves para variação de transparência
        alphaKeys[0].alpha = 0.2f;  // Transparente no início
        alphaKeys[0].time = 0f;     // No início (0%)
        alphaKeys[1].alpha = 0.2f;    // Totalmente opaco no final
        alphaKeys[1].time = 1f;     // No final (100%)

        // Definindo o gradiente com as chaves de cor e de transparência
        lr.colorGradient.SetKeys(colorKeys, alphaKeys);

    }

    void Update()
    {
        if (relogio == null || !relogio.rodadia) return;  // pausa quando o “tempo” para

        double tempoAgora = relogio.dias + relogio.hora / 24.0;

        Vector3 posAtual = transform.position;

        // Verifica se é necessário registrar um novo ponto
        if (pontos.Count == 0 || Vector3.Distance(pontos[^1], posAtual) > distanciaMin)
        {
            pontos.Add(posAtual);
            tempos.Add(tempoAgora);

            // Incrementa a quantidade de passagens pelo ponto atual
            if (passagensPorPonto.ContainsKey(posAtual))
            {
                passagensPorPonto[posAtual]++;
            }
            else
            {
                passagensPorPonto[posAtual] = 1;
            }
        }

        // Limpa os pontos antigos que estão fora da janela de tempo
        double janelaDias = (double)janela;
        while (tempos.Count > 0 && tempoAgora - tempos[0] > janelaDias)
        {
            tempos.RemoveAt(0);
            pontos.RemoveAt(0);
        }

        // Defina a quantidade de pontos que serão usados no LineRenderer (todos os pontos registrados)
        lr.positionCount = pontos.Count;  // Agora todos os pontos registrados serão usados

        if (pontos.Count == 0) return;

        // Desenha a linha com os pontos (mas o gradiente já foi configurado no Start)
        for (int i = 0; i < pontos.Count; i++)
        {
            lr.SetPosition(i, pontos[i]);
        }
    }


}

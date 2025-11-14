using System;
using UnityEngine;

[Serializable]
public class TarefaRotina
{
    [Tooltip("Hora inicial inclusiva (0-23)")]
    public int horaInicio;
    [Tooltip("Hora final exclusiva (0-23). Se menor que horaInicio indica wrap pela meia-noite")]
    public int horaFim;
    [Tooltip("Identificador da atividade / camada (ex.: 'casa', 'trabalho', 'restaurante')")]
    public string atividadeId;

    // Opcionais para uso em runtime / preferências
    [Tooltip("Nome preferencial de prédio (opcional)")]
    public string nomePredioPreferido;
    [Tooltip("Duração esperada em horas (opcional, informativo)")]
    public float duracaoEsperadaHoras;
    [Tooltip("Prioridade para resolver conflitos entre tarefas")]
    public int prioridade;

    public TarefaRotina() { }

    public TarefaRotina(int inicio, int fim, string atividade)
    {
        horaInicio = Mathf.Clamp(inicio, 0, 23);
        horaFim = Mathf.Clamp(fim, 0, 23);
        atividadeId = atividade;
    }

    // Verifica se a hora (0-23) está dentro do intervalo desta tarefa
    public bool ContemHora(int hora)
    {
        hora = ((hora % 24) + 24) % 24;
        if (horaInicio <= horaFim)
            return hora >= horaInicio && hora < horaFim;
        // wrap pela meia-noite
        return hora >= horaInicio || hora < horaFim;
    }

    // Verifica se há sobreposição entre duas tarefas (considerando wrap)
    public bool SobrepoeCom(TarefaRotina outra)
    {
        for (int h = 0; h < 24; h++)
        {
            if (this.ContemHora(h) && outra.ContemHora(h)) return true;
        }
        return false;
    }

    public override string ToString()
    {
        return $"[{horaInicio:00}:00 → {horaFim:00}:00] {atividadeId} (pref:{nomePredioPreferido})";
    }
}
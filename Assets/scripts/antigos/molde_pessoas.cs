/*
using System.Collections.Generic;


//SUBSTITUINDO POR TEMPLATEPESSOA
[System.Serializable]
public class molde_pessoas
{
    public string tipo_pessoa;

    // campos legados (mantidos para compatibilidade / edição rápida)
    public string atv_08_10;
    public string atv_10_12;
    public string atv_12_14;
    public string atv_14_16;
    public string atv_16_18;
    public string atv_18_20;
    public string atv_20_08;

    // Nova lista de tarefas (usa TarefaRotina.cs)
    public List<TarefaRotina> tarefas = new List<TarefaRotina>();

    public molde_pessoas()
    {
    }

    public molde_pessoas(string nome)
    {
        tipo_pessoa = nome;
    }

    /// <summary>
    /// Retorna a lista de tarefas (apenas 'tarefas' atualmente).
    /// A conversão automática a partir dos campos legados foi comentada
    /// para evitar comportamento inesperado.
    /// </summary>
    public List<TarefaRotina> GetTarefas()
    {
        if (tarefas != null && tarefas.Count > 0) return tarefas;

        // Conversão legada desativada — mantida como comentário para referência.
        
        var lista = new List<TarefaRotina>();
        AdicionarTarefaFromLegado(lista, atv_08_10, 8, 10);
        AdicionarTarefaFromLegado(lista, atv_10_12, 10, 12);
        AdicionarTarefaFromLegado(lista, atv_12_14, 12, 14);
        AdicionarTarefaFromLegado(lista, atv_14_16, 14, 16);
        AdicionarTarefaFromLegado(lista, atv_16_18, 16, 18);
        AdicionarTarefaFromLegado(lista, atv_18_20, 18, 20);
        AdicionarTarefaFromLegado(lista, atv_20_08, 20, 8); // wrap meia-noite

        // Não modifica 'tarefas' automaticamente — apenas retorna a lista construída.
        // Se preferir persistir a conversão, atribua 'tarefas = lista' no gerente ao carregar.
        return lista;
        

        // Retorna a lista atual (vazia se não houver tarefas definidas).
        return tarefas;
    }

    // Método legada comentado — mantido aqui apenas como referência.
    
    void AdicionarTarefaFromLegado(List<TarefaRotina> lista, string atividade, int inicio, int fim)
    {
        if (string.IsNullOrEmpty(atividade)) return;
        var t = new TarefaRotina(inicio, fim, atividade);
        lista.Add(t);
    }
    
}
*/
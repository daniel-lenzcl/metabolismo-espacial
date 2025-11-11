// ========= Debug System (Opção 3) =============================
using System.Collections.Generic;
using UnityEngine;

public enum DebugCategoria { tracking, TratamentoMapaCarregado, GestorPopulacao, Mapas, 
                             ControleCamera, Horas, 
                             mPredios, cPessoa, Rotina, conectados, ParaConectar, CaminhoTrail,
                             Simulacao, CarregarMapa , GerenteAmbiente, Botoes, Populacao, SalvarRedes,
                             GerenteEncontros, BuildOptimizer
}

public static class DebugController
{
    // Ative / desative aqui as categorias desejadas
    public static HashSet<DebugCategoria> categoriasAtivas = new HashSet<DebugCategoria>
    {
        // ✅ mostre logs de prédios; altere em runtime se quiser
//        DebugCategoria.mPredios,
//        DebugCategoria.conectados,  //esse vai sair, substituido por ParaConectar
//        DebugCategoria.ParaConectar,
//        DebugCategoria.CaminhoTrail,
//        DebugCategoria.GerenteEncontros,
//        DebugCategoria.SalvarRedes,
//        DebugCategoria.tracking,
//        DebugCategoria.TratamentoMapaCarregado,
//        DebugCategoria.GestorPopulacao,
//        DebugCategoria.Mapas,
//        DebugCategoria.ControleCamera,
//        DebugCategoria.Horas,
//        DebugCategoria.cPessoa,
//        DebugCategoria.Rotina,
//        DebugCategoria.CarregarMapa,
//        DebugCategoria.GerenteAmbiente,
        DebugCategoria.Botoes,
//        DebugCategoria.Populacao,
//        DebugCategoria.BuildOptimizer
    };

    public static void Log(DebugCategoria cat, string msg)
    {
        if (categoriasAtivas.Contains(cat))
            Debug.Log($"[{cat}] {msg}");
    }

    public static void LogWarning(DebugCategoria cat, string msg)
    {
        if (categoriasAtivas.Contains(cat))
            Debug.LogWarning($"[{cat}] {msg}");
    }

    public static void LogError(DebugCategoria cat, string msg)
    {
        if (categoriasAtivas.Contains(cat))
            Debug.LogError($"[{cat}] {msg}");
    }
}
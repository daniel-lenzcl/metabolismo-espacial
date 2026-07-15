using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class SlotRotinaUI 
{
    public string Id;
    public string atividadeId; // "dormir", "almocar", "lazer", etc.
    public int inicioMin;      // minutos desde 00:00
    public int fimMin;         // minutos desde 00:00
    public string tipoLugarId; // "casa", "trabalho", etc.

    public SlotRotinaUI(string atividadeId, int inicioMin, int fimMin, string tipoLugarId)
    {
        this.Id = System.Guid.NewGuid().ToString();
        this.atividadeId = atividadeId ?? "";
        this.inicioMin = inicioMin;
        this.fimMin = fimMin;
        this.tipoLugarId = tipoLugarId ?? "";
    }

    public SlotRotinaUI(string Id, string atividadeId, int inicioMin, int fimMin, string tipoLugarId)
    {
        this.Id = string.IsNullOrWhiteSpace(Id) ? System.Guid.NewGuid().ToString() : Id;
        this.atividadeId = atividadeId ?? "";
        this.inicioMin = inicioMin;
        this.fimMin = fimMin;
        this.tipoLugarId = tipoLugarId ?? "";
    }

}

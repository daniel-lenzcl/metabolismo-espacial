using System.Collections.Generic;
using System.Linq;
using System;
//using UnityEditor.PackageManager;

[Serializable]
public struct RotinaSlot
{
    public string Id;
    public string atividadeId;
    public int inicioMin;
    public int fimMin;
    public string tipoLugarId;

    public List<errosSlot> erros;
    public List<avisosSlot> avisos;

    public RotinaSlot(string atividadeId, int inicioMin, int fimMin, string tipoLugarId)
    {
        Id = System.Guid.NewGuid().ToString();
        this.atividadeId = atividadeId;
        this.inicioMin = inicioMin;
        this.fimMin = fimMin;
        this.tipoLugarId = tipoLugarId;
        this.erros = new List<errosSlot>();
        this.avisos = new List<avisosSlot>();
    }

    public RotinaSlot(string Id, string atividadeId, int inicioMin, int fimMin, string tipoLugarId)
    {
        this.Id = Id;
        this.atividadeId = atividadeId;
        this.inicioMin = inicioMin;
        this.fimMin = fimMin;
        this.tipoLugarId = tipoLugarId;
        this.erros = new List<errosSlot>();
        this.avisos = new List<avisosSlot>();
    }

    public RotinaSlot(string id, string atividadeId, int inicioMin, int fimMin, string tipoLugarId, List<errosSlot> erros, List<avisosSlot> avisos)
    {
        this.Id = id;
        this.atividadeId = atividadeId;
        this.inicioMin = inicioMin;
        this.fimMin = fimMin;
        this.tipoLugarId = tipoLugarId;
        this.erros = erros ?? new List<errosSlot>();
        this.avisos = avisos ?? new List<avisosSlot>();
    }


}

public enum errosSlot
{
    slotUiNulo,
    IdVazio,
    atividadeVazia,
    horaInicialInvalida,
    horaFinalInvalida,
    inicioIgualFim,
    sobreposicao
}

public enum avisosSlot
{
    lugarVazio,
    lugarDefault,
    lugarNaoEncontrado
}
public enum AvisosDaRotina
{
    rotina24hIncompleta,
    slotComAviso
}

public enum ErrosDaRotina
{
    rotinaVazia,
    slotComErro
}

public class ResumoDaRotina
{

    public List<ErrosDaRotina> erros = new List<ErrosDaRotina>();
    public List<AvisosDaRotina> avisos = new List<AvisosDaRotina>();

    public List<RotinaSlot> slotsNormalizados = new List<RotinaSlot>();

    public int minutosOcupados;
    public int minutosVazios;

//    public bool TemErros => erros.Count > 0;
//    public bool TemAvisos => avisos.Count > 0;

//    public bool podeSalvarPerfil;
//    public bool podeSimular;
}

[Serializable]
public class RotinaBase
{
    public List<RotinaSlot> slots = new List<RotinaSlot>();
    public ResumoDaRotina resumo = new ResumoDaRotina();

    public RotinaBase()
    {
    }

    public RotinaBase(List<RotinaSlot> slots)
    {
        this.slots = slots;
        this.resumo = new ResumoDaRotina();
    }

    public static bool TryCreateFromUI(
        List<SlotRotinaUI> slotsUI,
        out RotinaBase rotina,
        out string erro
    )
    {

//        rotina = null;
        erro = string.Empty;
        ResumoDaRotina resumoTemporario = new ResumoDaRotina();
        //        rotina.resumo

        List<RotinaSlot> slotsConvertidos = new List<RotinaSlot>();

        if (slotsUI == null || slotsUI.Count == 0)
        {
            //            erro = "A rotina está vazia.";
            //            return false;
            resumoTemporario.erros.Add(ErrosDaRotina.rotinaVazia);

        }
        else
        {
            foreach (var slotUI in slotsUI)
            {
                List<errosSlot> errosTemp = new List<errosSlot>();
                List<avisosSlot> avisosTemp = new List<avisosSlot>();

                if (slotUI == null)
                {
                    resumoTemporario.erros.Add(ErrosDaRotina.slotComErro);
                }
                else
                {
                    string id = slotUI.Id ?? "";
                    string atividade = (slotUI.atividadeId ?? "").Trim().ToLower();
                    string lugar = (slotUI.tipoLugarId ?? "").Trim().ToLower();

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        errosTemp.Add(errosSlot.IdVazio);
                    }

                    if (string.IsNullOrWhiteSpace(atividade))
                    {
                        //                erro = "Existe um slot com tipo de atividade vazio.";
                        //                return false;
                        errosTemp.Add(errosSlot.atividadeVazia);
                    }

                    if (!ValidarMinuto(slotUI.inicioMin))
                    {
                        //                erro = $"Horário inicial inválido: {slotUI.inicioMin}.";
                        //                return false;
//                        slotTemporario.inicioMin = -1;
                        errosTemp.Add(errosSlot.horaInicialInvalida);
                    }

                    if (!ValidarMinuto(slotUI.fimMin))
                    {
                        //                erro = $"Horário final inválido: {slotUI.fimMin}.";
                        //                return false;
//                        slotTemporario.fimMin = -1;
                        errosTemp.Add(errosSlot.horaFinalInvalida);
                    }

                    // evita slot com duração zero
                    if (slotUI.inicioMin == slotUI.fimMin)
                    {
                        //                erro = $"O slot {slotUI.tipoLugarId} tem início e fim iguais ({slotUI.inicioMin}).";
                        //                return false;
                        errosTemp.Add(errosSlot.inicioIgualFim);
                    }

                    if (string.IsNullOrWhiteSpace(lugar))
                    {
                        //                erro = "Existe um slot com tipo de lugar vazio.";
                        //                return false;
                        avisosTemp.Add(avisosSlot.lugarVazio);
                    }

                    slotsConvertidos.Add(new RotinaSlot(
                        id,
                        atividade,
                        slotUI.inicioMin,
                        slotUI.fimMin,
                        lugar,
                        errosTemp,
                        avisosTemp
                    ));
//                    slotsConvertidos.Add(new RotinaSlot(
//                        slotUI.Id,
//                        slotUI.atividadeId.Trim().ToLower(),
//                        slotUI.inicioMin,
//                        slotUI.fimMin,
//                        slotUI.tipoLugarId.Trim().ToLower(),
//                        errosTemp,
//                        avisosTemp
//                    ));
                }


                if (errosTemp.Count > 0)
                {
                    resumoTemporario.erros.Add(ErrosDaRotina.slotComErro);
                }
                if (avisosTemp.Count > 0)
                {
                    resumoTemporario.avisos.Add(AvisosDaRotina.slotComAviso);
                }
            }
        }




        // normaliza
        slotsConvertidos = Normalizar(slotsConvertidos);

        // valida sobreposição após normalização
//        if (!ValidarSemSobreposicao(slotsConvertidos, out erro, resumoTemporario))
//        {
//            return false;
//        }
        ValidarSemSobreposicao(slotsConvertidos, out erro, resumoTemporario);

        rotina = new RotinaBase(slotsConvertidos);
        rotina.resumo = resumoTemporario;
        return true;
    }

    private static bool ValidarMinuto(int minuto)
    {
        return minuto >= 0 && minuto < 1440;
    }

    private static List<RotinaSlot> Normalizar(List<RotinaSlot> slotsOriginais)
    {
        List<RotinaSlot> resultado = new List<RotinaSlot>();

        foreach (var slot in slotsOriginais)
        {
            // slot normal
            if (slot.inicioMin < slot.fimMin)
            {
                resultado.Add(slot);
            }
            // slot que cruza meia-noite, ex: 20:00 -> 08:00
            else
            {
                resultado.Add(new RotinaSlot(slot.Id, slot.atividadeId, slot.inicioMin, 1440, slot.tipoLugarId));
                resultado.Add(new RotinaSlot(slot.Id, slot.atividadeId, 0, slot.fimMin, slot.tipoLugarId));
            }
        }

        return resultado.OrderBy(s => s.inicioMin).ToList();
    }

    private static bool ValidarSemSobreposicao(List<RotinaSlot> slots, out string erro, ResumoDaRotina resumo)
    {
        erro = string.Empty;

        if (slots.Count == 0)
        {
            resumo.avisos.Add(AvisosDaRotina.rotina24hIncompleta);
            erro = "A rotina ficou vazia após normalização.";
            return false;
        }

        for (int i = 0; i < slots.Count - 1; i++)
        {
            var atual = slots[i];
            var proximo = slots[i + 1];

            if (atual.fimMin > proximo.inicioMin)
            {
                atual.erros.Add(errosSlot.sobreposicao);
                proximo.erros.Add(errosSlot.sobreposicao);
                erro =
                    $"Há sobreposição entre os slots '{atual.tipoLugarId}' " +
                    $"({atual.inicioMin}-{atual.fimMin}) e '{proximo.tipoLugarId}' " +
                    $"({proximo.inicioMin}-{proximo.fimMin}).";
                return false;
            }
        }

        return true;
    }
}

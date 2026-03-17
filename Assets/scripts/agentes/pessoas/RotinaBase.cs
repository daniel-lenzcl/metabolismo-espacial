using System.Collections.Generic;
using System.Linq;

public struct RotinaSlot
{
    public int inicioMin;
    public int fimMin;
    public string tipoLugarId;

    public RotinaSlot(int inicioMin, int fimMin, string tipoLugarId)
    {
        this.inicioMin = inicioMin;
        this.fimMin = fimMin;
        this.tipoLugarId = tipoLugarId;
    }
}

public class RotinaBase
{
    public List<RotinaSlot> slots = new List<RotinaSlot>();

    public RotinaBase()
    {
    }

    public RotinaBase(List<RotinaSlot> slots)
    {
        this.slots = slots;
    }

    public static bool TryCreateFromUI(
        List<SlotRotinaUI> slotsUI,
        out RotinaBase rotina,
        out string erro)
    {
        rotina = null;
        erro = string.Empty;

        if (slotsUI == null || slotsUI.Count == 0)
        {
            erro = "A rotina está vazia.";
            return false;
        }

        List<RotinaSlot> slotsConvertidos = new List<RotinaSlot>();

        foreach (var slotUI in slotsUI)
        {
            if (!ValidarMinuto(slotUI.inicioMin))
            {
                erro = $"Horário inicial inválido: {slotUI.inicioMin}.";
                return false;
            }

            if (!ValidarMinuto(slotUI.fimMin))
            {
                erro = $"Horário final inválido: {slotUI.fimMin}.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(slotUI.tipoLugarId))
            {
                erro = "Existe um slot com tipo de lugar vazio.";
                return false;
            }

            // evita slot com duração zero
            if (slotUI.inicioMin == slotUI.fimMin)
            {
                erro = $"O slot {slotUI.tipoLugarId} tem início e fim iguais ({slotUI.inicioMin}).";
                return false;
            }

            slotsConvertidos.Add(new RotinaSlot(
                slotUI.inicioMin,
                slotUI.fimMin,
                slotUI.tipoLugarId.Trim().ToLower()
            ));
        }

        // normaliza
        slotsConvertidos = Normalizar(slotsConvertidos);

        // valida sobreposição após normalização
        if (!ValidarSemSobreposicao(slotsConvertidos, out erro))
        {
            return false;
        }

        rotina = new RotinaBase(slotsConvertidos);
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
                resultado.Add(new RotinaSlot(slot.inicioMin, 1440, slot.tipoLugarId));
                resultado.Add(new RotinaSlot(0, slot.fimMin, slot.tipoLugarId));
            }
        }

        return resultado.OrderBy(s => s.inicioMin).ToList();
    }

    private static bool ValidarSemSobreposicao(List<RotinaSlot> slots, out string erro)
    {
        erro = string.Empty;

        if (slots.Count == 0)
        {
            erro = "A rotina ficou vazia após normalização.";
            return false;
        }

        for (int i = 0; i < slots.Count - 1; i++)
        {
            var atual = slots[i];
            var proximo = slots[i + 1];

            if (atual.fimMin > proximo.inicioMin)
            {
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

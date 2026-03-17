using System.Collections.Generic;
using UnityEngine;

public class FabricaPessoas
{
    public TemplatePessoa CriarTemplate(string tipoId, RotinaBase rotinaBase)
    {
        if (string.IsNullOrWhiteSpace(tipoId))
        {
            Debug.LogError("TipoId inválido ao criar TemplatePessoa.");
            return null;
        }

        if (rotinaBase == null)
        {
            Debug.LogError($"RotinaBase nula ao criar template {tipoId}.");
            return null;
        }

        return new TemplatePessoa(tipoId, rotinaBase);
    }

    public TemplatePessoa CriarTemplateFromUI(string tipoId, List<SlotRotinaUI> slotsUI, out string erro)
    {
        erro = string.Empty;

        if (!RotinaBase.TryCreateFromUI(slotsUI, out RotinaBase rotinaBase, out erro))
        {
            Debug.LogError($"Erro ao criar rotina do tipo {tipoId}: {erro}");
            return null;
        }

        return CriarTemplate(tipoId, rotinaBase);
    }

    public List<TemplatePessoa> CriarTemplatesBase()
    {
        List<TemplatePessoa> templates = new List<TemplatePessoa>();

        TemplatePessoa operario = CriarOperarioBase();
        if (operario != null)
            templates.Add(operario);

        TemplatePessoa cozinheiro = CriarCozinheiroBase();
        if (cozinheiro != null)
            templates.Add(cozinheiro);

        return templates;
    }

    private TemplatePessoa CriarOperarioBase()
    {
        RotinaBase rotinaBase = new RotinaBase
        {
            slots = new List<RotinaSlot>
            {
                new RotinaSlot(0,    120,  "casa"),        // 00:00 - 08:00
                new RotinaSlot(120,  600,  "trabalho"),    // 08:00 - 12:00
                new RotinaSlot(600,  840,  "restaurante"), // 12:00 - 14:00
                new RotinaSlot(840,  1080, "trabalho"),    // 14:00 - 18:00
                new RotinaSlot(1080, 1440, "casa")         // 18:00 - 24:00
            }
        };

        return CriarTemplate("operario", rotinaBase);
    }

    private TemplatePessoa CriarCozinheiroBase()
    {
        RotinaBase rotinaBase = new RotinaBase
        {
            slots = new List<RotinaSlot>
            {
                new RotinaSlot(0,    600,  "casa"),         // 00:00 - 10:00
                new RotinaSlot(600,  1220,  "restaurante"), // 10:00 - 22:00
                new RotinaSlot(1220, 1440, "casa")         // 18:00 - 24:00
            }
        };

        return CriarTemplate("cozinheiro", rotinaBase);
    }
}
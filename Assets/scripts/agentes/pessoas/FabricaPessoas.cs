using System.Collections.Generic;
using UnityEngine;

public class FabricaPessoas
{
    private const string TipoIdOperario =
        "a9833c69-a43c-481f-81df-fbd07e25521a";

    private const string TipoIdCozinheiro =
        "2e52a9c7-c0c2-46a5-8574-b1bb337a5645";

    public TemplatePessoa CriarTemplate(string tipoId, string nomeExibicao, float percentualPopulacao, RotinaBase rotinaBase, bool editavel)
    {
        if (string.IsNullOrWhiteSpace(tipoId))
        {
            Debug.LogError("TipoId inválido ao criar TemplatePessoa.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(nomeExibicao))
        {
            Debug.LogError(
                $"Nome de exibição inválido para {tipoId}.");

            return null;
        }

        if (percentualPopulacao < 0 || percentualPopulacao > 100)
        {
            Debug.LogError(
                $"Percentual inválido para {tipoId}: " +
                $"{percentualPopulacao}%.");
            return null;
        }


        if (rotinaBase == null)
        {
            Debug.LogError($"RotinaBase nula ao criar template {tipoId}.");
            return null;
        }

        return new TemplatePessoa(
            tipoId,
            nomeExibicao,
            percentualPopulacao,
            rotinaBase,
            editavel);
    }

    public TemplatePessoa CriarTemplateFromUI(string tipoId, string nomeExibicao, float percentualPopulacao, List<SlotRotinaUI> slotsUI, out string erro)
    {
        erro = string.Empty;

        if (!RotinaBase.TryCreateFromUI(slotsUI, out RotinaBase rotinaBase, out erro))
        {
            Debug.LogError($"Erro ao criar rotina do tipo {tipoId}: {erro}");
            return null;
        }
       
        if (percentualPopulacao < 0f || percentualPopulacao > 100f)
        {
            Debug.LogError(
                $"Percentual inválido para {tipoId}: " +
                $"{percentualPopulacao:F2}%.");

            return null;
        }

        return CriarTemplate(tipoId, nomeExibicao, percentualPopulacao, rotinaBase, true);
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
                new RotinaSlot("dormir", 0,    120,  "casa"),        // 00:00 - 08:00
                new RotinaSlot("trabalhar", 120,  600,  "trabalho"),    // 08:00 - 12:00
                new RotinaSlot("almocar",600,  840,  "restaurante"), // 12:00 - 14:00
                new RotinaSlot("trabalhar",840,  1080, "trabalho"),    // 14:00 - 18:00
                new RotinaSlot("dormir",1080, 1440, "casa")         // 18:00 - 24:00
            }
        };

        return CriarTemplate(
            TipoIdOperario,
            "Operário",
            50.00f,
            rotinaBase,
            false);
    }

    private TemplatePessoa CriarCozinheiroBase()
    {
        RotinaBase rotinaBase = new RotinaBase
        {
            slots = new List<RotinaSlot>
            {
                new RotinaSlot("dormir", 0,    600,  "casa"),         // 00:00 - 10:00
                new RotinaSlot("trabalhar", 600,  1220,  "restaurante"), // 10:00 - 22:00
                new RotinaSlot("dormir", 1220, 1440, "casa")         // 18:00 - 24:00
            }
        };

        return CriarTemplate(
            TipoIdCozinheiro,
            "Cozinheiro",
            50.00f,
            rotinaBase,
            true);
    }
}

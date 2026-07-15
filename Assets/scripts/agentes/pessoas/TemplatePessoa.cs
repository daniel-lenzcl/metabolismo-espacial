using System;

[Serializable]
public class TemplatePessoa
{
    public string tipoId;
    public string nomeExibicao;
    public float percentualPopulacao;
    public RotinaBase rotinaBase;
    public bool editavel;

    public TemplatePessoa()
    {
    }

    public TemplatePessoa(
        string tipoId,
        string nomeExibicao,
        float percentualPopulacao,
        RotinaBase rotinaBase,
        bool editavel)
    {
        this.tipoId = tipoId;
        this.nomeExibicao = nomeExibicao;
        this.percentualPopulacao =
            ArredondarPercentual(percentualPopulacao);
        this.rotinaBase = rotinaBase;
        this.editavel = editavel;
    }

    private static float ArredondarPercentual(float percentual)
    {
        return (float)Math.Round(percentual, 2);
    }
}
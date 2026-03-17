using System;

[Serializable]
public class TemplatePessoa
{
    public string tipoId;
    public RotinaBase rotinaBase;

    public TemplatePessoa()
    {
    }

    public TemplatePessoa(string tipoId, RotinaBase rotinaBase)
    {
        this.tipoId = tipoId;
        this.rotinaBase = rotinaBase;
    }
}
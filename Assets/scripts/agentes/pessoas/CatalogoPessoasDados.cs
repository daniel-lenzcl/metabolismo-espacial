using System;
using System.Collections.Generic;

[Serializable]
public class CatalogoPessoasDados
{
    public int versao = 1;
    public List<TemplatePessoa> pessoas = new List<TemplatePessoa>();
}

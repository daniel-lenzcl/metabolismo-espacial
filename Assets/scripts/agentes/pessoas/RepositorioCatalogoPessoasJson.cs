using System;
using System.IO;
using UnityEngine;

public class RepositorioCatalogoPessoasJson
{
    private const string NomeArquivo = "catalogo-pessoas.json";

    public string CaminhoArquivo { get; }

    public RepositorioCatalogoPessoasJson(string diretorio = null)
    {
        string diretorioFinal = string.IsNullOrWhiteSpace(diretorio)
            ? Application.persistentDataPath
            : diretorio;

        CaminhoArquivo = Path.Combine(diretorioFinal, NomeArquivo);
    }

    public bool Existe()
    {
        return File.Exists(CaminhoArquivo);
    }

    public bool TentarCarregar(out CatalogoPessoasDados catalogo, out string erro)
    {
        catalogo = null;
        erro = string.Empty;

        if (!Existe())
        {
            erro = "O catálogo de pessoas ainda não existe.";
            return false;
        }

        try
        {
            string json = File.ReadAllText(CaminhoArquivo);

            if (string.IsNullOrWhiteSpace(json))
            {
                erro = "O arquivo do catálogo de pessoas está vazio.";
                return false;
            }

            catalogo = JsonUtility.FromJson<CatalogoPessoasDados>(json);

            if (catalogo == null)
            {
                erro = "Não foi possível interpretar o catálogo de pessoas.";
                return false;
            }

            if (catalogo.pessoas == null)
                catalogo.pessoas = new System.Collections.Generic.List<TemplatePessoa>();

            return true;
        }
        catch (Exception exception)
        {
            erro = $"Falha ao carregar o catálogo de pessoas: {exception.Message}";
            return false;
        }
    }

    public bool TentarSalvar(CatalogoPessoasDados catalogo, out string erro)
    {
        erro = string.Empty;

        if (catalogo == null)
        {
            erro = "O catálogo de pessoas é nulo.";
            return false;
        }

        string caminhoTemporario = CaminhoArquivo + ".tmp";

        try
        {
            string diretorio = Path.GetDirectoryName(CaminhoArquivo);

            if (!string.IsNullOrWhiteSpace(diretorio))
                Directory.CreateDirectory(diretorio);

            string json = JsonUtility.ToJson(catalogo, true);
            File.WriteAllText(caminhoTemporario, json);

            if (File.Exists(CaminhoArquivo))
                File.Delete(CaminhoArquivo);

            File.Move(caminhoTemporario, CaminhoArquivo);
            return true;
        }
        catch (Exception exception)
        {
            erro = $"Falha ao salvar o catálogo de pessoas: {exception.Message}";

            if (File.Exists(caminhoTemporario))
                File.Delete(caminhoTemporario);

            return false;
        }
    }
}

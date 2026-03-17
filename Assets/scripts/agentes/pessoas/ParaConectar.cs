using System.Collections.Generic;
using UnityEngine;

// NOVO: incluir struct
using static EncontroInfo;

public class ParaConectar : MonoBehaviour
{
    // NOVO: lista de encontros com detalhes
    public List<Encontro> encontrosRegistrados = new List<Encontro>();
    GerenteEncontros gerenteEncontros;

    // REMOVIDO: quemEncontrei, quemVi etc.

    public cPessoa minhaPessoa;

    void Start()
    {
        DebugController.Log(DebugCategoria.ParaConectar, $"START ▸ tracking");

        gerenteEncontros = FindObjectOfType<GerenteEncontros>();

        minhaPessoa = GetComponent<Rotina>().pessoa;
        if (minhaPessoa == null)
            DebugController.LogWarning(DebugCategoria.ParaConectar, "Start: Nenhuma referência à 'pessoa'");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("pessoas")) return;

        var outraPessoa = other.GetComponent<Rotina>().pessoa;
        if (outraPessoa == null || minhaPessoa == null) return;

        // EVITA DUPLICIDADE: registra só se este for o menor nome (padroniza)
        string nomeEu = minhaPessoa.identidade;
        string nomeOutro = outraPessoa.identidade;

        if (string.Compare(nomeEu, nomeOutro) > 0) return; // outro já vai registrar

        // OBTER posicao média entre os dois agentes
        Vector3 pos = (transform.position + other.transform.position) * 0.5f;

        // OBTER tempo simulado
        var h = FindObjectOfType<horas>();
        int dia = h?.dia ?? 0;
        int hora = h?.hora ?? 0;
        int min = h?.min ?? 0;

        // NOVO: cria struct e adiciona
        var encontro = new Encontro(outraPessoa, pos, dia, hora, min);
        encontrosRegistrados.Add(encontro);

        gerenteEncontros.RegistrarEncontro(minhaPessoa, outraPessoa, pos);

        DebugController.Log(DebugCategoria.ParaConectar, $"ENCONTRO: {encontro} com {encontro.outro.identidade}");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class cPessoa
{
    public UnityEvent OnCasaDefinida = new UnityEvent();

    public string identidade;
    //    public int idade;
    public GameObject tipoPessoa;
    public GameObject objPessoa;
    public Predios minhaCasa;
    public Predios meuTrabalho;
    public mPredios MminhaCasa;
    public mPredios MmeuTrabalho;
    public mPredios MmeuRestaurante;

    public Gerente_de_ambiente ambiente;
    public gestor_populacao gerente_populacao;
    public GameObject as_pessoas; //prefab do agente
    //    public Rotina rotina;

    public cPessoa(string mnome)
    {
        this.identidade = mnome;
    }
    //    public cPessoa(int id, GameObject tipo, Predios mcasa, Predios mtrab)
    public cPessoa(GameObject prefab, string mnome)
    {
        this.tipoPessoa = prefab;
        this.identidade = mnome;
    }

    public cPessoa(string mnome, GameObject prefab, mPredios mcasa, mPredios mtrab)
    {
        this.tipoPessoa = prefab;
        this.identidade = mnome;
        this.MminhaCasa = mcasa;
        this.MmeuTrabalho = mtrab;
        //        this.idade = idade;
        //        this.tipoPessoa = tipo;
        //        this.enderecoCasa = casa;
    }


    public cPessoa(GameObject prefab, string mnome, Predios mcasa, Predios mtrab)
    {
        this.tipoPessoa = prefab;
        this.identidade = mnome;
        this.minhaCasa = mcasa;
        this.meuTrabalho = mtrab;
//        this.idade = idade;
//        this.tipoPessoa = tipo;
//        this.enderecoCasa = casa;
    }
    public void Inicializar()
    {
        DebugController.Log(DebugCategoria.tracking, $"START/Inicializar ▸ tracking");
        //        rotina = this.gameObject.GetComponent<Rotina>();
        ambiente = GameObject.Find("ambiente").GetComponent<Gerente_de_ambiente>();
        gerente_populacao = GameObject.Find("ambiente").GetComponent<gestor_populacao>();
        as_pessoas = GameObject.Find("as_pessoas");

        OnCasaDefinida.AddListener(InstanciaPessoa);

        if (ambiente.mlista_dos_predios.Count > 0)
        {
            InicializaEnderecos(); // Já carregado
//            OnCasaDefinida.AddListener(() =>
//            {
//                objPessoa.AddComponent<CaminhoTrail>();
//                DebugController.Log(DebugCategoria.cPessoa, $"InstanciaPessoa ▸ OnCasaDefinida acionada");
//            });

        }
        else
        {
            ambiente.OnPrediosCarregados.AddListener(InicializaEnderecos);
        }
        DebugController.Log(DebugCategoria.cPessoa, $"START/Inicializar ▸ ");

        //        Debug.Log("CPESSOA-START");

        //TRAZER PARA CA A INSTANCIA DO OBJETO PESSOA

    }

    public void InstanciaPessoa()
    {

        //        minhaCasa.enderecoXYZ
        DebugController.Log(DebugCategoria.cPessoa,
            $"InstanciaPessoa ▸ pfGente nulo? {(gerente_populacao.pfGente == null)} | " +
            $"minhaCasa nulo? {(MminhaCasa == null)}");
        DebugController.Log(DebugCategoria.cPessoa, $"InstanciaPessoa ▸ pfGente: {gerente_populacao.pfGente.name}");
        DebugController.Log(DebugCategoria.cPessoa, $"minhaCasa: '{MminhaCasa.nomePredio}', coordenada {MminhaCasa.enderecoXYZ}");

        objPessoa = UnityEngine.Object.Instantiate(gerente_populacao.pfGente, MminhaCasa.enderecoXYZ, Quaternion.identity);
        // Ajustar a posição para o ponto válido na NavMesh
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(MminhaCasa.enderecoXYZ, out hit, 10.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            // Atualiza a posição do agente para a posição válida da NavMesh
            Vector3 adjustedPosition = hit.position;

            // Aqui obtemos a altura do prefab do agente para ajustar a posição vertical
            float alturaPrefab = objPessoa.GetComponent<Collider>().bounds.extents.y;

            // Ajustamos a posição para que a base do agente fique no nível correto da NavMesh
            adjustedPosition.y += alturaPrefab;  // Ajusta para a base do prefab (em vez de usar o centro)

            // Atualiza a posição do agente
            objPessoa.transform.position = adjustedPosition;
            Debug.Log($"Instanciado o agente {identidade} na posição válida da NavMesh: {adjustedPosition}");
        }
        else
        {
            // Se não encontrar um ponto válido na NavMesh, usa a posição original (fallback)
            Debug.LogWarning($"Ponto {MminhaCasa.enderecoXYZ} não está acessível na NavMesh. Usando posição fallback.");
        }


        Rotina rotina = objPessoa.AddComponent<Rotina>();
        rotina.pessoa = this;
        objPessoa.name = identidade;
        //        nova.Inicializar();
        objPessoa.transform.parent = as_pessoas.transform;
        objPessoa.tag = "pessoas";
        // ▸ garante Rigidbody kinematic (necessário para triggers dos prédios)
        Rigidbody rb = objPessoa.GetComponent<Rigidbody>();
        if (rb == null) rb = objPessoa.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;

//        objPessoa.AddComponent<conectados>();
        objPessoa.AddComponent<ParaConectar>();
        objPessoa.AddComponent<CaminhoTrail>();
    }

    public void InicializaEnderecos()
    {
        DebugController.Log(DebugCategoria.tracking, $"InicializaEnderecos ▸ tracking");

        //        List<mPredios> casas = ambiente.mlista_dos_predios.Where(p => p.nomePredio.ToLower().Contains("casa")).ToList();
        MminhaCasa = SelecionaPredios(ambiente.mlista_dos_predios.Where(p => p.nomePredio.ToLower().Contains("casa")).ToList());
        MmeuTrabalho = SelecionaPredios(ambiente.mlista_dos_predios.Where(p => p.nomePredio.ToLower().Contains("trabalho")).ToList());
        MmeuRestaurante = SelecionaPredios(ambiente.mlista_dos_predios.Where(p => p.nomePredio.ToLower().Contains("restaurante")).ToList());
        DebugController.Log(DebugCategoria.cPessoa, $"InicializaEnderecos ▸ casa: {MminhaCasa.nomePredio} | trabalho: {MmeuTrabalho} | restaurante: {MmeuRestaurante}");
        OnCasaDefinida.Invoke();

    }

    public mPredios SelecionaPredios(List<mPredios> lista_dos_predios)
    {
        DebugController.Log(DebugCategoria.tracking, $"SelecionaPredios ▸ tracking");

        //        var casas = ambiente.mlista_dos_predios.Where(p => p.nomePredio.ToLower().Contains("casa")).ToList();

        //        Debug.Log("total de predios na lista: " + lista_dos_predios.Count +"; lista de: "+ lista_dos_predios[0].nomePredio);
        DebugController.Log(DebugCategoria.cPessoa, 
            $"SelecionaPredios ▸ total de predios na lista: '{lista_dos_predios.Count}'.");

        mPredios predioEscolhido = null;

        // exemplo: tenta até achar uma casa com vaga
        for (int tentativas = 0; tentativas < lista_dos_predios.Count; tentativas++)
        {
            int idx = Random.Range(0, lista_dos_predios.Count);
            if (lista_dos_predios[idx].TemVaga())
            {
                predioEscolhido = lista_dos_predios[idx];
                break;
            }
        }

        if (predioEscolhido == null)
        {
//            Debug.LogWarning("Nenhuma casa com vaga disponível!");
            DebugController.LogWarning(DebugCategoria.cPessoa, $"SelecionaPredios ▸ Nenhuma casa com vaga disponível!");

            return null;                   // ou trate de outra forma
        }

        // agora registra a pessoa
        bool ok = predioEscolhido.AdicionarMorador();   // só retorna false se alguém ocupou a vaga no mesmo frame
        if (ok)
        {
            DebugController.Log(DebugCategoria.cPessoa,
                $"SelecionaPredios ▸ '{predioEscolhido.nomePredio}' foi escolhido");

            return predioEscolhido;
//            meupredio = predioEscolhido;    // ou o campo equivalente da sua cPessoa
        }

        return null;                   // ou trate de outra forma
        //        Debug.Log("Casas atribuídas: " + casas.Count);


    }

    public void despedida()
    {
        DebugController.Log(DebugCategoria.tracking, $"despedida ▸ tracking");

        MminhaCasa?.RemoverMorador();
        MmeuTrabalho?.RemoverMorador();
        MmeuRestaurante?.RemoverMorador();
        Debug.Log("fui destruido: " + identidade);
        DebugController.LogWarning(DebugCategoria.cPessoa, $"despedida ▸ fui destruido:'{ identidade}'");


    }


}



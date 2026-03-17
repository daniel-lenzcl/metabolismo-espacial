using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GerenteAgentes<TAgente, TTemplate, TFabrica> : MonoBehaviour
{
    protected Gerente_de_ambiente gerenteAmbiente;

    [SerializeField] protected GameObject prefabAgente;

    protected Dictionary<string, TTemplate> colecaoTemplates;
    protected Dictionary<string, int> percentualPorTemplate;
    protected Dictionary<int, TAgente> registroAgentes;

    public IReadOnlyDictionary<string, int> quantidadePorTipo => percentualPorTemplate;

    protected TFabrica fabrica;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DefinirQuantidadePorTemplate(Dictionary<string, int> quantidadeRecebida)
    {
        if (quantidadeRecebida == null)
        {
            Debug.LogError("Quantidade por template recebida é null.");
            return;
        }

        percentualPorTemplate = new Dictionary<string, int>(quantidadeRecebida);

        foreach (KeyValuePair<string, int> item in percentualPorTemplate)
        {
            Debug.Log(item.Key + " : " + item.Value);
        }
    }
}

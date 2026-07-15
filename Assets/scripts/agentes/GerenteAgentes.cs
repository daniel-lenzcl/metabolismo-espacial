using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GerenteAgentes<TAgente, TTemplate, TFabrica> : MonoBehaviour
{
    protected Gerente_de_ambiente gerenteAmbiente;

    [SerializeField] 
    protected GameObject prefabAgente;

    protected Dictionary<string, TTemplate> colecaoTemplates;
    protected Dictionary<int, TAgente> registroAgentes;
    //    protected Dictionary<string, int> percentualPorTemplate;

    protected TFabrica fabrica;

    /*
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
    */
}

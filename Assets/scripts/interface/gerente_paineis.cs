using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gerente_paineis : MonoBehaviour
{
    public GameObject painel;
//    public List<GameObject> gobotoes;
    public List<Button> botoes;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void habilitaPainel()
    {
        foreach (Button b in botoes)
        {
            b.interactable = !b.interactable;
        }

        if (painel.activeSelf == false)
        {
            painel.SetActive(true);
            return;
        } 
        else if (painel.activeSelf == true)
        {
            painel.SetActive(false);
            return;

        }
        //so para novidade

    }

}

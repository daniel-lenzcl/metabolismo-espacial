using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testeBotaoMapa : MonoBehaviour
{
    public GameObject painel;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class botao_dia : MonoBehaviour
{
    public Text nomeBotao;
    public bool comecaDia = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void mudaNome()
    {
        comecaDia = !comecaDia;
        if (comecaDia)
        {
            nomeBotao.text = "dia rolando";
        } else
        {
            nomeBotao.text = "começar dia";
        }
    }
}

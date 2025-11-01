using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class molde_pessoas
{
    public string tipo_pessoa;

    public string atv_08_10;
    public string atv_10_12;
    public string atv_12_14;
    public string atv_14_16;
    public string atv_16_18;
    public string atv_18_20;
    public string atv_20_08;

    public molde_pessoas()
    {
//        tipo_pessoa = nome;
    }

    public molde_pessoas (string nome)
    {
        tipo_pessoa = nome;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

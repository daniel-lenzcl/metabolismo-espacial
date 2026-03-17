using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerenteUI : MonoBehaviour
{
    public event Action OnBotaoPopularClick;

    public void AoClicarBotaoPopular()
    {
        Debug.Log("Botão criar pessoas clicado.");
        OnBotaoPopularClick?.Invoke();
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

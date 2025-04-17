using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cCrianca
{
    public string nomeP;
    public GameObject prefabP;
    public List<Predios> atividades;

    public cCrianca (string nome, GameObject tipoPreFab, List<Predios> atv)
    {
        this.nomeP = nome;
        this.prefabP = tipoPreFab;
        this.atividades = atv;
    }
    // Update is called once per frame
    void Start()
    {
        Predios predioTemp = atividades.Find(x => x.nomePredio.Contains ("casa"));
//            atividades.Contains("casa");
        prefabP.transform.position = predioTemp.enderecoXYZ;
//        Instantiate(predioTemp);  /////////////////////////////////////////////////ta dando erro aki. classe nao pode instanciar sem ser monobehavior?
//////////////////////////////////////////olha como foi a instanciacao das casas no levelgenerator////////////////////
    }

    void Update()
    {
        
    }
}

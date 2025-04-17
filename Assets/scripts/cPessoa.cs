//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class cPessoa
{
    public string identidade;
//    public int idade;
    public GameObject tipoPessoa;
    //    public int capacidade;
    //    public Vector3 enderecoCasa;
    //    public Vector3 enderecoTrabalho;
    //    public Vector3 enderecoBar;
    public Predios minhaCasa;
    public Predios meuTrabalho;

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
    public void Start()
    {
        tipoPessoa.transform.position = minhaCasa.enderecoXYZ;
        Debug.Log("CPESSOA-START: endereco " + tipoPessoa.transform.position);
    }
}



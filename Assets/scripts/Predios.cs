using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Predios 
{
    public GameObject predioPreFab;
    public string nomePredio;
    public Vector3 enderecoXYZ;
    public int capacidadeGente;

    public Predios(GameObject tipo, string nome, Vector3 end, int totalGente)
    {
        this.predioPreFab = tipo;
        this.nomePredio = nome;
        this.enderecoXYZ = end;
        capacidadeGente = totalGente;
    }

    public Predios(GameObject tipo, string nome)
    {
        this.predioPreFab = tipo;
        this.nomePredio = nome;
        //        this.enderecoXYZ = end;
        //        capacidadeGente = totalGente;
    }
}

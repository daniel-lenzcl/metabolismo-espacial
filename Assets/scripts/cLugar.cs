using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lugar
{
    public int identidade;
    public GameObject tipoLugar;
    public int capacidade;
    public Vector3 coordenadas;

    public Lugar ()
       { 
       }

    public Lugar (int id, GameObject tipo, int cap, Vector3 coord)
    {
        this.identidade = id;
        this.tipoLugar = tipo;
        this.capacidade = cap;
        this.coordenadas = coord;
    }
}



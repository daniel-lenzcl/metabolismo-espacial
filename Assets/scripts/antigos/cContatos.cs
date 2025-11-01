using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class cContatos
{
    public GameObject contato;
    public  float horacont;
//    public List<string> contato;
//    public List<float> vezesEncontro;


    public cContatos()
    {
    }

    public cContatos(GameObject quem, float time)
    {
        this.contato = quem;
//        this.horacont += time;
        this.horacont = time;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class casa : MonoBehaviour
{

    public GameObject pessoa;
    public int maxPessoas;
    public GameObject crianca;
    public int numberOfCriancas;
    public int nomepessoa;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("CASA-START");
        GameObject geral = GameObject.Find("Terrain");
        //   int tpop = geral.GetComponent<populacao>().populacaoTotal;
        // int criancas = tpop * GetComponent<populacao>().p100crianca / 100;
        // int adultos = tpop - criancas;
        //        Debug.Log("populacao total: " + tpop );
        //        Debug.Log("populacao total: " + tpop + " criancas: " + criancas);

        maxPessoas = geral.GetComponent<levelgenerator>().maxPessoasI;
//        maxPessoas = 3;

        Vector3 endereco = this.transform.position;
        int pnc = (int) Random.Range(1, maxPessoas+1);
 //       Debug.Log("pessoas na casa: " + pnc);
//        Debug.Log("endereco: " + endereco);

        for (int i = 0; i < pnc; i++)
        {
            pessoa.transform.position = new Vector3(endereco.x, pessoa.transform.position.y, endereco.z);
            //            pessoa.transform.position.y = endereco.y;
//            geral.GetComponent<levelgenerator>().contador ++;
//            Debug.Log("populacao total: " + geral.GetComponent<levelgenerator>().contador);
            //            geral = i;
            Instantiate(pessoa);
//            Instantiate(pessoa("joao" + i));
            //            Debug.Log("repos pessoa"+ i + " "+ pessoa.transform.position);
            // pessoa.transform.parent = this.transform;
        }

        if (pnc == 2) 
        {
            pnc = (int)Random.Range(0, numberOfCriancas+1);

            for (int i = 0; i < pnc; i++)
            {
                crianca.transform.position = new Vector3(endereco.x, crianca.transform.position.y, endereco.z);
                Instantiate(crianca);
            }

        }
//        crianca.transform.position = endereco;

    }

}

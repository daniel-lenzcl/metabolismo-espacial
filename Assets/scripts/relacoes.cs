using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class relacoes : MonoBehaviour
{
    public GameObject geral;
    public Text TXTtotalcasas;
    public Text TXTtotaltrabalhos;
    public Button gerarMapa;
    public InputField numeroCasas;
    // Start is called before the first frame update
    void Start()
    {
        geral = GameObject.Find("Terrain");

//        Debug.Log("RELACOES - START: total pessoas: " + geral.GetComponent<levelgenerator>().totalPessoas +
//                  "\n                total casas: " + geral.GetComponent<levelgenerator>().totalCasas +
//                  "\n                total trabalhos: " + geral.GetComponent<levelgenerator>().totalTrabalhos);

        GetTotalPessoas(geral.GetComponent<levelgenerator>().totalPessoas.ToString());

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void bAtribuiRelacoes()
    {
        geral.GetComponent<levelgenerator>().setaListas("predios");
        geral.GetComponent<levelgenerator>().setaListas("enderecos");
        geral.GetComponent<levelgenerator>().setaListas("pessoas");
        geral.GetComponent<levelgenerator>().iniciaMapa();

        for (int i = 0; i < geral.GetComponent<levelgenerator>().totalCasas; i++)
        {
            geral.GetComponent<levelgenerator>().predios.Add(new Predios(geral.GetComponent<levelgenerator>().casa, "casa" + i));
        }
//        for (int i = 0; i < totalEscolas; i++)
//        {
//            predios.Add(new Predios(escola, "escola" + i));
//        }
        for (int i = 0; i < geral.GetComponent<levelgenerator>().totalTrabalhos; i++)
        {
            geral.GetComponent<levelgenerator>().predios.Add(new Predios(geral.GetComponent<levelgenerator>().trabalho, "trabalho" + i));
        }



        /////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////ISSO PRECISA ACONTECER DPS DE TER OS ENDERECOS DOS PREDIOS TODOS. REVER////////////////
        //////////////////////////////////////////////   OU MUDAR PARA A INSTANCIACAO PEGAR SO O NOME DO PREDIO E DAI REFERENCIAR

        List<Predios> asCasas = new List<Predios>();
        asCasas.AddRange(geral.GetComponent<levelgenerator>().predios.FindAll((x) => x.nomePredio.Contains("casa")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
        List<Predios> osTrabalhos = new List<Predios>();
        osTrabalhos.AddRange(geral.GetComponent<levelgenerator>().predios.FindAll((x) => x.nomePredio.Contains("trabalho")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
                                                                                                                                        //        Predios casat;
        int ttcasas = asCasas.Count;

        int pessoasNCasa = int.Parse(GameObject.Find("InputTotalCasa").GetComponent<InputField>().text);
        int indexCasa = 0;
        int contapessoasCasa = 0;

        int pessoasNtrab = int.Parse(GameObject.Find("InputTotalTrabalho").GetComponent<InputField>().text);
        int indexTrab = 0;
        int contapessoasTrab = 0;
        Debug.Log("RELACOES-B ATRIBUI RELACOES: pessoas na casa:" + pessoasNCasa + "pessoas no trab: "+ pessoasNtrab);
        for (int i = 0; i < geral.GetComponent<levelgenerator>().totalPessoas; i++)
        {
            if (contapessoasCasa < pessoasNCasa)
            {
                //                index ++;
                contapessoasCasa++;
            } else
            {
                indexCasa++;
                contapessoasCasa = 1;
            }

            if (contapessoasTrab < pessoasNtrab)
            {
                //                index ++;
                contapessoasTrab++;
            }
            else
            {
                indexTrab++;
                contapessoasTrab = 1;
            }


            //            casat = asCasas[(int)Mathf.PingPong(i, asCasas.Count - 1)];
            //            casat = asCasas[(int)Mathf.Floor(i*fracao)];
//            Debug.Log("RELACOES-B ATRIBUI RELACOES: nome das casas: " + indexCasa + "contapessoas casa: " + contapessoasCasa +
//                "\n                 nome dos trab: " + indexTrab + " conta pessoas trab: " + contapessoasTrab);
//            Debug.Log("RELACOES-B ATRIBUI RELACOES: nome das casas: " + asCasas[indexCasa].nomePredio + "endereco casa: " + asCasas[indexCasa].enderecoXYZ);
            geral.GetComponent<levelgenerator>().todasAsPessoas.Add(new cPessoa(geral.GetComponent<levelgenerator>().prefabP, "pessoa" + i, asCasas[indexCasa], osTrabalhos[indexTrab]));
//            index++;
        }
        Debug.Log("RELACOES-B ATRIBUI RELACOES: contagem todasaspessoas: " + geral.GetComponent<levelgenerator>().todasAsPessoas.Count + "total de predios: "+ geral.GetComponent<levelgenerator>().predios.Count);


        gerarMapa.interactable = true;
    }

    public void GetTotalPessoas(string tTPessoa)   /////pessoas vai ser recuperado na classe casa
    {
        gerarMapa.interactable = false;

        geral.GetComponent<levelgenerator>().totalPessoas = int.Parse(tTPessoa);
        GetCasas(GameObject.Find("InputTotalCasa").GetComponent<InputField>().text);
        GetTrabalhos(GameObject.Find("InputTotalTrabalho").GetComponent<InputField>().text);

        //        Debug.Log("RELACOES - GET TOTAL PESSOAS: pessoas: " + tTPessoa);

    }


    public void GetCasas(string tCasa)
    {
        gerarMapa.interactable = false;

        //        Debug.Log("RELACOES - GET TOTAL CASAS: total casas (antes): " + geral.GetComponent<levelgenerator>().totalCasas);
        float pessoasPcasa = float.Parse(tCasa);
        float t =  Mathf.Ceil((float)geral.GetComponent<levelgenerator>().totalPessoas / pessoasPcasa);
        
        geral.GetComponent<levelgenerator>().totalCasas = (int)t;
        TXTtotalcasas.text = geral.GetComponent<levelgenerator>().totalCasas.ToString();
  
//        Debug.Log("RELACOES - GET TOTAL CASAS: total quebrado: " + t +  
//                "\nRELACOES - GET TOTAL CASAS: total casas(depois): " + geral.GetComponent<levelgenerator>().totalCasas);
    }

    public void GetEscolas(string tEscola)
    {
        geral.GetComponent<levelgenerator>().totalEscolas = int.Parse(tEscola);
    }

    public void GetTrabalhos(string tTrabalho)
    {
        gerarMapa.interactable = false;

        //        Debug.Log("RELACOES - GET TOTAL TRABALHOS: total trabalhos(antes): " + geral.GetComponent<levelgenerator>().totalTrabalhos);
        float pessoasPtrabalho = float.Parse(tTrabalho);
        geral.GetComponent<levelgenerator>().totalTrabalhos = (int)Mathf.Ceil((float)geral.GetComponent<levelgenerator>().totalPessoas / pessoasPtrabalho);
        //        geral.GetComponent<levelgenerator>().totalCasas = int.Parse(tCasa);
        //        GameObject.Find("Terrain").GetComponent<levelgenerator>().iniciaMapa();
        TXTtotaltrabalhos.text = geral.GetComponent<levelgenerator>().totalTrabalhos.ToString();

//        Debug.Log("RELACOES - GET TOTAL TRABALHOS: pessoas por trabalho: " + pessoasPtrabalho+
//                 "\nRELACOES - GET TOTAL TRABALHOS: total trabalhos(depois): " + geral.GetComponent<levelgenerator>().totalTrabalhos);
    }

    public void GetPessoas(string tPessoa)   /////pessoas vai ser recuperado na classe casa
    {
        geral.GetComponent<levelgenerator>().maxPessoasI = int.Parse(tPessoa);
    }


}

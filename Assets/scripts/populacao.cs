///SCRIPT RESPONSAVEL POR DESCREVER A MATRIZ DE ENCONTROS E SALVAR OS ARQUIVOS DESSA MATRIZ


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


//[System.Serializable]
public class populacao: MonoBehaviour
{

    public int populacaoTotal;
 
    public int p100crianca;
    public float[,] matriz;
    public string matriz_texto;
    public string vertices_individuos;
    public List<cContatos> listaC = new List<cContatos>();
    public GameObject[] individuos;
    public int numeroTestes = 0;
    public string endereco;
    public Button podelistarede;




    /*
     * public int p11_20;
        public int p21_30;
        public int p31_40;
        public int p41_50;
        public int p51_60;
        public int p61_70;
        public int p71_80;
        public int p81_100;
    */

    public void matrizEncontros()
    {
        GameObject geral = GameObject.Find("Terrain");
        int dimensao = geral.GetComponent<levelgenerator>().contador;

//        GameObject testeNome = GameObject.Find("pessoa1");
//        Debug.Log("id da pessoa1 eh " + testeNome.GetComponent<pessoa>().identidadepessoa);

        matriz = new float[dimensao, dimensao];


        individuos = GameObject.FindGameObjectsWithTag("pessoas");
        if (individuos == null)
        {
            Debug.Log("BOTAO LISTA: nenhuma pessoa");
            return;

        }

        vertices_individuos = "";
        foreach (GameObject i in individuos)
        {

//            vertices_individuos += (i.GetComponent<pessoa>().identidadepessoa +1)+ " \"" +   i.name.ToString() +"\"% " + i.GetComponent<pessoa>().euPessoa.minhaCasa.nomePredio + " "+ i.GetComponent<pessoa>().euPessoa.meuTrabalho.nomePredio + "\n";
        //    vertices_individuos += "%"+(i.GetComponent<pessoa>().identidadepessoa + 1) + " \"" + i.name.ToString() + "\"% " + i.GetComponent<pessoa>().euPessoa.minhaCasa.nomePredio + " " + i.GetComponent<pessoa>().euPessoa.meuTrabalho.nomePredio + "\n";
            int mx = i.GetComponent<pessoa>().identidadepessoa;
            listaC = i.GetComponent<conectados>().quemEncontrei;
            foreach(cContatos l in listaC)
            {
                int my = l.contato.GetComponent<pessoa>().identidadepessoa;
                matriz[mx, my] = l.horacont;
//                Debug.Log("hora do encontro: " + l.horacont);
            }
        }

        matriz_texto = "";

        for (int a = 0; a < dimensao; a++)
        {
            matriz_texto += ";p" + a;// + ";";
        }
//        matriz_texto += "\n";

        for (int a = 0; a < dimensao; a++)
        {
            matriz_texto += "\np" + a;// +";";
            for (int j = 0; j < dimensao; j++)
            {
                if(a == j)
                {
                    matriz[a, j] = 1;       //PREENCHENDO DIAGONAL PARA GEPHY RECONHECER OS NODES SEM CONTATO
                }
                    matriz_texto += ";" + matriz[a, j].ToString();// + ";";
            }
//            matriz_texto += "\n";
        }
        Debug.Log("matriz de encontros: \n"+ matriz_texto);


        /*        if (quemVi == " ")
                {
                    Debug.Log("dizEncontros: eu, " + this.GetComponent<pessoa>().identidadepessoa + ", nao vi ninguem");
                    return;
                }
                Debug.Log("dizEncontros: eu, " + this.GetComponent<pessoa>().identidadepessoa + ", vi " + quemVi);
        */        //        Debug.Log("dizEncontros: eu, " + this.GetComponent<pessoa>().identidadepessoa + ", encontrei " + quemEncontrei.Count + "pessoas");


    }

    public void GetEndereco(string tEndereco)
    {
        endereco = tEndereco;
        if (System.IO.Directory.Exists(@endereco.ToString())) 
        {
            podelistarede.interactable = true;
        }       //        if(@endereco)
                //        gerarMapa.interactable = true;
    }


    public void salvaMatriz() //Gephy
    {
        //endereco do arquivo
        string path = @"C:\Users\danie\gdrive prourb\PASTA academica\@UFRJ\doutorado\analise de redes\redes geradas gephy";
        //        endereco = GameObject.Find("InputEndereco").GetComponent<InputField>().text;
        path = @endereco.ToString();
        //data para cabecalho
        string dia = System.DateTime.Now.ToString(" dd-MM-yy HH_mm_ss ");
        //criando o arquivo
        string tM = GameObject.Find("Canvas").GetComponent<Mapas>().tipoMapa;
        string tD = GameObject.Find("Terrain").GetComponent<levelgenerator>().tipoDistribuicao;
        path += "/simulacao" + dia + " " + tM.Substring(0, 3) + " " + tD.Substring(0, 3) + ".csv";//net";
//        File.WriteAllText(path, "% mapa: " + tM + "; distribuicao: " + tD + "; " + GameObject.Find("Terrain").GetComponent<horas>().as_hora.text + "\n");
//        File.AppendAllText(path, "%*vertices " + individuos.Length.ToString() + "\n");

        /*        
                if (!File.Exists(path))
                {
                    path += "/teste" + numeroTestes + ".net";
                    File.WriteAllText(path, "*vertices "+ individuos.Length.ToString() + "\n");
                    numeroTestes++;
                }
                else
                {
                    path += "/teste" + numeroTestes + ".net";
                    File.WriteAllText(path, "*vertices " + individuos.Length.ToString() + "\n");
                    numeroTestes++;
                }
        */

        //conteudo do arquivo ---- ja usando o q ta escrito na variavel matriz+texto
        //cabecalho e verticies
        File.AppendAllText(path, vertices_individuos);


        //adicionar o conteudo no arquivo, no caso a matriz com o seu cabecalho
//        File.AppendAllText(path, "%*matrix\n" + matriz_texto);
        File.AppendAllText(path, matriz_texto);
        Debug.Log("arquivo salvo: " + path);
    }

    public void salvaMatrizPajek()
    {
        //endereco do arquivo
        string path = @"C:\Users\danie\gdrive prourb\PASTA academica\@UFRJ\doutorado\analise de redes\redes geradas";
//        endereco = GameObject.Find("InputEndereco").GetComponent<InputField>().text;
        path = @endereco.ToString();
        //data para cabecalho
        string dia = System.DateTime.Now.ToString(" dd-MM-yy HH_mm_ss ");
        //criando o arquivo
        string tM = GameObject.Find("Canvas").GetComponent<Mapas>().tipoMapa;
        string tD = GameObject.Find("Terrain").GetComponent<levelgenerator>().tipoDistribuicao;
        path += "/simulacao" + dia + " " + tM.Substring(0, 3) + " " + tD.Substring(0, 3) + ".net";
        File.WriteAllText(path, "% mapa: " + tM + "; distribuicao: " + tD + "; "+ GameObject.Find("Terrain").GetComponent<horas>().as_hora.text + "\n");
        File.AppendAllText(path, "%*vertices " + individuos.Length.ToString() + "\n");

        /*        
                if (!File.Exists(path))
                {
                    path += "/teste" + numeroTestes + ".net";
                    File.WriteAllText(path, "*vertices "+ individuos.Length.ToString() + "\n");
                    numeroTestes++;
                }
                else
                {
                    path += "/teste" + numeroTestes + ".net";
                    File.WriteAllText(path, "*vertices " + individuos.Length.ToString() + "\n");
                    numeroTestes++;
                }
        */

        //conteudo do arquivo ---- ja usando o q ta escrito na variavel matriz+texto
        //cabecalho e verticies
        File.AppendAllText(path, vertices_individuos);


        //adicionar o conteudo no arquivo, no caso a matriz com o seu cabecalho
        File.AppendAllText(path, "*matrix\n"+matriz_texto);
        Debug.Log("arquivo salvo: " + path);
    }
    /*   void start ()
      {

          populacaoTotal = p0_10 + p11_20 + p21_30 + p31_40 + p41_50 + p51_60 + p61_70 + p71_80 + p81_100;

      }
      */

}

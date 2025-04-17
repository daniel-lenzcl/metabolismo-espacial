using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using System.Linq;
//using System.Data.Entity;

public class conectados : MonoBehaviour
{

    //The list of colliders currently inside the trigger
    public List<GameObject> encontro;
    public LineRenderer linhasRede;
    Vector3 pos0;
    Vector3 pos1;
    private List<Vector3> pontosLinha = new List<Vector3>();
    private Vector3 posContato;

    public List<cContatos> quemEncontrei = new List<cContatos>();
    public cContatos testando;
    public string quemVi = " ";

   
    //    public List<string> quemVi = new List<string>();

    public bool esbarrei = false;  

    public void Start()
    {
        encontro = this.GetComponent<pessoa>().contatos;
        encontro.Clear();
        //        linhasRede = this.GetComponent<pessoa>().linhaContato;
        //        pos0 = this.transform.position;
        //        linhasRede = GetComponent<LineRenderer>();
        linhasRede.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        linhasRede.SetColors(Color.blue, Color.blue);
        linhasRede.SetWidth ( 0.3f, 0.3f);


        quemEncontrei.Clear();

    }

    void Update()
    {

        //---------BLOCO DE DEBUG PARA DISCRIMINACAO DOS CONTATOS
        //string result = "rede contatos: ";
        //foreach (var item in encontro)
        //{
        //    result += item.ToString() + ", ";
        //}
        //        Debug.Log(result);
        //------------------------------------------------------

        //--------------------------------------------------
        //incluir rotina para decair o valor dos encontros
        //----------------------------------------------------

        /*
        if (quemEncontrei.Count > 0)
        {
            pontosLinha.Clear();
            string r = "rede: ";

            foreach (cContatos pessoaContato in quemEncontrei)
            {
                posContato = pessoaContato.contato.transform.position;
                //Debug.Log("pt: " + posContato.ToString());
                //if (!pontosLinha.Contains(posContato))
                //      {
                //                        Debug.Log("entrou no pontosLinha: " + posContato);
                pontosLinha.Add(posContato);
                pontosLinha.Add(this.transform.position);
                r += posContato.ToString() + ", ";
                //    }
            }
            //            Debug.Log("total encontros " + encontro.Count + " total pontos linha " + pontosLinha.Count);
            //     Debug.Log(r);
            //    Debug.Log("pontos: " + pontosLinha.ToString());

            linhasRede.SetPositions(pontosLinha.ToArray());
        }
        */


        //----------------------------------------------------------
        /*
        if (encontro.Count > 0)
        {
            pontosLinha.Clear();
            string r = "rede: ";

                foreach (GameObject contato in encontro)
                {
                posContato = contato.transform.position;
                //Debug.Log("pt: " + posContato.ToString());
                //if (!pontosLinha.Contains(posContato))
                //      {
                    //                        Debug.Log("entrou no pontosLinha: " + posContato);
                    pontosLinha.Add(posContato);
                    pontosLinha.Add(this.transform.position);
                    r += posContato.ToString() + ", ";
                //    }
                }
            //            Debug.Log("total encontros " + encontro.Count + " total pontos linha " + pontosLinha.Count);

            //     Debug.Log(r);
            //    Debug.Log("pontos: " + pontosLinha.ToString());


            //   TENTAR FAZER ISSO COM O FOREACH
            /*
             int total_contatos = encontro.Count -1;
            while (total_contatos > 0)
            {
//                Debug.Log("contatos: " + total_contatos);
                //VERIFICAR COMO ASSOCIAR A POSICAO Dos GameObject
                //LINERENDERER SO PODE SER 1 POR OBJETO. ENTAO VERIFICAR ONDE Q FICA CADA UM E COMO ORGANIZAR ISSO. SE DA PRA CRIAR POR VEZ. OU JA DEIXA CRIADO POR CADA PESSOA

                //                pontosLinha.Add(this.GetComponent<GameObject>().transform.position);
                Vector3 posContato = encontro[total_contatos].transform.position;
                pontosLinha.Add(posContato);

                total_contatos--;
            }
            *//*
            linhasRede.SetPositions(pontosLinha.ToArray());

        }
        */
        //----------------------------------------------------------------

    }

    public void dizEncontros()
    {
        if(quemVi == " ")
        {
            Debug.Log("CONECTADOS-DIZ_ENCONTROS: eu, "+this.GetComponent<pessoa>().identidadepessoa + ", nao vi ninguem");
            return;
        }
        Debug.Log("CONECTADOS-DIZ_ENCONTROS: eu, " + this.GetComponent<pessoa>().identidadepessoa + ", vi " + quemVi);
//        Debug.Log("dizEncontros: eu, " + this.GetComponent<pessoa>().identidadepessoa + ", encontrei " + quemEncontrei.Count + "pessoas");
    }

    /// <summary>
    /// PRECISA VER COMO FUNCIONA O TRIGGER, NO SENTIDO DE QUAL RAIO DE ACAO. SE CONTA SO O 1o. e dai corrigir pra dar o trigger para todos q estiverem no raio. nesse caso,
    /// vai precisar ver um esquema pra pegar quem ta no raio mas sem repetir pelo momento.
    /// 
    /// problema aki eh liberar o trigger enter. se toda vida entrar, vou marcar varias vezes o mesmo sujeito. entao precisa ver como resolver isso por algum outro lado
    /// se encontrei um, dai cncelo, como encontrar outro, ou se repentir o encontro, como nao contar de novo? 
    /// 
    /// provavelmente a resposta vai ser por criar um flag pra cada contato novo ativado pelos triggers
    /// </summary>
    /// <param name="Ooutro"></param>
    public void seraQueEncontrei(GameObject Ooutro)
    {
        //        if (esbarrei) { return; }
        //        esbarrei = !esbarrei;
        //        Debug.Log("CONECTADOS - SERA Q ENCONTREI: esbarrei em alguem: " + Ooutro.name);

        //        bool tp = false;

        //testes q deram quase certo tao aki
        //    if (quemEncontrei.Exists(x => x.contato == tempcont.contato)) 
        //cContatos teste51 = quemEncontrei.Find(x => x.contato == tempcont.contato);
        //int teste52 = quemEncontrei.IndexOf(quemEncontrei.Find(x => x.contato == tempcont.contato));
        //Debug.Log("testando encontrar com INDEX OF . FIND x=> x.contat, antes:" + teste52);

        //quemEncontrei.Add(tempcont);

        //        teste51 = quemEncontrei.Find (x => x.contato == tempcont.contato);
        //teste52 = quemEncontrei.IndexOf(quemEncontrei.Find(x => x.contato == tempcont.contato));

        //Debug.Log("testando encontrar com INDEX OF . FIND x=> x.contat, DEPOIS:" + teste52);
        //        Debug.Log("testando encontrar com EXISTS x=> x.contat, antes:" + quemEncontrei.IndexOf(teste51));
        // teste51 = quemEncontrei.Contains(tempcont);
        //Debug.Log("testando encontrar com CONTAINS, depois:" + teste51);

        cContatos tempcont = new cContatos(Ooutro.gameObject, 1);
        string tempquemVi = tempcont.contato.GetComponent<pessoa>().identidadepessoa.ToString();
        int idxcontato = quemEncontrei.IndexOf(quemEncontrei.Find(x => x.contato == tempcont.contato));

//        Debug.Log("CONECTADOS - SERA Q ENCONTREI: encontrei o" + tempquemVi +"o index de quem encontrei agora eh: "+ idxcontato);
//        dizEncontros();

        if (idxcontato < 0)
        {
            quemEncontrei.Add(tempcont);
            quemVi += tempquemVi + " ";
//            Debug.Log("CONECTADOS - SERA Q ENCONTREI: encontrei o " + tempquemVi + " pela 1a vez");
//            dizEncontros();

        }
        else
        {
            quemEncontrei[idxcontato].horacont++;// = quemEncontrei[indexcontato].horacont + 1;
            quemVi += tempquemVi + " ";
//            Debug.Log("CONECTADOS - SERA Q ENCONTREI: ja eh a " + quemEncontrei[idxcontato].horacont + "que vi o" + tempquemVi);
//            dizEncontros();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        esbarrei = !esbarrei;
//        Debug.Log("CONECTADOS: TRIGGER EXIT: " + esbarrei);

    }

    private void OnTriggerEnter(Collider other)
    {
//        Debug.Log("CONECTADOS: TRIGGER ENTER ");
        seraQueEncontrei(other.gameObject);
//        this.ResetTrigger;

    }

//    private void OnTriggerStay(Collider other)
//    {
//        if (esbarrei) { return; }
//        Debug.Log("CONECTADOS: TRIGGER STAY");
//        seraQueEncontrei(other.gameObject);
//        esbarrei = !esbarrei;
//        return;
//     }
}
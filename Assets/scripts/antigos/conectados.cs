using System.Collections.Generic;
using UnityEngine;
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
        DebugController.Log(DebugCategoria.conectados, $"START ▸ tracking");

        //        encontro = this.GetComponent<pessoa>().contatos;
        //        encontro.Clear();
        //colocar a lista onde guardar as pessoas encontradas

        //        linhasRede = this.GetComponent<pessoa>().linhaContato;
        //        pos0 = this.transform.position;
        //        linhasRede = GetComponent<LineRenderer>();
        if (linhasRede == null)
        {
            linhasRede = GetComponent<LineRenderer>();
        }

        if (linhasRede == null)
        {
            DebugController.LogError(DebugCategoria.conectados, "Start ▸ LineRenderer não encontrado em 'conectados'.");
            return;
        }

        // material para o line renderer
        linhasRede.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

        // substitui métodos obsoletos por propriedades atuais
        linhasRede.startColor = Color.blue;
        linhasRede.endColor = Color.blue;

        linhasRede.startWidth = 0.3f;
        linhasRede.endWidth = 0.3f;

        quemEncontrei.Clear();

    }

    void Update()
    {

    }

    public void dizEncontros()
    {
        DebugController.Log(DebugCategoria.conectados, $"dizEncontros ▸ tracking");

        if (quemVi == " ")
        {
            Debug.Log("CONECTADOS-DIZ_ENCONTROS: eu, " + this.GetComponent<pessoa>().identidadepessoa + ", nao vi ninguem");
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
        DebugController.Log(DebugCategoria.conectados, $"seraQueEncontrei ▸ tracking");


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
            quemEncontrei[idxcontato].horacont++;// horacont como marcador de quantas vezes encontrou
            quemVi += tempquemVi + " ";
            //            Debug.Log("CONECTADOS - SERA Q ENCONTREI: ja eh a " + quemEncontrei[idxcontato].horacont + "que vi o" + tempquemVi);
            //            dizEncontros();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        DebugController.Log(DebugCategoria.conectados, $"OnTriggerExit ▸ tracking {esbarrei}");

        esbarrei = !esbarrei;
        //        Debug.Log("CONECTADOS: TRIGGER EXIT: " + esbarrei);

    }

    private void OnTriggerEnter(Collider other)
    {
        DebugController.Log(DebugCategoria.conectados, $"OnTriggerEnter ▸ tracking {other.name}");

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
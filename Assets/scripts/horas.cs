
using System;
using UnityEngine;
using UnityEngine.UI;

public class horas : MonoBehaviour
{
    //    public Transform mexerhoras, mexerminuto, mexersegundo;
    public Text as_hora;
    public Text os_dia;
    public double tempoSimulado;// = 3;


    public int hora;
    public int min;
    public int dias;
    //    float temposim = 0.5f;
    double temporizador = 0f;
    double tpassado = 0f;
    Texture tex;
    bool listaAuto = false;
    bool primeiraLista = false;
    public bool rodadia = true;

    public Button listaRede;

    public void Start()
    {
        hora = 0;
        min = 0;
        dias = 0;
        tempoSimulado = 0.1;
        //        Debug.Log(DateTime.Now);
        //   Debug.Log(" vamo ver");
        //       Debug.Log(DateTime.Now);
        tpassado = Time.deltaTime;
        rodadia = false;
        listaAuto = true;
        primeiraLista = false;
        Debug.Log("HORAS - START: inicializado RODA DIA = " +rodadia);
        Debug.Log(tempoSimulado);

    }

    void Update()
    {
        //   rodadia = GameObject.Find("Canvas").GetComponent<botoes>().comecaDia;  -----------------> ESSE CONTROLE VEIO PRA UMA VARIAVEL DAKI


        //        temporizador += Time.deltaTime * boolToFloat(rodadia);
//        Debug.Log("HORAS - UPDATE: roda dia: " + rodadia);

        if (rodadia)
              {
                  temporizador += Time.deltaTime;
            //Debug.Log("roda dia: " + rodadia);
//                    Debug.Log("intervalo " + tempoSimulado);
//                    Debug.Log("temporizador " + temporizador);
//        Debug.Log("tempo passado " + (tempoSimulado - temporizador));

            if (tempoSimulado < temporizador )
            {
                //            float tpassado = Time.deltaTime - temporizador;
                //           Debug.Log("tempo passado " + tpassado);
                //           Debug.Log("tempo andou");
                temporizador = 0;// Time.deltaTime;
                min+=20;
                if (min > 59)
                {
                    min = 0;
                    hora++;
                    if (hora > 23) 
                    { 
                        hora = 0;
                        dias++;
                    }
                }
            }
            /*
                    mexerhoras.localRotation = Quaternion.Euler(0f, DateTime.Now.Hour * angulohora, 0f);
                    mexerminuto.localRotation = Quaternion.Euler(0f, DateTime.Now.Minute * angulomin, 0f);
                    mexersegundo.localRotation = Quaternion.Euler(0f, DateTime.Now.Second * anguloseg, 0f);
            */
            //        mexerhoras.localRotation = Quaternion.Euler(0f, hora * angulohora, 0f);
            //        mexerminuto.localRotation = Quaternion.Euler(0f, min * angulomin, 0f);
            //        mexersegundo.localRotation = Quaternion.Euler(0f, DateTime.Now.Second * anguloseg, 0f);
            //        as_hora.text = hora.ToString() + "h" + min.ToString();

        }
        //        os_dia.text = (dias+"d");
        //        as_hora.text = (hora + "h" + min.ToString("00"));

        as_hora.text = (dias + "d " + hora.ToString("00") + "h" + min.ToString("00"));

        if (!primeiraLista && listaAuto && dias == 0 && hora == 1)
        {
            if (listaRede.interactable)
            {
                Debug.Log("lista rede na 1h da manha");
                primeiraLista = true;
                GameObject.Find("Canvas").GetComponent<botoes>().botaoListaRede();
            }
        }

        if (listaAuto && dias == 7 && hora == 5)
        {
            if (listaRede.interactable)
            {
                Debug.Log("lista rede no dia 7, 5h da manha");
                rodadia = false;
                listaAuto = false;
                GameObject.Find("Canvas").GetComponent<botoes>().botaoListaRede();
            }
        }


    }

    /*
        public void Sair()
        {
            Application.Quit();
        }
    */

    /*
    void OnGUI()
    {
        GUILayout.Label(tex);
        GUILayout.Label(tex);
        GUILayout.Label("  " + hora + "h" + min);
    }
*/
}
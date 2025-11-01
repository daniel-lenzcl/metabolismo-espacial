using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class horas : MonoBehaviour
{
    public UnityEvent MudouHora = new UnityEvent(); // Evento para quando bater 8h

    //    public Transform mexerhoras, mexerminuto, mexersegundo;
    public Text as_hora;
    public Text os_dia;
    public double tempoSimulado;// = 3;

    public int dias;
    public int hora;
    public int min;
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
        tempoSimulado = 0.6f;
        //        Debug.Log(DateTime.Now);
        //   Debug.Log(" vamo ver");
        //       Debug.Log(DateTime.Now);
        tpassado = Time.deltaTime;
        rodadia = false;
        listaAuto = false;
        primeiraLista = false;
        DebugController.Log(DebugCategoria.Horas, "HORAS - START: inicializado RODA DIA = " + rodadia);
        DebugController.Log(DebugCategoria.Horas, tempoSimulado.ToString());


}

void Update()
    {
        //acionado pelo botao roda_dia no canvas, ver botoes.cs
        if (rodadia)
        {
            temporizador += Time.deltaTime;
            //Debug.Log("roda dia: " + rodadia);
            // Debug.Log("intervalo " + tempoSimulado);
            // Debug.Log("temporizador " + temporizador);
            // Debug.Log("tempo passado " + (tempoSimulado - temporizador));

            if (tempoSimulado < temporizador)
            {
                //            float tpassado = Time.deltaTime - temporizador;
                //           Debug.Log("tempo passado " + tpassado);
                //           Debug.Log("tempo andou");
                temporizador = 0;// Time.deltaTime;
                min += 20;
                if (min > 59)
                {
                    min = 0;
                    hora++;
                    MudouHora.Invoke(); // Dispara o evento
//                    Debug.Log("disparou hora");
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
        }

        as_hora.text = (dias + "d " + hora.ToString("00") + "h" + min.ToString("00"));

        if (primeiraLista && listaAuto && dias == 0 && hora == 1)
        {
            if (listaRede.interactable)
            {
                DebugController.Log(DebugCategoria.Horas, "lista rede na 1h da manha");
                primeiraLista = false;
                GameObject.Find("Canvas").GetComponent<botoes>().botaoListaRede();
            }
        }

        if (listaAuto && dias == 7 && hora == 5)
        {
            if (listaRede.interactable)
            {
                DebugController.Log(DebugCategoria.Horas, "lista rede no dia 7, 5h da manha");
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

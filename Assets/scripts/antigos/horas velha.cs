
using System;
using UnityEngine;
using UnityEngine.UI;

public class horasvelhas : MonoBehaviour
{
//    public Transform mexerhoras, mexerminuto, mexersegundo;
    public Text as_hora;
    public float tempoSimulado = 1f;


    public float hora;
    public float min;
//    float temposim = 0.5f;
    float temporizador = 0f;
    float tpassado = 0f;


    void Start()
    {
        hora = 0f;
        min = 0f;
    //        Debug.Log(DateTime.Now);
 //   Debug.Log(" vamo ver");
 //       Debug.Log(DateTime.Now);
        tpassado = Time.deltaTime;
    }

    void Update()
    {
        temporizador += Time.deltaTime;
//        Debug.Log("intervalo " + tempoSimulado);
//        Debug.Log("temporizador " + temporizador);
//        Debug.Log("tempo passado " + (temporizador - tpassado));

        if (tempoSimulado < temporizador)
        {
            //            float tpassado = Time.deltaTime - temporizador;
            //           Debug.Log("tempo passado " + tpassado);
 //           Debug.Log("tempo andou");
            temporizador = Time.deltaTime;
            min++;
            if (min > 59f)
            {
                min = 0f;
                hora++;
                if (hora > 23f) { hora = 0f; }
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
        as_hora.text = hora.ToString() + "h" + min.ToString();
    }

}
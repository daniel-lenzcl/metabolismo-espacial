using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class horas : MonoBehaviour
{
    public UnityEvent MudouHora = new UnityEvent(); // Evento para quando bater 8h

    private Dictionary<int, List<System.Action>> eventosPorMinuto = new Dictionary<int, List<System.Action>>();
    /// <summary>
    /// evento parametrizado para levantar a partir das rotinasBase.
    /// </summary>

    //    public Transform mexerhoras, mexerminuto, mexersegundo;
    public Text as_hora;
    public Text os_dia;
    public double tempoSimulado;// = 3;

    public int dia;
    public int hora;
    public int min;

    public int minutoDoDia;// = (int)(minutoAbs % 1440);
//    int dia = (int)(minutoAbs / 1440);
//    int hora = minutoDoDia / 60;
//    int minuto = minutoDoDia % 60;

    //tempo total do dia em minutos
    public long minutosCorridos = 0;
    public long horasAntes = 0;
    public long horasDepois = 0;
    //passo da contagem por minuto
    public int minutosPasso = 20;

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

        minutosCorridos = 0;
        AtualizaRelogio();

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

                long minutosAntes = minutosCorridos;
                horasAntes = minutosCorridos/60;

                minutosCorridos += minutosPasso;
                AtualizaRelogio();

                horasDepois = minutosCorridos / 60;
                long minutosDepois = minutosCorridos;

                // dispara eventos por minuto atravessado
                for (long minutoAbs = minutosAntes + 1; minutoAbs <= minutosDepois; minutoAbs++)
                {
                    int minutoDoDiaAtual = (int)(minutoAbs % 1440);

                    if (eventosPorMinuto.TryGetValue(minutoDoDiaAtual, out var callbacks))
                    {
                        foreach (var cb in callbacks)
                            cb?.Invoke();
                        Debug.Log("horas disparou alarme " + minutoDoDiaAtual);
                    }
                }

                for (long i = 0; i < (horasDepois - horasAntes); i++)
                {
                    MudouHora?.Invoke();
//                    Debug.Log("disparou hora");
                }


            }

        }

        as_hora.text = (dia + "d " + hora.ToString("00") + "h" + min.ToString("00"));

        if (primeiraLista && listaAuto && dia == 0 && hora == 1)
        {
            if (listaRede.interactable)
            {
                DebugController.Log(DebugCategoria.Horas, "lista rede na 1h da manha");
                primeiraLista = false;
                GameObject.Find("Canvas").GetComponent<botoes>().botaoListaRede();
            }
        }

        if (listaAuto && dia == 7 && hora == 5)
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

    // Converte totalMinutes para dia/hora/min e atualiza campos públicos
    void AtualizaRelogio()
    {        
        minutoDoDia = (int)(minutosCorridos % 1440);
        dia = (int)(minutosCorridos / 1440);
        hora = minutoDoDia / 60;
        min = minutoDoDia % 60;
    }

    public void RegistrarEventoNoMinuto(int minuto, System.Action callback)
    {
        minuto = ((minuto % 1440) + 1440) % 1440;

        if (!eventosPorMinuto.TryGetValue(minuto, out var lista))
        {
            lista = new List<System.Action>();
            eventosPorMinuto[minuto] = lista;
        }

        if (!lista.Contains(callback))
            lista.Add(callback);
        Debug.Log("alarme criado: " + minuto);
    }

    public void RemoverEventoNoMinuto(int minuto, System.Action callback)
    {
        minuto = ((minuto % 1440) + 1440) % 1440;

        if (eventosPorMinuto.TryGetValue(minuto, out var lista))
        {
            lista.Remove(callback);

            if (lista.Count == 0)
                eventosPorMinuto.Remove(minuto);
        }
    }

}

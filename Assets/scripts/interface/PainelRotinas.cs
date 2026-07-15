using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PainelRotinas : MonoBehaviour
{
    public GerentePessoas gerentePessoas;
    public TMP_Dropdown OpcoesPessoas;
    public TMP_InputField InputNomePessoa;
    public TMP_InputField InputPercentual;
    private string pessoaSelecionadaId;
    private TemplatePessoa pessoaEmEdicao;

    public TMP_Dropdown DropAtividade;
    public TMP_InputField InputAtividade;

    public TMP_Dropdown DropLugar;
    public TMP_InputField InputLugar;

    public TMP_InputField inicio;
    public TMP_InputField fim;

    public Text SlotsRotinas;
    public Text PerfisRotinas;

    public Button bEditarSlot;
    public Button bApagarSlot;

    public Transform ContentSlots;
    public Button PrefabBotaoSlot;
    private int slotSelecionadoIndex = -1;
    private string slotSelecionadoId = null;

    public Transform PreviewSlots;
    public Image PrefabBlocoSlot;

    public List<SlotRotinaUI> slotsEditando = new List<SlotRotinaUI>();

    // Start is called before the first frame update

    private List<string> atividades = new List<string>
    {
        "",
        "nova...",
        "dormir",
        "comer",
        "trabalhar"
    };

    private List<string> lugares = new List<string>
    {
        "",
        "casa",
        "loja",
        "resto",
        "nova..."
    };

    public void AtualizarDropdown(TMP_Dropdown dropdown, List<string> lista)
    {
        dropdown.ClearOptions();

        List<string> opcoes = new List<string>(lista);
//        opcoes.Add("nova...");

        dropdown.AddOptions(opcoes);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }
    public void CarregarTemplates()
    {

    }

    public void AoMudarAtividade(int index)
    {
        string selecionada = DropAtividade.options[index].text;

        if (selecionada == "nova...")
        {
            Debug.Log("Selecionou criar nova atividade: " + selecionada);
            InputAtividade.text = "";
            InputAtividade.gameObject.SetActive(true);
            DropAtividade.gameObject.SetActive(false);
            InputAtividade.Select();
            InputAtividade.ActivateInputField();
        }
        else
        {
            Debug.Log("Selecionou atividade antiga: " + selecionada);
            InputAtividade.gameObject.SetActive(false);
        }
    }

    public void ConfirmarNovaAtividade()
    {
        string nova = InputAtividade.text.Trim();

        if (string.IsNullOrEmpty(nova))
            return;

        if (!atividades.Contains(nova))
        {
            atividades.Add(nova);
        }

        AtualizarDropdown(DropAtividade, atividades);

        int novoIndex = atividades.IndexOf(nova);
        DropAtividade.value = novoIndex;
        DropAtividade.RefreshShownValue();

        InputAtividade.text = "";
        InputAtividade.gameObject.SetActive(false);
        DropAtividade.gameObject.SetActive(true);
    }

    void Start()
    {
        bApagarSlot.interactable = false;
        bEditarSlot.interactable = false;

        InputAtividade.gameObject.SetActive(false);
        AtualizarDropdown(DropAtividade, atividades);
        AtualizarDropdown(DropLugar, lugares);
    }

    public void AtualizarVisualizacao()
    {
        indicesSlotsEmConflito = EncontrarIndicesSlotsEmConflito();

        AtualizarBotoesSlots();

        AtualizarPreview24h();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CriarSlot()
    {
        //        SlotRotinaUI novoSlot = new SlotRotinaUI(System.Guid.NewGuid().ToString(), atividade, valorInicio, valorFim, tipoLugar);
        SlotRotinaUI novoSlot = InputParaSlot();
        //        novoSlot.Id = System.Guid.NewGuid().ToString();
        List<SlotRotinaUI> slotsCandidatos = new List<SlotRotinaUI>(slotsEditando);
        slotsCandidatos.Add(novoSlot);

        RotinaBase.TryCreateFromUI(slotsCandidatos, out RotinaBase rotinaTeste, out string erro);

        slotsEditando.Add(novoSlot);

        AtualizarVisualizacao();
        InputAtividade.gameObject.SetActive(false);
    }

    public void EditarSlot()
    {
        SlotRotinaUI slotSelecionado = slotsEditando.Find(s => s.Id == slotSelecionadoId);
        if (slotSelecionado == null)
        {
            Debug.LogWarning("Slot selecionado nao encontrado.");
            return;
        }
        SlotRotinaUI temporario = InputParaSlot();

        slotSelecionado.atividadeId = temporario.atividadeId;
        slotSelecionado.inicioMin = temporario.inicioMin;
        slotSelecionado.fimMin = temporario.fimMin; 
        slotSelecionado.tipoLugarId = temporario.tipoLugarId;

        RotinaBase.TryCreateFromUI(slotsEditando, out RotinaBase rotina, out string erro);

        AtualizarVisualizacao();

        bApagarSlot.interactable = false;
        bEditarSlot.interactable = false;
    }

    private SlotRotinaUI InputParaSlot()
    {
        if (!TentarConverterHoraParaMinutos(inicio.text, out int valorInicio))
        {
            Debug.LogWarning("Hora inicial inválida. Use HH:mm, por exemplo 08:00. valor zerado");
            valorInicio = -1;
       }
        if (!TentarConverterHoraParaMinutos(fim.text, out int valorFim))
        {
            Debug.LogWarning("Hora final inválida. Use HH:mm, por exemplo 12:00. valor zerado");
            valorFim = -1;
        }
        string tipoLugar = DropLugar.options[DropLugar.value].text;
        string atividade = DropAtividade.options[DropAtividade.value].text;

        SlotRotinaUI inputSlot = new SlotRotinaUI(System.Guid.NewGuid().ToString(), atividade, valorInicio, valorFim, tipoLugar);

        return inputSlot;
    }

    public void ApagarSlot()
    {
        //        slotsEditando.RemoveAt(slotSelecionadoIndex);
        slotsEditando.RemoveAll(s => s.Id == slotSelecionadoId);
        slotSelecionadoId = null;

        bApagarSlot.interactable = false;
        bEditarSlot.interactable = false;

        InputSlotAtualizar(slotApagado);

        AtualizarVisualizacao();
    }

    public void SalvarPerfilRotina()
    {

    }

    public void ApagarPerfilRotina()
    {
    }

  
    private bool TentarConverterHoraParaMinutos(string texto, out int minutos)
    {
        minutos = 0;

        if (string.IsNullOrWhiteSpace(texto))
            return false;

        string[] partes = texto.Split(':');

        if (partes.Length != 2)
            return false;

        if (!int.TryParse(partes[0], out int hora))
            return false;

        if (!int.TryParse(partes[1], out int minuto))
            return false;

        if (hora < 0 || hora > 23)
            return false;

        if (minuto < 0 || minuto > 59)
            return false;

        minutos = hora * 60 + minuto;
        return true;
    }

    private string FormatarMinutosComoHora(int minutos)
    {
        minutos = ((minutos % 1440) + 1440) % 1440;

        int hora = minutos / 60;
        int minuto = minutos % 60;

        return $"{hora:00}:{minuto:00}";
    }

    public void AtualizarBotoesSlots()
    {
        foreach (Transform filho in ContentSlots)
        {
            Destroy(filho.gameObject);
        }

//        for (int i = 0; i < slotsEditando.Count; i++)
        foreach(SlotRotinaUI slot in slotsEditando)
            {
  //          int indice = i;
  //          SlotRotinaUI slotAtualizacao = slotsEditando[i];

            Button botao = Instantiate(PrefabBotaoSlot, ContentSlots);
            botao.gameObject.SetActive(true);

            TMP_Text textoBotao = botao.GetComponentInChildren<TMP_Text>();
            textoBotao.text = TextoDoSlot(slot);

            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => SelecionarSlot(slot.Id));
            
            Image imagemBotao = botao.GetComponent<Image>();

            if (indicesSlotsEmConflito.Contains(slot.Id))
            {
                imagemBotao.color = new Color(1f, 0.35f, 0.35f, 1f);
            }
            else if (slot.Id == slotSelecionadoId)
            {
                imagemBotao.color = Color.yellow;
            }
            else
            {
                imagemBotao.color = Color.white;
            }
        }
    }

    private string TextoDoSlot(SlotRotinaUI slot)
    {
        return slot.atividadeId
            + " | "
            + FormatarMinutosComoHora(slot.inicioMin)
            + " - "
            + FormatarMinutosComoHora(slot.fimMin)
            + " | "
            + slot.tipoLugarId;
    }


    public void SelecionarSlot(string selectId)
    {
        if (selectId == slotSelecionadoId)
        {
            slotSelecionadoId = null;
            bApagarSlot.interactable = false;
            bEditarSlot.interactable = false;

            InputSlotAtualizar(slotApagado);
        }
        else
        {
            slotSelecionadoId = selectId;
            bApagarSlot.interactable = true;
            bEditarSlot.interactable = true;

            SlotRotinaUI slot = slotsEditando.Find(s => s.Id == slotSelecionadoId);

            InputSlotAtualizar(slot);
        }

        Debug.Log("Slot selecionado: " + selectId);
        AtualizarVisualizacao();
    }

    private void SelecionarOpcaoDropdown(TMP_Dropdown dropdown, string texto)
    {
        int index = dropdown.options.FindIndex(opcao => opcao.text == texto);

        if (index >= 0)
        {
            dropdown.value = index;
            dropdown.RefreshShownValue();
        }
        else
        {
            dropdown.value = 0;
        }
    }
     
    private SlotRotinaUI slotApagado = new SlotRotinaUI("", "", 0, 0, "");
    private void InputSlotAtualizar(SlotRotinaUI slotAtualizacao)
    {
        SelecionarOpcaoDropdown(DropAtividade, slotAtualizacao.atividadeId);
        inicio.text = FormatarMinutosComoHora(slotAtualizacao.inicioMin);
        fim.text = FormatarMinutosComoHora(slotAtualizacao.fimMin);
        SelecionarOpcaoDropdown(DropLugar, slotAtualizacao.tipoLugarId);

    }

    /// <summary>
    /// SESSAO LINHA DO TEMPO: atribui uma cor para cada tipo de atividade+tipoLugar+horario,
    /// para facilitar a visualizacao da rotina no preview de 24h. 
    /// A chave é composta por atividadeId + tipoLugarId + inicioMin + fimMin, 
    /// para diferenciar mesmo atividades iguais em horarios diferentes ou lugares diferentes. 
    /// As cores são geradas aleatoriamente, mas com um filtro para evitar tons de vermelho, 
    /// reservado pra indicar conflitos, falhas ou incompletude na definicao da rotina.
    /// </summary>
    private Dictionary<string, Color> coresPorAtividade = new Dictionary<string, Color>();
    private HashSet<string> indicesSlotsEmConflito = new HashSet<string>();

    private const string vazioID = "vazio";
    public void AtualizarPreview24h()
    {
        foreach (Transform filho in PreviewSlots)
        {
            Destroy(filho.gameObject);
        }

        RectTransform barraRect = PreviewSlots as RectTransform;
        float larguraTotal = barraRect.rect.width;

        List<SlotRotinaUI> slotsOrdenados = new List<SlotRotinaUI>(slotsEditando);
        slotsOrdenados.Sort((a, b) => a.inicioMin.CompareTo(b.inicioMin));
        int cursorMin = 0;

        foreach(SlotRotinaUI slot in slotsOrdenados)
//            for (int i = 0; i < slotsOrdenados.Count; i++)
        {
//            SlotRotinaUI slotAtualizacao = slotsOrdenados[i];

//            if (slot.inicioMin > cursorMin)
//            {
//                int duracaoVazio = slot.inicioMin - cursorMin;
//                CriarBlocoPreview(slot, vazioID, duracaoVazio, larguraTotal, new Color(0f, 0f, 0f, 0.08f));
//            }

            int duracaoSlot = slot.fimMin - slot.inicioMin;

            if (duracaoSlot > 0)
            {
                
                CriarBlocoPreview(slot, slot.Id, duracaoSlot, larguraTotal, CorDoSlot(slot));
            }

//            cursorMin = Mathf.Max(cursorMin, slot.fimMin);
        }

//        if (cursorMin < 1440)
//        {
//            int duracaoVazioFinal = 1440 - cursorMin;
//            CriarBlocoPreview(slot, vazioID, duracaoVazioFinal, larguraTotal, new Color(0f, 0f, 0f, 0.08f));
//        }


    
    }

    private void CriarBlocoPreview(SlotRotinaUI slot, string slotId, int duracaoMinutos, float larguraTotal, Color cor)
    {
        if (duracaoMinutos <= 0)
            return;

        float xInicio = larguraTotal * (slot.inicioMin / 1440f);
        float larguraBloco = larguraTotal * (duracaoMinutos / 1440f);

        Image bloco = Instantiate(PrefabBlocoSlot, PreviewSlots);
        bloco.gameObject.SetActive(true);
        bloco.color = cor;

        RectTransform rect = bloco.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 0.5f);

        rect.anchoredPosition = new Vector2(xInicio, 0f);
        rect.sizeDelta = new Vector2(larguraBloco, 0f);
        /*
        LayoutElement layout = bloco.GetComponent<LayoutElement>();
        if (layout == null)
            layout = bloco.gameObject.AddComponent<LayoutElement>();

        layout.minWidth = larguraBloco;
        layout.preferredWidth = larguraBloco;
        layout.flexibleWidth = 0;
        */


        if (slotId == vazioID) return;

        //Outline para indicar conflito e bloco selecionado
        Outline outline = bloco.GetComponent<Outline>();

        if (outline == null)
            outline = bloco.gameObject.AddComponent<Outline>();

        if (indicesSlotsEmConflito.Contains(slotId))
        {
            outline.enabled = true;
            outline.effectDistance = new Vector2(0f, 0f);

            if (slotId == slotSelecionadoId)
            {
                outline.effectColor = new Color(1, 0.92f, 0.016f, 0.5f); //Color.yellow;
            }
            else
            {
                outline.effectColor = new Color(1,0,0,0.5f);//.red;
            }
        }
        else if (slotId == slotSelecionadoId)
        {
            outline.enabled = true;
            outline.effectColor = new Color(1,1,1,0.5f); //.white
            outline.effectDistance = new Vector2(0f, 0f);
        }
        else
        {
            outline.enabled = false;
        }

    }

    private int CalcularDuracaoMinutos(int inicioMin, int fimMin)
    {
        if (fimMin > inicioMin)
            return fimMin - inicioMin;

        return (1440 - inicioMin) + fimMin;
    }

    private Color CorDoSlot(SlotRotinaUI slot)
    {
//        string chave = index.ToString() + slotAtualizacao.atividadeId.Trim().ToLower() + slotAtualizacao.inicioMin.ToString() + slotAtualizacao.fimMin.ToString() + slotAtualizacao.tipoLugarId.Trim().ToLower();
        string chave = slot.Id;
        //        if (string.IsNullOrEmpty(chave))
        //            chave = tipoLugarId.Trim().ToLower();

        if (coresPorAtividade.ContainsKey(chave))
            return coresPorAtividade[chave];

        Color novaCor = GerarCorAleatoriaSemVermelho();
        coresPorAtividade[chave] = novaCor;

        return novaCor;
    }

    private Color GerarCorAleatoriaSemVermelho()
    {
        float hue;

        do
        {
            hue = Random.Range(0f, 1f);
        }
        while (hue < 0.08f || hue > 0.92f); // evita faixa vermelha

        float saturation = Random.Range(0.45f, 0.75f);
        float value = Random.Range(0.65f, 0.95f);

        Color cor = Color.HSVToRGB(hue, saturation, value);
        cor.a = 0.45f; // transparencia

        return cor;
    }

    private HashSet<string> EncontrarIndicesSlotsEmConflito()
    {
        HashSet<string> conflitos = new HashSet<string>();

//        foreach(SlotRotinaUI a in slotsEditando)
        for (int i = 0; i < slotsEditando.Count; i++)
        {
//            foreach (SlotRotinaUI b in slotsEditando)
            for (int j = i + 1; j < slotsEditando.Count; j++)
            {
                SlotRotinaUI a = slotsEditando[i];
                SlotRotinaUI b = slotsEditando[j];
//                if (a.Id == b.Id) continue;

                int inicio = Mathf.Max(a.inicioMin, b.inicioMin);
                int fim = Mathf.Min(a.fimMin, b.fimMin);

                if (inicio < fim)
                {
                    conflitos.Add(a.Id);
                    conflitos.Add(b.Id);
                }
            }
        }

        return conflitos;
    }

}

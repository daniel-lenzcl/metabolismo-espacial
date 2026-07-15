/*
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class painel_tipo_pessoa : MonoBehaviour
{
    [SerializeField] Gerente_de_ambiente Ambiente;
    public string formulario_tipo_pessoa;

    public string formulario_atv_08_10;
    public string formulario_atv_10_12;
    public string formulario_atv_12_14;
    public string formulario_atv_14_16;
    public string formulario_atv_16_18;
    public string formulario_atv_18_20;  
    public string formulario_atv_20_08;

    public TMP_Dropdown input_atv_08_10;
    public TMP_Dropdown input_atv_10_12;
    public TMP_Dropdown input_atv_12_14;
    public TMP_Dropdown input_atv_14_16;
    public TMP_Dropdown input_atv_16_18;
    public TMP_Dropdown input_atv_18_20;
    public TMP_Dropdown input_atv_20_08;

    public TMP_Dropdown opcoesPessoas;
    public TMP_InputField novoNomePessoas;

    // ✅ Aqui ficam os slots montados no início
    private List<SlotRotinaUI> rotinaSlotsUI;

    // ✅ (opcional) Também guardo os dropdowns numa lista/array para iterar
    private TMP_Dropdown[] dropdownsRotina;
    //    public List<SO_pessoa> tipos_de_pessoas = new List<SO_pessoa>();
    public List<molde_pessoas> as_pessoas = new List<molde_pessoas>();

    public int contador_de_tipos;
    public int i = 0;


    public static event Action OnPainelAtivado;
    public event Action<molde_pessoas> OnAtualizaPessoas;

    void OnEnable()
    {
        OnPainelAtivado?.Invoke(); // dispara evento quando for ativado
        Debug.Log("ativando painel de pessoas");
    }

    public void atualizar_tipo_pessoa()
    {
        //fazer a selecao entre o nome novo ou algum q esteja selecionado no dropdown
        molde_pessoas pessoa_temp = new molde_pessoas();
        if (novoNomePessoas.gameObject.activeSelf)
        {
            pessoa_temp.tipo_pessoa = novoNomePessoas.text;
            novoNomePessoas.text = "";
            novoNomePessoas.gameObject.SetActive(false); // Mostra campo de input
        }
        else
        {
            pessoa_temp.tipo_pessoa = opcoesPessoas.options[opcoesPessoas.value].text;
        }

        //adiciona os valores dos horarios
        pessoa_temp.atv_08_10 = input_atv_08_10.options[input_atv_08_10.value].text;
        pessoa_temp.atv_10_12 = input_atv_10_12.options[input_atv_10_12.value].text;
        pessoa_temp.atv_12_14 = input_atv_12_14.options[input_atv_12_14.value].text;
        pessoa_temp.atv_14_16 = input_atv_14_16.options[input_atv_14_16.value].text;
        pessoa_temp.atv_16_18 = input_atv_16_18.options[input_atv_16_18.value].text;
        pessoa_temp.atv_18_20 = input_atv_18_20.options[input_atv_18_20.value].text;
        pessoa_temp.atv_20_08 = input_atv_20_08.options[input_atv_20_08.value].text;

        Debug.Log("no evento atualizando " + pessoa_temp.tipo_pessoa);
        OnAtualizaPessoas?.Invoke(pessoa_temp);

    }

    void Start()
    {
        PreencheDropDownOpcoesPessoas();

        contador_de_tipos = 0;

        //        gameObject.SetActive(false);

    }

    public void PreencheDropDownOpcoesPessoas()
    {
        // Limpa as opções anteriores
        opcoesPessoas.ClearOptions();

        // lista com as novas opções
        List<string> tipos = new List<string>();

        //1a posicao vazia
        tipos.Add("");
        //Debug.Log("total na lista do ambiente: " + Ambiente.lista_tipos_pessoas.Count);

        foreach (molde_pessoas m in Ambiente.lista_tipos_pessoas)
        {
            tipos.Add(m.tipo_pessoa);
        }
        //ultima posicao com 'criar novo'
        tipos.Add("novo...");
        //adiciona as opcoes
        opcoesPessoas.AddOptions(tipos);

        // Opcional: seleciona o primeiro
        opcoesPessoas.value = 0;
        opcoesPessoas.RefreshShownValue();

        Debug.Log("total de tipos: " + tipos.Count);
    }

    public void OnDropdownChanged(int index)
    {
        //       Debug.Log("on dropdown, index: " + index + "texto: " + opcoesPessoas.options[index].text);
        if (opcoesPessoas.options[index].text == "novo...")
        {
            //            Debug.Log("dropdown");
            novoNomePessoas.gameObject.SetActive(true); // Mostra campo de input
        }
    }

   
    void Update()
    {

    }
}

*/

using System.Collections.Generic;
using UnityEngine;

public class GerenteEncontros : MonoBehaviour
{
    public Dictionary<string, List<float>> encontrosRegistrados; // Dicionário para armazenar os encontros com timestamps
    private MaterialPropertyBlock propertyBlock;
    public Mesh marcadorMesh;
    public Material marcadorMaterial;

    public List<GameObject> marcadoresAtivos;  // Lista para armazenar os marcadores criados

    public List<Matrix4x4> marcadoresMatrices;  // Lista para armazenar as matrizes das instâncias de meshes



    public GameObject cilindroPrefab;   // O prefab do cilindro a ser instanciado
    public float alturaCilindro = 1f;   // Altura do cilindro
    public float raioCilindro = 0.2f;   // Raio do cilindro
    void Start()
    {
        DebugController.Log(DebugCategoria.GerenteEncontros, "Start ▸");


        encontrosRegistrados = new Dictionary<string, List<float>>(); // Inicializa o dicionário de encontros
        propertyBlock = new MaterialPropertyBlock(); // Inicializa o MaterialPropertyBlock para manipular as cores

        marcadoresAtivos = new List<GameObject>();  // Inicializa a lista de marcadores
        marcadoresMatrices = new List<Matrix4x4>();  // Inicializa a lista de matrizes


        // Exemplo de como configurar a malha e o material para DrawMeshInstanced
        Graphics.DrawMeshInstanced(marcadorMesh, 0, marcadorMaterial, new Matrix4x4[] { Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one) }, 1);
    }

    void Update()
    {
        if (marcadoresMatrices.Count > 0)
        {
            // Desenha as instâncias de mesh (cilindro ou outro mesh) na posição armazenada nas matrizes
            Graphics.DrawMeshInstanced(marcadorMesh, 0, marcadorMaterial, marcadoresMatrices);
//            Debug.Log($"atualizando marcadores, tamano da lista: {marcadoresMatrices.Count}");
        }

        // Tamanho desejado (ajustar conforme necessário)
 //       Vector3 tamanhoMarcador = new Vector3(2f, 20f, 2f); // Ajusta a escala para 2x em cada eixo

        // Cria a instância de mesh do marcador na posição do encontro com a escala ajustada
 //       Graphics.DrawMeshInstanced(marcadorMesh, 0, marcadorMaterial,
 //           new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.identity, tamanhoMarcador) }, 1, propertyBlock);

    }
    // Subscrição ao evento de encontro
    void OnEnable()
    {
//        Paraconectar.OnEncontro += RegistrarEncontro;
    }

    void OnDisable()
    {
 //       Paraconectar.OnEncontro -= RegistrarEncontro;
    }
    void OnGUI()
    {
        // Exibindo o tamanho do dicionário na interface do usuário
        GUILayout.Label("Tamanho do Dictionary: " + encontrosRegistrados.Count);
    }

    // Método que é chamado quando um encontro acontece
    public void RegistrarEncontro(cPessoa agente1, cPessoa agente2, Vector3 posicao)
    {
        string chaveEncontro = GerarChaveEncontro(agente1, agente2); // Cria uma chave única para o par de agentes

        // Verifica se o encontro já foi registrado
        if (!encontrosRegistrados.ContainsKey(chaveEncontro))
        {
            encontrosRegistrados[chaveEncontro] = new List<float>(); // Cria uma lista de timestamps para o encontro
        }

        // Registra o timestamp do encontro
        float timestamp = Time.time; // Usamos o tempo do jogo para o timestamp
        encontrosRegistrados[chaveEncontro].Add(timestamp); // Adiciona o timestamp à lista de encontros

        // Cria o marcador para esse encontro
        CriarMarcador(posicao, timestamp);

        // Debug para verificar os encontros


        DebugController.Log(DebugCategoria.GerenteEncontros, $"RegistrarEncontro ▸ Encontro registrado entre {agente1.identidade} e {agente2.identidade} no tempo {timestamp}.");
    }

    // Criação do marcador (instância de mesh) no ponto de encontro
    private void CriarMarcador(Vector3 posicao, float timestamp)
    {
        // Ajusta as cores do marcador com base nos agentes envolvidos (aqui estamos usando cores fixas, mas pode ser dinâmico)
        propertyBlock.SetColor("_Color1", Color.red); // Exemplo: Cor do agente 1
        propertyBlock.SetColor("_Color2", Color.blue); // Exemplo: Cor do agente 2

        // Tamanho desejado (ajustar conforme necessário)
        Vector3 tamanhoMarcador = new Vector3(2f, 20f, 2f); // Ajusta a escala para 2x em cada eixo

        // Cria a instância de mesh do marcador na posição do encontro com a escala ajustada
//        Graphics.DrawMeshInstanced(marcadorMesh, 0, marcadorMaterial,
//            new Matrix4x4[] { Matrix4x4.TRS(posicao, Quaternion.identity, tamanhoMarcador) }, 1, propertyBlock);

        // Instancia o marcador (cilindro) no ponto de encontro
        GameObject marcador = Instantiate(cilindroPrefab, posicao, Quaternion.identity);

        // Ajusta a altura e o raio do cilindro, caso necessário
        marcador.transform.localScale = new Vector3(raioCilindro*4, alturaCilindro , 4*raioCilindro);

        // Adiciona o marcador à lista de marcadores ativos
//        marcadoresAtivos.Add(marcador);
        // (Opcional) Você pode adicionar mais funcionalidades aqui, como animações ou efeitos de partículas
        // Cria a matriz de transformação para o marcador
//        Matrix4x4 matriz = Matrix4x4.TRS(posicao, Quaternion.identity, Vector3.one);
        // Adiciona a matriz à lista de instâncias
//        marcadoresMatrices.Add(matriz);
    }

    // Método para gerar uma chave única para os encontros entre dois agentes (para evitar duplicação)
    private string GerarChaveEncontro(cPessoa agente1, cPessoa agente2)
    {
        // A chave é uma combinação dos nomes dos agentes, de forma que "pessoa1_pessoa2" e "pessoa2_pessoa1" sejam iguais
        return agente1.identidade.CompareTo(agente2.identidade) < 0 ? $"{agente1.identidade}_{agente2.identidade}" : $"{agente2.identidade}_{agente1.identidade}";
    }

    // Método para filtrar os encontros por tempo (exemplo: retorna os encontros em um intervalo de tempo específico)
    public List<float> FiltrarEncontrosPorTempo(cPessoa agente1, cPessoa agente2, float inicio, float fim)
    {
        string chaveEncontro = GerarChaveEncontro(agente1, agente2);

        if (encontrosRegistrados.ContainsKey(chaveEncontro))
        {
            // Filtra os timestamps que estão dentro do intervalo de tempo desejado
            List<float> encontrosFiltrados = encontrosRegistrados[chaveEncontro].FindAll(timestamp => timestamp >= inicio && timestamp <= fim);
            return encontrosFiltrados;
        }
        else
        {
            return new List<float>(); // Nenhum encontro registrado
        }
    }
    public void DeletarMarcadores()
    {
        // Limpa a lista de matrizes, o que efetivamente "deleta" os marcadores ao não desenhá-los mais
        marcadoresMatrices.Clear();
        Debug.Log("Todos os marcadores foram deletados.");
    }
}

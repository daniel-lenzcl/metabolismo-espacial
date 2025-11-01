using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class levelgenerator : MonoBehaviour
{

    //    public GameObject pessoa;
    //    public GameObject enemy;
    public GameObject cam;
    public GameObject prefabP; //prefab das pessoas
    public GameObject casa;
    public GameObject escola;
    public GameObject trabalho;
    public GameObject predio_com_porta;


    public GameObject terrain;
    public List<Predios> predios;
    public List<Vector3> enderecos;
    public List<cPessoa> todasAsPessoas;
    public string tipoDistribuicao;// = "aleatorio";
    public List<cPessoa> tempPessoas;



    private Collider col;

//    public NavMeshSurface superficie;

//    public int numberOfEnemies;
//    public int numberOfPessoas;
    public int totalCasas;
    public int totalEscolas;
    public int totalTrabalhos;
    public int maxPessoasI;
    public int totalPessoas;
    public float distanciaPredios = 10f;

    private List<Vector3> usedPoints;

    public int numLugares;

    public Lugar lugar;
    public cPessoa[] familias;
    //    public List<cPessoa> familias;
    public int contador;
    public int contador2;


    void Start()
    {

/*
        Debug.Log("levelgenerator - START: total pessoas: " + totalPessoas +
                  "\n                total casas: " + totalCasas +
                  "\n                total trabalhos: " + totalTrabalhos);
*/
        /*        int tpop = GetComponent<populacao>().populacaoTotal;
                int criancas = tpop * GetComponent<populacao>().p100crianca/100;
                int adultos = tpop - criancas;
                Debug.Log("populacao total: " + tpop + " criancas: " + criancas);
        */

        usedPoints = new List<Vector3>();
//        cam.GetComponent<ajusteCamera>().reposicionar(); //substituida por CONTROLECAMERA
        //        iniciaMapa();
        //        lugar = new List<Lugar>();
    }

    // Update is called once per frame
//    void Update()
//    {
//        
//    }

    public void setaListas(string lista)
    {
        if (predios == null) { return; }

        switch (lista)
        {
            case "predios":
                predios.Clear();
                break;
            case "enderecos":
                enderecos.Clear();
                break;
            case "pessoas":
//                if (todasAsPessoas == null) { return; }
                todasAsPessoas.Clear();
                break;
        }
    }


    public void iniciaMapa()
    {

        //        setaListas("predios");
        //        setaListas("enderecos");
        //        setaListas("pessoas");


        GameObject[] pessoas = GameObject.FindGameObjectsWithTag("pessoas");
        foreach (GameObject g in pessoas)
        {
            contador = 0;
            Destroy(g);
        }
        GameObject[] criancas = GameObject.FindGameObjectsWithTag("criancas");
        foreach (GameObject g in criancas)
        {
            Destroy(g);
        }
        GameObject[] lugares = GameObject.FindGameObjectsWithTag("lugares");
        foreach (GameObject g in lugares)
        {
            Destroy(g);
        }
        GameObject[] escolas = GameObject.FindGameObjectsWithTag("escolas");
        foreach (GameObject g in escolas)
        {
            Destroy(g);
        }
        GameObject[] trabalhos = GameObject.FindGameObjectsWithTag("trabalho");
        foreach (GameObject g in trabalhos)
        {
            Destroy(g);
        }

        /////////////////////////////////////////////////////////////////////////////
        ///PROVAVELMENTE, MELHOR REGISTRAR TODOS OS LUGARES AQUI. PERMITIRIA UM COUNT TOTAL DE LUGARES, BEM COMO DE CADA TIPO.
        ///NESSE CASO, COMO CONTROLAR A ATRIBUICAO DISTRIBUIDA, ALEATORIA OU SETORIZADA? 
        ///         FAZER UMA ROTINA DE ESCOLHA DISSO, ONDE SE FACA A LISTA DE LUGARES A PARTIR DA UNIAO DAS DEMAIS. ESSA UNIAO VAI SER ALEATORIA, SEQUENCIAL, OU INTERCALADA. 
        ///         ESSA LISTA FINAL SERIA ENVIADA PARA A LOCACAO. ENTRETANTO, ISSO PODE TER CASOS ESPECIAIS QUANDO TRATAR DE MATRIZES POR EXEMPLO


        //      GenerateObjects(pessoa, numberOfPessoas);
        //        GenerateObjects(escola, totalEscolas);

        //        GenerateObjects(trabalho, totalTrabalhos);
        //        GenerateObjects(casa, totalCasas);

        //        pessoa();

        /*
        for (int i = 0; i < totalCasas; i++)
        {
            predios.Add(new Predios(casa, "casa" + i));
        }
        for (int i = 0; i < totalEscolas; i++)
        {
            predios.Add(new Predios(escola, "escola" + i));
        }
        for (int i = 0; i < totalTrabalhos; i++)
        {
            predios.Add(new Predios(trabalho, "trabalho" + i));
        }

        Debug.Log("LEVEL-INICIA MAPA: contagem todasaspessoas: " + todasAsPessoas.Count);
        for (int i = 0; i < totalPessoas; i++)
        {
            todasAsPessoas.Add(new cPessoa(prefabP, "pessoa" + i));
        }
        Debug.Log("LEVEL-INICIA MAPA: contagem todasaspessoas: " + todasAsPessoas.Count);
        */


        //atribuiPessoas();
    }


    public void colocaPredios()
    {

        terrain.GetComponent<NavMeshSurface>().BuildNavMesh();

        switch (tipoDistribuicao)
        {
            case "aleatorio":
                List<Vector3> endtemp = enderecos;
                for (int i = 0; i < predios.Count; i++)
                {
                    //                    Debug.Log("LEVEL\COLOCA PREDIOS: total de enderecos rand:" + enderecos.Count);
                    int tend = Random.Range(0, enderecos.Count);
                    predios[i].enderecoXYZ = endtemp[tend];
                    //                    Instantiate(predios[i].predioPreFab, endtemp[tend], Quaternion.identity);
                    endtemp.RemoveAt(tend);

                }
                break;
            case "setorizado":
                break;
            case "continuo":
                for (int i = 0; i < predios.Count; i++)
                {
                    predios[i].enderecoXYZ = enderecos[i];
                    //                    Instantiate(predios[i].predioPreFab, enderecos[i], Quaternion.identity);

                }
                break;
            case "alternado":
                ///                                             CRIAR ESQUEMA TIPO ENDTEMP DO ALEATORIO
                List<Predios> asCasas = new List<Predios>();
                asCasas.AddRange(predios.FindAll((x) => x.nomePredio.Contains("casa")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
                List<Predios> osTrabalhos = new List<Predios>();
                osTrabalhos.AddRange(predios.FindAll((x) => x.nomePredio.Contains("trabalho")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
                float numCasa = asCasas.Count;
                float numTrab = osTrabalhos.Count;

                List<Predios> maisPredios = new List<Predios>();
                List<Predios> menosPredios = new List<Predios>();
                float prop;
                if (numCasa < numTrab)
                {
                    prop = numTrab / numCasa;
                    maisPredios = osTrabalhos;
                    menosPredios = asCasas;
                }
                else
                {
                    prop = numCasa / numTrab;
                    maisPredios = asCasas;
                    menosPredios = osTrabalhos;
                }

                float propC = numCasa / numTrab;
                float propT = numTrab / numCasa;
                int Pindex = 0;
                int index1 = 0;
                int index2 = 0;
                //Debug.Log("LEVEL COLOCA PREDIOS - ALTERNADO: numCasa " + numCasa + "numTrab " + numTrab);
                //Debug.Log("LEVEL COLOCA PREDIOS - ALTERNADO: propC " + propC + "propT " + propT);


                /// reescrever a partir de uma lista copiada, em q cada atribuicao "come" a lista. ao final, garante q todos os elementos da lista terao sido percorridos
                /// algo tipo
                /// copiaEnderecos
                /// 
                /// conta os q precisa do tipo 1
                ///     atribui pra tipo1[0]=copiaEnderecos[0]
                ///     tipo1[0].Remove, copaiEnderecos[0].remove, proxima contagem
                /// conta os q precisa do tipo 2
                ///     atribui pra tipo2[0]=copiaEnderecos[0]
                ///     tipo2[0].Remove, copaiEnderecos[0].remove, proxima contagem
                /// 
                Debug.Log("LEVEL COLOCA PREDIOS - ALTERNADO: porp: " + prop +" mais predios = " + maisPredios.Count + "menos predios " + menosPredios.Count);

                int controleProporcao = 0;
                for (int i = 0; i < predios.Count; i++)
                {
                Debug.Log("LEVEL COLOCA PREDIOS - ALTERNADO: i: " + i);

                    if (controleProporcao < prop)
                    {
                        if (index1 < maisPredios.Count)
                        {
                            Pindex = predios.IndexOf(maisPredios[index1]);
                            Debug.Log("LEVEL COLOCA PREDIOS - poe mais predios: indexC: " + index1 + "Pindex " + Pindex);
                            index1++;
                            controleProporcao++;
                        }
                    }
                    else
                    {
                        if (index2 < menosPredios.Count)
                        {
                            Pindex = predios.IndexOf(menosPredios[index2]);
                            Debug.Log("LEVEL COLOCA PREDIOS - poe menos predios: indexT: " + index2 + "Pindex " + Pindex);
                            index2++;
                            if (index1 < maisPredios.Count)
                            {
                                controleProporcao = 0;
                            }

                        }

                    }

                    predios[Pindex].enderecoXYZ = enderecos[i];
//                    Debug.Log("LEVEL COLOCA PREDIOS - Mathf.Max: " + Mathf.Max(propC, propT) + "controlePredio: " + controlePredio);
                }
                break;
            case "importar mapa":
                GameObject ambiente = GameObject.Find("ambiente");
                ambiente.GetComponent<CarregarMapa>().IniciarCaptura();




                break;

        }
        ///teste eqt ajusta a identificacao das portas
//        foreach (Predios p in predios)
//        {
//            Instantiate(p.predioPreFab, p.enderecoXYZ, Quaternion.identity);
//            Debug.Log("LEVEL-COLOCA PREDIOS: nome predio: "+ p.nomePredio + "endereco " + p.enderecoXYZ);
//        }

        cam.GetComponent<ajusteCamera>().reposicionar();
        //        Debug.Log("centro bounds: " + posCam);

    }

    /*
    public void distribuicaoAtividades(Dropdown dropDistribuicao)
    {
        tipoDistribuicao = dropDistribuicao.options[dropDistribuicao.value].text;
        Debug.Log("LEVEL-DISTRIBUICAO ATIVIDADES: distribuicao eh:" + tipoDistribuicao);

        //        Debug.Log("distribuicao eh:" + dropDistribuicao.options[dropDistribuicao.value].text);
    }*/

    public void mapaAleatorio()
    {

        float passoRad = 2 * Mathf.PI / predios.Count; 

        float raio = (distanciaPredios ) / (passoRad / 3);
        //        Debug.Log("LEVEL-MAPA CIRCULO: passo(rad): " + passoRad);

        terrain.GetComponent<Terrain>().terrainData.size = new Vector3(raio*2, 0, raio*2 );       //dimensionamento variavel atribuido aki

        //        Generate();   -----> substituido por iniciaMapa
        usedPoints.Clear();

        gerarMatrizUsedPoints(predios.Count);

        GenerateObjects(escola, totalEscolas);
        GenerateObjects(trabalho, totalTrabalhos);
        GenerateObjects(casa, totalCasas);
//        Debug.Log("LEVEL - MAPA ALEATORIO: total de enderecos: "+ enderecos.Count);
        colocaPredios();

    }

    void GenerateObjects(GameObject go, int amount)
    {
        //        Debug.Log("LEVEL - GENERATE OBJECTS: total de enderecos no comeco: " + enderecos.Count);
        if (go == null) return;
        col = terrain.GetComponent<TerrainCollider>();

        for (int i = 0; i < amount; i++)
        {
            //Vector3 randPoint = getRandPoint();
            //usedPoints.Add(randPoint);
            //randPoint.y += go.transform.position.y;
            //            Instantiate(go,randPoint,Quaternion.identity);

            //////////////versao simplificada via matriz de pontos
            int auxRdm = Random.Range(0, usedPoints.Count);
            Vector3 randPoint = usedPoints[auxRdm];
            usedPoints.RemoveAt(auxRdm);
            /////////////////
            enderecos.Add(randPoint);

        }
        //        Debug.Log("LEVEL - GENERATE OBJECTS: total de enderecos no fim: " + enderecos.Count);

        //        colocaPredios();
    }

    public void gerarMatrizUsedPoints(int totalPontos)
    {

        totalPontos *= 3;
            int ladoX = (int)Mathf.Sqrt(totalPontos);
            int ladoZ = (int)(totalPontos / ladoX);
            float sobra = totalPontos - (ladoX * ladoZ);
            //        Debug.Log("LEVEL\MAPA MATRIZ: colunas: " + (lado1 + Mathf.Ceil(sobra/lado1)));

            float colunaExtra = Mathf.Ceil(sobra / ladoX);
            //        Debug.Log("LEVEL\MAPA MATRIZ: pontos x: " + (ladoX + colunaExtra));

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///                     INSERIR AJUSTE TE TAMANHO DO TERRENO

            float x = (ladoX + colunaExtra) * distanciaPredios + distanciaPredios;
            float z = ladoZ * distanciaPredios + distanciaPredios;
            terrain.GetComponent<Terrain>().terrainData.size = new Vector3(x, 0, z);       //dimensionamento variavel atribuido aki

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            float passoX = distanciaPredios; // lateralX / (ladoX + colunaExtra + 1);
            float passoZ = distanciaPredios; // lateralZ / (ladoZ + 1);
                                             //        Debug.Log("LEVEL\MAPA MATRIZ: lado x: "+ lateralX+"passo x: " + passoX);
                                             //        Debug.Log("LEVEL\MAPA MATRIZ: lado z: " +lateralZ+"passo z: " + passoZ);

            float posX = passoX;
            float posZ = passoZ;
            int indexX = 1;
            int indexZ = 1;

            Vector3 tvector = new Vector3((indexX * passoX), 0f, (indexZ * passoZ));
            float tempY = Terrain.activeTerrain.SampleHeight(tvector);
            for (int i = 0; i < totalPontos; i++) // predios.Count; i++)
            {
                //            Debug.Log("LEVEL-MAPA MATRIZ: index x: " + indexX + "; index z: " + indexZ);
                Vector3 posicao = new Vector3((indexX * passoX), tempY, (indexZ * passoZ));
                usedPoints.Add(posicao);
                //            Debug.Log("LEVEL-MAPA MATRIZ: coordendas: " + posicao);
                //            Debug.Log("LEVEL-MAPA MATRIZ: coordendas\n x: " + (indexX * passoX) + ", z: " + (indexZ * passoZ) + ", y: " + tempY);


                indexX += 1;
                if (indexX > ladoX + colunaExtra)
                {
                    indexX = 1;
                    indexZ += 1;
                    if (indexZ > ladoZ)
                    {
                        indexZ = 1;
                    }
                }
            }
        Debug.Log("total usedPoints: " + usedPoints.Count);


    }

    public void mapaMatriz()
    {
        //        enderecos = new Vector3[totalCasas + totalEscolas + totalTrabalhos];

//        Debug.Log("LEVEL-MAPA MATRIZ: total de predios: " + predios.Count);
//        Debug.Log("LEVEL-MAPA MATRIZ: total de enderecos: " + enderecos.Count);

        int ladoX = (int)Mathf.Sqrt(predios.Count);
        int ladoZ = (int)(predios.Count / ladoX);
        float sobra = predios.Count - (ladoX * ladoZ);
        //        Debug.Log("LEVEL\MAPA MATRIZ: lado 1: " + lado1 + "; lado 2: " + lado2 + "; sobra: " + sobra);
        //        Debug.Log("LEVEL\MAPA MATRIZ: colunas: " + (lado1 + Mathf.Ceil(sobra/lado1)));

        float colunaExtra = Mathf.Ceil(sobra / ladoX);
        //        Debug.Log("LEVEL\MAPA MATRIZ: sobra/lado: " + colunaExtra);
        //        Debug.Log("LEVEL\MAPA MATRIZ: pontos x: " + (ladoX + colunaExtra));
        //        Debug.Log("LEVEL\MAPA MATRIZ: pontos z: " + (ladoZ));

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                     INSERIR AJUSTE TE TAMANHO DO TERRENO
        ///                     
        ///             int pontos = predios.Count;
        ///             float passoRad = 2 * Mathf.PI / pontos; //INVERTER A DEFINICAO. PASSO_RAD = 180-(360/PONTOS)
        ///             float raio = (distanciaPredios / 2) / (passoRad / 2);

        float x = (ladoX + colunaExtra) * distanciaPredios + distanciaPredios;          //TA BEM ERRADO, HORA FALA DE QTD DE PONTOS NA OUTRA DE DISTANCIA, REVISAR
        float z = ladoZ * distanciaPredios + distanciaPredios;
        terrain.GetComponent<Terrain>().terrainData.size = new Vector3(x, 0, z);       //dimensionamento variavel atribuido aki
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Collider terreno = terrain.GetComponent<TerrainCollider>();
        //float lateralX = terreno.bounds.max.x - terreno.bounds.min.x;
        //float lateralZ = terreno.bounds.max.z - terreno.bounds.min.z;
        float passoX = distanciaPredios; // lateralX / (ladoX + colunaExtra + 1);
        float passoZ = distanciaPredios; // lateralZ / (ladoZ + 1);
        //        Debug.Log("LEVEL\MAPA MATRIZ: lado x: "+ lateralX+"passo x: " + passoX);
        //        Debug.Log("LEVEL\MAPA MATRIZ: lado z: " +lateralZ+"passo z: " + passoZ);

        float posX = passoX;
        float posZ = passoZ;
        int indexX = 1;
        int indexZ = 1;

        Vector3 tvector = new Vector3((indexX * passoX), 0f, (indexZ * passoZ));
        float tempY = Terrain.activeTerrain.SampleHeight(tvector);

        //        Vector3 novapos = new Vector3(x, tempY, z);

        //        GameObject predio = casa;
        for (int i = 0; i < predios.Count; i++)
        {
            //            Debug.Log("LEVEL-MAPA MATRIZ: index x: " + indexX + "; index z: " + indexZ);
            Vector3 posicao = new Vector3((indexX * passoX), tempY, (indexZ * passoZ));
            enderecos.Add(posicao);
            //            Debug.Log("LEVEL-MAPA MATRIZ: coordendas: " + posicao);
            //            Debug.Log("LEVEL-MAPA MATRIZ: coordendas\n x: " + (indexX * passoX) + ", z: " + (indexZ * passoZ) + ", y: " + tempY);

            ////////////////////////// colocar a instanciacao pra outro lugar, baseada na associacao de list.predios com list.enderecos/////////////////
            //            Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);

            indexX += 1;
            if (indexX > ladoX + colunaExtra)
            {
                indexX = 1;
                indexZ += 1;
                if (indexZ > ladoZ)
                {
                    indexZ = 1;
                }
            }
        }

        ////////////////////////// colocar a instanciacao pra outro lugar, baseada na associacao de list.predios com list.enderecos/////////////////
        colocaPredios();
        /*
         * registro do total de posicoes necessarias
         * registro da posicao inicial
         * gerar as varias posicoes na forma (forma definida pela expressao que a define)
         * 
         * decidir se gera todas as posicoes e devolve pra rotina anterior para atribuicao, ou se mantem registro interno e vai atribuindo a medida q recebe
        */
    }

    public void mapaLinha()
    {
        //        enderecos = new Vector3[totalCasas + totalEscolas + totalTrabalhos];

//        Debug.Log("LEVEL-MAPA LINHA: total de enderecos: " + predios.Count);

        int ladoX = predios.Count;
        int ladoZ = 1;
        float sobra = predios.Count - (ladoX * ladoZ);

        //        Debug.Log("lado 1: " + lado1 + "; lado 2: " + lado2 + "; sobra: " + sobra);
        //      Debug.Log("colunas: " + (lado1 + Mathf.Ceil(sobra/lado1)));
        float colunaExtra = Mathf.Ceil(sobra / ladoX);
        //        colunaExtra = colunaExtra / lado1;
        //        Debug.Log("LEVEL-MAPA LINHA: sobra/lado: " + colunaExtra);
        //        Debug.Log("LEVEL-MAPA LINHA: pontos x: " + (ladoX + colunaExtra));
        //        Debug.Log("LEVEL-MAPA LINHA: pontos z: " + (ladoZ));

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                     INSERIR AJUSTE TE TAMANHO DO TERRENO
        ///                     
        ///             int pontos = predios.Count;
        ///             float passoRad = 2 * Mathf.PI / pontos; //INVERTER A DEFINICAO. PASSO_RAD = 180-(360/PONTOS)
        ///             float raio = (distanciaPredios / 2) / (passoRad / 2);
        float x = ladoX * distanciaPredios + distanciaPredios;
        float z = ladoZ * distanciaPredios + distanciaPredios;

        terrain.GetComponent<Terrain>().terrainData.size = new Vector3(x, 0, z);       //dimensionamento variavel atribuido aki
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        Collider terreno = terrain.GetComponent<TerrainCollider>();
        //float lateralX = terreno.bounds.max.x - terreno.bounds.min.x;
        //float lateralZ = terreno.bounds.max.z - terreno.bounds.min.z;
        float passoX = distanciaPredios; // lateralX / (ladoX + colunaExtra + 1);
        float passoZ = distanciaPredios; // lateralZ / (ladoZ + 1);

//        Debug.Log("LEVEL-MAPA LINHA: lado x: " + lateralX + "passo x: " + passoX);
//        Debug.Log("LEVEL-MAPA LINHA: lado z: " + lateralZ + "passo z: " + passoZ);

        float posX = passoX;
        float posZ = passoZ;
        int indexX = 1;
        int indexZ = 1;

        Vector3 tvector = new Vector3((indexX * passoX), 0f, (indexZ * passoZ));
        float tempY = Terrain.activeTerrain.SampleHeight(tvector);

        //        Vector3 novapos = new Vector3(x, tempY, z);

        //        GameObject predio = casa;
        for (int i = 0; i < predios.Count; i++)
        {
//            Debug.Log("LEVEL-MAPA LINHA: index x: " + indexX + "; index z: " + indexZ);
            Vector3 posicao = new Vector3((indexX * passoX), tempY, (indexZ * passoZ));
//            Debug.Log("LEVEL-MAPA LINHA: coordendas: " + posicao);
            //            Debug.Log("coordendas\n x: " + (indexX * passoX) + ", z: " + (indexZ * passoZ) + ", y: " + tempY);

            enderecos.Add(posicao);


            //            Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);

            indexX += 1;
            if (indexX > ladoX + colunaExtra)
            {
                indexX = 1;
                indexZ += 1;
                if (indexZ > ladoZ)
                {
                    indexZ = 1;
                }
            }

        }
        colocaPredios();

    }

    public void mapaCirculoAntigo()             //PROCEDIMENTO BASEADO NO RAIO
    {
        //        enderecos = new Vector3[totalCasas + totalEscolas + totalTrabalhos];

//        Debug.Log("LEVEL-MAPA CIRCULO: total de enderecos: " + predios.Count);

        int pontos = predios.Count;
//        Debug.Log("LEVEL-MAPA CIRCULO: pontos: " + pontos);

        Collider terreno = terrain.GetComponent<TerrainCollider>();
        float lateralX = terreno.bounds.max.x - terreno.bounds.min.x;
        float lateralZ = terreno.bounds.max.z - terreno.bounds.min.z;

        float raio = lateralZ;
        if (lateralX < lateralZ)
        {
            raio = lateralX;
        }
        raio = (raio * .85f) / 2;

        float passoRad = 2 * Mathf.PI / pontos;

//        Debug.Log("LEVEL-MAPA CIRCULO: passo(rad): " + passoRad);

        int index = 1;

        Vector3 tvector = new Vector3(0f, 0f, 0f);
        float tempY = Terrain.activeTerrain.SampleHeight(tvector);

        for (int i = 0; i < predios.Count; i++)
        {
//            Debug.Log("LEVEL-MAPA CIRCULO: index x: " + index);
            float x = Mathf.Cos(passoRad * index) * raio + lateralX / 2;
            float z = Mathf.Sin(passoRad * index) * raio + lateralZ / 2;
            Vector3 posicao = new Vector3(x, tempY, z);
//            Debug.Log("LEVEL-MAPA CIRCULO: coordendas: " + posicao);
            //            Debug.Log("LEVEL\MAPA CIRCULO: coordendas\n x: " + (indexX * passoX) + ", z: " + (indexZ * passoZ) + ", y: " + tempY);


            enderecos.Add(posicao);
            //            Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);

            index += 1;

        }
        colocaPredios();
    }

    public void mapaCirculo()           //NOVO CIRCULO, BASEADO NA DISTANCIA ENTRE OS PONTOS
    {
        //        enderecos = new Vector3[totalCasas + totalEscolas + totalTrabalhos];

        //        Debug.Log("LEVEL-MAPA CIRCULO: total de enderecos: " + predios.Count);

        int pontos = predios.Count;
        //        Debug.Log("LEVEL-MAPA CIRCULO: pontos: " + pontos);

        //        Collider terreno = terrain.GetComponent<TerrainCollider>();     //trocar isso por dimensao variavel, de acordo com o total de elementos  -> resolvido
        //        float lateralX = terreno.bounds.max.x - terreno.bounds.min.x;
        //        float lateralZ = terreno.bounds.max.z - terreno.bounds.min.z;

        //float lado = 20;  trocado por 'distanciaPredios'

        float passoRad = 2 * Mathf.PI / pontos; //INVERTER A DEFINICAO. PASSO_RAD = 180-(360/PONTOS)

        float raio = (distanciaPredios / 2)/(passoRad / 2);
        //        Debug.Log("LEVEL-MAPA CIRCULO: passo(rad): " + passoRad);



        terrain.GetComponent<Terrain>().terrainData.size = new Vector3 (raio * 2 + 20, 0, raio*2+20);       //dimensionamento variavel atribuido aki
                                                                                                            //atualizar o terreno para o navmesh
                                                                                                            //        NavMeshSurface superficie = terrain;
                                                                                                            //NavMesh.BuildNavMesh();
//        terrain.GetComponent<NavMeshSurface>().BuildNavMesh();

        Collider terreno = terrain.GetComponent<TerrainCollider>();     
        float lateralX = terreno.bounds.max.x - terreno.bounds.min.x;
        float lateralZ = terreno.bounds.max.z - terreno.bounds.min.z;


        int index = 1;

        Vector3 tvector = new Vector3(0f, 0f, 0f);
        float tempY = Terrain.activeTerrain.SampleHeight(tvector);

        for (int i = 0; i < predios.Count; i++)
        {
            //            Debug.Log("LEVEL-MAPA CIRCULO: index x: " + index);
            float x = Mathf.Cos(passoRad * index) * raio + lateralX / 2;                //  X = DISTANCIA_PADRAO * COS(ANGULO ATUAL * I * PASSO_RAD) 
            float z = Mathf.Sin(passoRad * index) * raio + lateralZ / 2;                // 
            Vector3 posicao = new Vector3(x, tempY, z);
            //            Debug.Log("LEVEL-MAPA CIRCULO: coordendas: " + posicao);
            //            Debug.Log("LEVEL\MAPA CIRCULO: coordendas\n x: " + (indexX * passoX) + ", z: " + (indexZ * passoZ) + ", y: " + tempY);


            enderecos.Add(posicao);
            //            Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);

            index += 1;

        }
        colocaPredios();
    }

    public void mapaCruz()
    {
        //        enderecos = new Vector3[totalCasas + totalEscolas + totalTrabalhos];

//        Debug.Log("LEVEL-MAPA CRUZ: total de enderecos: " + predios.Count);

        int ladoX = (int)Mathf.Ceil(predios.Count / 2);
        int ladoZ = predios.Count - ladoX;
        float sobra = 0;
        //        float sobra = predios.Count - (ladoX * ladoZ);

        //        Debug.Log("lado 1: " + lado1 + "; lado 2: " + lado2 + "; sobra: " + sobra);
        //      Debug.Log("colunas: " + (lado1 + Mathf.Ceil(sobra/lado1)));
        float colunaExtra = Mathf.Ceil(sobra / ladoX);
        //        colunaExtra = colunaExtra / lado1;
        //        Debug.Log("LEVEL-MAPA CRUZ: sobra/lado: " + colunaExtra);
        //        Debug.Log("LEVEL-MAPA CRUZ: pontos x: " + (ladoX + colunaExtra));
        //        Debug.Log("LEVEL-MAPA CRUZ: pontos z: " + (ladoZ));


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///                     INSERIR AJUSTE TE TAMANHO DO TERRENO
        ///                     
        ///             int pontos = predios.Count;
        ///             float passoRad = 2 * Mathf.PI / pontos; //INVERTER A DEFINICAO. PASSO_RAD = 180-(360/PONTOS)
        ///             float raio = (distanciaPredios / 2) / (passoRad / 2);
        ///             
        float x = ladoX * distanciaPredios + distanciaPredios;
        float z = ladoZ * distanciaPredios + distanciaPredios;

        terrain.GetComponent<Terrain>().terrainData.size = new Vector3(x, 0, z);       //dimensionamento variavel atribuido aki
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        Collider terreno = terrain.GetComponent<TerrainCollider>();
        float lateralX = terreno.bounds.max.x - terreno.bounds.min.x;
        float lateralZ = terreno.bounds.max.z - terreno.bounds.min.z;
        float passoX = lateralX / (ladoX + colunaExtra + 1);
        float passoZ = lateralZ / (ladoZ + 1);

//        Debug.Log("LEVEL-MAPA CRUZ: lado x: " + lateralX + "passo x: " + passoX);
//        Debug.Log("LEVEL-MAPA CRUZ: lado z: " + lateralZ + "passo z: " + passoZ);

        //        float posX = passoX;
        //        float posZ = passoZ;
        float posX = passoX;
        float posZ = lateralZ / 2;
        int indexX = 1;
        int indexZ = 1;

        Vector3 tvector = new Vector3((indexX * passoX), 0f, (indexZ * passoZ));
        float tempY = Terrain.activeTerrain.SampleHeight(tvector);

        //        Vector3 novapos = new Vector3(x, tempY, z);

        //        GameObject predio = casa;


        for (int i = 0; i < predios.Count; i++)
        {
            //            Debug.Log("index x: " + indexX + "; index z: " + indexZ);
            //            Vector3 posicao = new Vector3((indexX * posX), tempY, (indexZ * posZ));
            //            Debug.Log("coordendas: " + posicao);
            //            Debug.Log("coordendas\n x: " + (indexX * passoX) + ", z: " + (indexZ * passoZ) + ", y: " + tempY);

            //           Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);

            if (i < ladoX)
            {
//                Debug.Log("LEVEL-MAPA CRUZ: index x: " + indexX + "; index z: " + indexZ);
                Vector3 posicao = new Vector3((indexX * posX), tempY, (indexZ * posZ));
                enderecos.Add(posicao);
                //                Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);
                indexX += 1;
            }
            else
            {
                indexX = 1;
                posX = lateralX / 2;
                posZ = passoZ;

//                Debug.Log("LEVEL-MAPA CRUZ: index x: " + indexX + "; index z: " + indexZ);
                Vector3 posicao = new Vector3((indexX * posX), tempY, (indexZ * posZ));
                enderecos.Add(posicao);
                //                Instantiate(predios[i].tipoPredio, posicao, Quaternion.identity);
                indexZ += 1;
            }

        }
        colocaPredios();
    }



    Vector3 getRandPoint()
    {
        //aki a nao sobreposicao ta precisa, o q permite q um obj entre dentro do outro. refazer de modo a nao criar mais obj dentro de outro

        float maiorpredio = 5f;
//        float x = Random.Range(col.bounds.min.x + maiorpredio, col.bounds.max.x - maiorpredio);
//        float z = Random.Range(col.bounds.min.z + maiorpredio, col.bounds.max.z - maiorpredio);

        Vector3 tvector = new Vector3(0, 0f, 0);
//        float tempY = Terrain.activeTerrain.SampleHeight(tvector);

//        Vector3 novapos = new Vector3(x, tempY, z);
        bool pontosBatem = false;
        //        Vector3 novapos = new Vector3(0, 0, 0);
        int limitador = 0;
        float TempEspacamento = distanciaPredios + maiorpredio;
        float minX = col.bounds.min.x + maiorpredio;
        float maxX = col.bounds.max.x - maiorpredio;
        float minZ = col.bounds.min.z + maiorpredio;
        float maxZ = col.bounds.max.z - maiorpredio;

        if (usedPoints.Count < 1) 
        {
            float x = Random.Range(minX , maxX );
            float z = Random.Range(minZ , maxZ );

            tvector = new Vector3(x, 0f, z);
//            float tempY = Terrain.activeTerrain.SampleHeight(tvector);
//            Vector3 novapos = new Vector3(x, tempY, z);
        }
        else
        {
            do
            {
//                Random.InitState((int)(Mathf.Pow((float)Time.unscaledTimeAsDouble, 2))); ;
//                int seed = Random.Range(0, 124334);
//                Random.seed = seed;
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);

                tvector = new Vector3(x, 0f, z);
                float tempY = Terrain.activeTerrain.SampleHeight(tvector);
                Vector3 novapos = new Vector3(x, tempY, z);
                float tempDist = 0;
                //CHECAR COMO SELECIONAR PONTO NAO REPETIDO FORA DE AREA JA OCUPADA POR OUTRO OBJETO. TVZ BOUNDS SEJA UMA ESTRATEGIA
                foreach (Vector3 p in usedPoints)
                {
                    tempDist = Vector3.Distance(p, tvector);
                    if (tempDist < TempEspacamento)
                    {
                        //                    Debug.Log("distancia entre pontos: " + Vector3.Distance(p, novapos));
                        //                    Debug.Log("distancia padrao: " + (distanciaPredios+maiorpredio));
                        pontosBatem = true;
                        break;
                    }
                }
                limitador++;
                if (pontosBatem == false)
                {
                    Debug.Log("vezes tentadas antes do limite: " + limitador + "distancia: " + tempDist +"espacamento: "+TempEspacamento);
                    break;
                }
                if (limitador > 1600)
                {
                    Debug.Log("limite vezes tentadas: " + limitador + "ultima distancia: " + tempDist + "espacamento: " + TempEspacamento);
                    break;
                }
                //            Debug.Log("ta perto: " + pontosBatem);

            } while (pontosBatem == true);

        }

        //        if (usedPoints.Contains(novapos)) getRandPoint();
        //      foreach (GameObject limites in trabalhos)
        //       {
        //           if (limites.Collider.Contains(novapos)) getRandPoint();
        //                Destroy(limites);
        //       }

        return tvector;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////verificar a sequencia de geracao das coisas. ex: lista de predios, lista de pessoas, atribuir enderecos, instanciar
    /////////////////////////////////////////////// e a relacao disso com os botoes
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////mudar os processos de ir de um lado pra outro na classe PESSSOA pra ficar compativel com o nome sistema de enderecos



    public void colocaPessoas()
    {
        /*
        List<Predios> asCasas = new List<Predios>();
        asCasas.AddRange(predios.FindAll((x) => x.nomePredio.Contains("casa")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
        string tempcasa = "";
        foreach (Predios z in asCasas)
        {
            tempcasa += " " + z.nomePredio;
        }*/
        //        Debug.Log("LEVEL-ATRIBUI PESSOAS:nome das casas: " + tempcasa);

        /*
        List<Predios> osTrabalhos = new List<Predios>();
        osTrabalhos.AddRange(predios.FindAll((x) => x.nomePredio.Contains("trabalho")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao
        string temptrab = "";
        foreach (Predios z in osTrabalhos)
        {
            temptrab += " " + z.nomePredio;
        }*/

        //        Debug.Log("LEVEL-ATRIBUI PESSOAS:nome dos trampos: " + temptrab);

        //        for (int i = 0; i < todasAsPessoas.Count; i++)
        //        {
        //            todasAsPessoas[i].minhaCasa = asCasas[(int)Mathf.PingPong(i, asCasas.Count-1)];
        //            todasAsPessoas[i].meuTrabalho = osTrabalhos[(int)Mathf.PingPong(i, osTrabalhos.Count-1)];
        //          Debug.Log("LEVEL-ATRIBUI PESSOAS: pessoa " + todasAsPessoas[i].identidade + " mora na casa " +todasAsPessoas[i].minhaCasa.nomePredio + "e trabalha no "+ todasAsPessoas[i].meuTrabalho.nomePredio);
        //            Debug.Log("endereco casa " + todasAsPessoas[i].minhaCasa.);
        //        }

//        Debug.Log("LEVEL-ATRIBUI PESSOAS: so perguntando total de gente: " + todasAsPessoas.Count);

        //        Debug.Log("LEVEL-ATRIBUI PESSOAS: so perguntando casa" + todasAsPessoas[1].identidade + " endereco" + todasAsPessoas[1].minhaCasa.enderecoXYZ);
        //        Debug.Log("LEVEL-ATRIBUI PESSOAS: so perguntando nome " + todasAsPessoas[1].identidade + " endereco" + todasAsPessoas[1].minhaCasa.predioPreFab.transform.position);

        string nomedacasa;
        Predios tempcasa;
//        Debug.Log("LEVEL-ATRIBUI PESSOAS: antes do foreach -> total de predios: " + predios.Count);// "endereco via predio:"+ tempcasa.enderecoXYZ);
        foreach (cPessoa tp in todasAsPessoas)
        {
    //        Find((x) => x.name == someString)
            nomedacasa = tp.minhaCasa.nomePredio;
            tempcasa = predios.Find((x) => x.nomePredio == nomedacasa);
//            Debug.Log("LEVEL-ATRIBUI PESSOAS: nome da pessoa: " + tp.identidade + " nome predio: " + tempcasa.nomePredio + ", endereco: "+tempcasa.enderecoXYZ);// "endereco via predio:"+ tempcasa.enderecoXYZ);


//            Debug.Log("LEVEL-ATRIBUI PESSOAS: so perguntando nome " + tp.identidade + " endereco"+tp.minhaCasa.nomePredio+ " " +tp.minhaCasa.enderecoXYZ + "trabalho: "+ tp.meuTrabalho.enderecoXYZ);
//            Debug.Log("LEVEL-ATRIBUI PESSOAS: so perguntando nome predio " + tp.minhaCasa.nomePredio + " endereco" + endtemp);
            //               geral.GetComponent<levelgenerator>().trabalho, tp.minhaCasa.nomePredio)
            //                        asCasas.AddRange(predios.FindAll((x) => x.nomePredio.Contains("casa")));                   // tentando fazer sublist de list. .ADDRANGE resolveu a questao

          Instantiate(tp.tipoPessoa, tp.minhaCasa.enderecoXYZ, Quaternion.identity);
        }
    }
}
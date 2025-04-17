using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ajusteCamera : MonoBehaviour
{

    public List<Transform> alvos;
    //buscar tudo com tag predio
    public Vector3 offset;
    GameObject terreno;

    public void reposicionar()
    {
        terreno = GameObject.Find("Terrain");
        Vector3 centro = centroCam();
        offset = new Vector3(-centro.x - 3, alturaCam() / 3f, -centro.z-10);
        transform.position = centro + offset;
        transform.LookAt (centro);
        Debug.Log("centro: " + centro + " altura: " + offset);

    }

    float alturaCam()
    {
        GameObject terreno = GameObject.Find("Terrain");
        float corda;
        float x = terreno.GetComponent<Terrain>().terrainData.size.x;
        float z = terreno.GetComponent<Terrain>().terrainData.size.z;
        if (z < x) { corda = x; } else { corda = z; }

        float altura = (-corda  / Mathf.Sin(30)/2) ; 
        return altura ;
    }

    
    public void /*Vector3*/ centroCamBounds()
    {
        //        List<GameObject>  /////////EXEMPLO DE BOUNDS/////////////////////////////////////////////////////////////
        /*
        alvos.Clear();
        GameObject[] casas = GameObject.FindGameObjectsWithTag("lugares");
        GameObject[] trabalhos = GameObject.FindGameObjectsWithTag("trabalho");
        if (casas == null || trabalhos == null) 
        {
            Debug.Log("centro bounds: sem objetos");

            return new Vector3(0, 0, 0); 
        }
        Debug.Log("total de centro bounds: " + casas.Length + trabalhos.Length);

        
        foreach (GameObject c in casas)
        {
            alvos.Add(c.transform);
        }
        foreach (GameObject t in trabalhos)
        {
            alvos.Add(t.transform);
        }

        var bounds = new Bounds(alvos[0].transform.position, Vector3.zero);
        foreach (Transform a in alvos)
        {
            bounds.Encapsulate(a.transform.position);
        }
        */ //////////////////////////////////////////////////////////////////////////////////////////////////
    }

    public Vector3 centroCam()
    {

        GameObject terreno = GameObject.Find("Terrain");
//        Debug.Log("posicao do terreno: " + terreno.transform.position);
        //        float corda;

        Vector3 centroTerreno = new Vector3(terreno.GetComponent<Terrain>().terrainData.size.x/2, 0, terreno.GetComponent<Terrain>().terrainData.size.z/2);
        Debug.Log("centro do terreno: " + centroTerreno);

        //Debug.Log("centro bounds: " + bounds.center);
        return centroTerreno;//  bounds.center;
    }


}

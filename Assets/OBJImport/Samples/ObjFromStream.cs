using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ObjFromStream : MonoBehaviour {
    private const string url = "https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj";

    // Start agora é uma coroutine assíncrona
    IEnumerator Start () {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Falha ao baixar OBJ: {www.error}");
                yield break;
            }
#else
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError($"Falha ao baixar OBJ: {www.error}");
                yield break;
            }
#endif

            // create stream and load
            var text = www.downloadHandler.text;
            using (var textStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                try
                {
                    var loadedObj = new OBJLoader().Load(textStream);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Erro ao carregar OBJ: {e}");
                }
            }
        }
    }
}

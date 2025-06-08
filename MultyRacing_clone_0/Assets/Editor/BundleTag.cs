using System.Collections; 
using UnityEngine;
using UnityEngine.Networking;
using FishNet.Object;

[System.Serializable]
public class BundleTag
{
    public string name;
    public string tag;
}

[System.Serializable]
public class BundleTagList
{
    public BundleTag[] bundleTags;
}

public class PlayerTagLoader : NetworkBehaviour
{
    public string playerName = "Player 1";
    public string apiUrl = "http://localhost:4000/api/bundles";

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner) StartCoroutine(GetTagFromServer());
    }

    IEnumerator GetTagFromServer()
    {
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al conectar con API: " + www.error);
        }
        else
        {
            string json = "{\"bundleTags\":" + www.downloadHandler.text + "}";
            var tagList = JsonUtility.FromJson<BundleTagList>(json);

            foreach (var tag in tagList.bundleTags)
            {
                if (tag.name == playerName)
                {
                    Debug.Log($"Jugador {playerName} tiene tag: {tag.tag}");
                    AplicarTag(tag.tag);
                    break;
                }
            }
        }
    }

    void AplicarTag(string tag)
    {
        var rend = GetComponentInChildren<Renderer>();
        if (rend == null) return;

        if (tag == "speedster")
            rend.material.color = Color.red;
        else if (tag == "tank")
            rend.material.color = Color.blue;
        else
            rend.material.color = Color.white;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkConnectionManager : MonoBehaviour
{
    [SerializeField]
    string apiEndpoint = "https://still-maze-hero.herokuapp.com/";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Test()
    {
        UnityWebRequest req = UnityWebRequest.Get(apiEndpoint);
        yield return req.SendWebRequest();

        if(req.isNetworkError || req.isHttpError)
        {
            Debug.LogError(req.error);
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseManager : MonoBehaviour
{

    public GameObject WaitForConnectionMessage;

    public GameObject InternetNotWorking;

    public GameObject NextStep;

    public string apiKey = "AIzaSyC0p3ARMKYIc3hMGz1kX64QsFVlRz6CTD0";

    private string identitytoolkitUri = "https://identitytoolkit.googleapis.com/";

    private string anonymousAuthentication = "/v1/accounts:signUp?key=";

    public string realTimeDatabase ;

    private AuthenticateAnonymouseResponse authResponse = new AuthenticateAnonymouseResponse();

    public ScriptableObject Questionaire;

    public string[] trialSequence;

    // Start is called before the first frame update
    void Start()
    {
        //uncomment to test on a scene 
        //StartCoroutine(Test());

        //attempt to authenticate as soon as the experiment starts
        StartCoroutine(AutenticateAsync());

    }

    IEnumerator Test()
    {
        yield return AutenticateAsync();
        string json = JsonUtility.ToJson(Questionaire);
        yield return SaveData(json);
    }

    IEnumerator AutenticateAsync()
    {   
        string url = identitytoolkitUri + anonymousAuthentication + apiKey;

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"returnSecureToken\":true}");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            WaitForConnectionMessage.SetActive(false);
            InternetNotWorking.SetActive(true);

            Debug.Log("Anonymous Authorization Erro: " + request.error);
        }
        else
        {
            WaitForConnectionMessage.SetActive(false);
            NextStep.SetActive(true);

            Debug.Log("Anonymous Authorization OK");
            Debug.Log("Status Code: " + request.responseCode);
            authResponse = JsonUtility.FromJson<AuthenticateAnonymouseResponse>(request.downloadHandler.text);
            Debug.Log("ID" + authResponse.localId);
        }
     
    }

    public void SaveQuestionaire(DataToCollect questionaire) {

        DataToCollectUpload q = (DataToCollectUpload)questionaire;
        string json = JsonUtility.ToJson(q);
        StartCoroutine(SaveData(json));

    }

    IEnumerator SaveData(string json)
    {
        // if there is no authenticated data than attempt to authenticate 
        if (authResponse.localId == null) yield return AutenticateAsync();
        if (authResponse.localId == null) yield break;

            string url = realTimeDatabase + authResponse.localId + "/data.json?auth=" + authResponse.idToken;

        var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.error != null)
        {

            Debug.Log("Put Data Erro: " + request.error);
        }
        else
        {
            Debug.Log("Put Data OK");
            Debug.Log("Status Code: " + request.responseCode);
        }

    }

   
}

[Serializable]
class AuthenticateAnonymouseResponse
{
    public string kind;
    public string idToken;
    public string refreshToken;
    public int expiresIn;
    public string localId;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;

public class BrowserView : MonoBehaviour {

    const string GOOGLE_SEARCH = "https://www.google.com/search?q=";
    const string URI = "http://198.252.105.8:3000/getImage";

    public Material screenMat;

    Texture2D tempTex;

    private void Start() {
        tempTex = new Texture2D(2, 2);
    }

    public void VisitGoogle() {
        StartCoroutine(GetImageFromURL("http://www.google.com/"));
    }

    public void SearchGoogle(string query) {
        StartCoroutine(GetImageFromURL(GOOGLE_SEARCH + query));
    }

    IEnumerator GetImageFromURL(string url) {
        UnityWebRequest www = UnityWebRequest.Get(URI);
        www.SetRequestHeader("head", url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            tempTex.LoadImage(www.downloadHandler.data);
            tempTex.EncodeToPNG();
            screenMat.mainTexture = tempTex;
        }
    }
}
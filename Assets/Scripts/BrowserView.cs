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
    public Texture whiteTex;

    Texture2D tempTex;

    private void Start() {
        tempTex = new Texture2D(2, 2);
        VisitGoogle();
    }

    public void ProcessQuery(string query) {
        query = query.ToLower();
        //start loading animation
        AnimLoading.Instance.ShouldLoad(true,query);
        screenMat.mainTexture = whiteTex;
        //start loading page
        if (query.Equals("google") || query.Equals("home") || query.Equals("search")) {
            VisitGoogle();
        } else {
            SearchGoogle(query);
        }
    }

    void VisitGoogle() {
        StartCoroutine(GetImageFromURL("http://www.google.com/"));
    }

    void SearchGoogle(string query) {
        string currUrl = GOOGLE_SEARCH + query;
        StartCoroutine(GetImageFromURL(currUrl));
    }

    IEnumerator GetImageFromURL(string url) {
        UnityWebRequest www = UnityWebRequest.Get(URI);
        www.SetRequestHeader("head", url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            tempTex.LoadImage(www.downloadHandler.data);
            screenMat.mainTexture = tempTex;
            AnimLoading.Instance.ShouldLoad(false, "");
        }
    }
}
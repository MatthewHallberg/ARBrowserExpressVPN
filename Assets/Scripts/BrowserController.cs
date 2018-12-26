using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;

public class BrowserController : MonoBehaviour {

    const string GOOGLE_SEARCH = "https://www.google.com/search?q=";
    const string URI_IMAGE = "http://198.252.105.8:3000/getImage";
    const string URI_LINKS = "http://198.252.105.8:12001";

    private void Start() {
        VisitGoogle();
    }

    public void ProcessQuery(string query) {
        StopAllCoroutines();
        query = query.ToLower();
        //start loading page
        if (query.Equals("google") || query.Equals("home") || query.Equals("search")) {
            VisitGoogle();
        } else {
            SearchGoogle(query);
        }
    }

    void VisitGoogle() {
        StartCoroutine(GetImageFromURL("http://www.google.com/", true,string.Empty));
    }

    void SearchGoogle(string query) {
        string currUrl = GOOGLE_SEARCH + query;
        StartCoroutine(GetImageFromURL(currUrl,true,query));
    }

    IEnumerator GetLinksFromQuery(string query) {
        UnityWebRequest www = UnityWebRequest.Get(URI_LINKS);
        www.SetRequestHeader("head", UnityWebRequest.EscapeURL(query));
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            string[] links = www.downloadHandler.text.Split(',');
            foreach (string link in links) {
                if (link.Length > 0) {
                    yield return StartCoroutine(GetImageFromURL(link, false, string.Empty));
                }
            }
        }
    }

    IEnumerator GetImageFromURL(string url,bool isMain, string query) {
        UnityWebRequest www = UnityWebRequest.Get(URI_IMAGE);
        www.SetRequestHeader("head", url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            byte[] imageBytes = www.downloadHandler.data;
            if (isMain) {
                SceneController.Instance.LoadMainTexture(imageBytes);
                if (query != String.Empty)StartCoroutine(GetLinksFromQuery(query));
            } else {
                SceneController.Instance.LoadLink(imageBytes);
            }
        }
    }
}
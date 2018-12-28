using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebController : MonoBehaviour {

    class AndroidPluginCallback : AndroidJavaProxy {
        public AndroidPluginCallback() : base("com.example.matthew.webViewPlugin.PluginCallback") { }

        public void onFrameUpdate(AndroidJavaObject bytesObj) {
            Debug.Log("~~~~~~ENTER callback onSuccess!!!!!!! From Unity!");

            AndroidJavaObject bufferObject = bytesObj.Get<AndroidJavaObject>("Buffer");
            byte[] bytes = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(bufferObject.GetRawObject());

            RawImage rawImage = GameObject.Find("RawImage").GetComponent<RawImage>();
            Texture2D newTex = new Texture2D(200, 300);
            newTex.LoadImage(bytes);
            rawImage.texture = newTex;
            Debug.Log("~~~~~LENGTH: " + bytes.Length);
        }
    }

    AndroidJavaObject activity;
    AndroidJavaObject plugin;
    AndroidJavaObject data;

    private void Start() {
        InitPlugin();
    }

    void InitPlugin() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin = new AndroidJavaObject(
            "com.example.matthew.webViewPlugin.WebBridge");
        }));

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin.Call("init");
        }));
    }

    /// <summary>
    /// gets called via SendMessage from the android plugin
    /// </summary>
    /// <param name="imageString">recognizedText.</param>
    public void OnWebImageReceived(string imageString) {
        Debug.Log("Get image..." + imageString);
        //SceneController.Instance.OnResultRecieved(recognizedText);
    }

    public void GetImageFromURL(string url) {
        // Calls the function from the jar file
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin.Call("GetBitmap", "http://www.area51.org/");
        }));
    }
}

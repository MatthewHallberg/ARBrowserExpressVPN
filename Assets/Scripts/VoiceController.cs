using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour {

    public BrowserView browser;
    public Text debugText;

    AndroidJavaObject activity;
    AndroidJavaObject plugin;

    private void Start() {
        InitPlugin();
    }

    void InitPlugin() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                plugin = new AndroidJavaObject(
                "com.example.matthew.plugin.VoiceBridge");
        }));

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin.Call("StartPlugin");
        }));
    }

    public void OnVoiceResult(string recognizedText) {
//        browser.ProcessQuery(result[0]);
        debugText.text = recognizedText;
    }

    public void GetSpeech() {
        //play loading animation
        //      AnimLoading.Instance.ShouldLoad(true);

        // Calls the function from the jar file
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin.Call("StartSpeaking");
        }));
    }
}

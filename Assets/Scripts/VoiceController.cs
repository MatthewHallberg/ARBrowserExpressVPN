using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour {

    public delegate void OnResultRecieved(string result);
    public static OnResultRecieved resultRecieved;

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

    /// <summary>
    /// gets called via SendMessage from the android plugin
    /// </summary>
    /// <param name="recognizedText">recognizedText.</param>
    public void OnVoiceResult(string recognizedText) {
        Debug.Log(recognizedText);
        resultRecieved?.Invoke(recognizedText);
    }

    /// <summary>
    /// gets called via SendMessage from the android plugin
    /// </summary>
    /// <param name="error">Error.</param>
    public void OnErrorResult(string error) {
        Debug.Log(error);
        SceneController.Instance.OnErrorRecieved(error);
    }

    public void GetSpeech() {
        // Calls the function from the jar file
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            plugin.Call("StartSpeaking");
        }));
    }
}

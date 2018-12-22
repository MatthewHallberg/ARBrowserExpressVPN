using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour {

    public BrowserView browser;
    public Text debugText;

    AndroidJavaClass pluginClass = new AndroidJavaClass("com.plugin.speech.pluginlibrary.TestPlugin");

    private void Start() {
        InitPlugin();
    }

    void InitPlugin() {
        // Pass the name of the game object which has the onActivityResult(string recognizedText) attached to it.
        pluginClass.CallStatic("setReturnObject", "VoiceToText");
        Debug.Log("Return Object Set");


        // Setting language is optional. If you don't run this line, it will try to figure out language based on device settings
        pluginClass.CallStatic("setLanguage", "en_US");
        Debug.Log("Language Set");


        // The following line sets the maximum results you want for recognition
        pluginClass.CallStatic("setMaxResults", 3);
        Debug.Log("Max Results Set");

        // The following line sets the question which appears on intent over the microphone icon
        pluginClass.CallStatic("changeQuestion", "Hello, How can I help you???");
        Debug.Log("Question Set");
    }

    public void onActivityResult(string recognizedText) {
        char[] delimiterChars = { '~' };
        string[] result = recognizedText.Split(delimiterChars);
//        browser.ProcessQuery(result[0]);
        debugText.text = result[0];
    }

    public void GetSpeech() {
        //play loading animation
//      AnimLoading.Instance.ShouldLoad(true);

        // Calls the function from the jar file
        pluginClass.CallStatic("promptSpeechInput");
    }
}

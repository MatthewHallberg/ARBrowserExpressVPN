using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

    public BrowserView browser;
    public VoiceController voiceController;

    public Animation speechButtonAnim;

    public static SceneController Instance { get; private set; }

    bool canSearch = true;

    private void Awake() {
        Instance = this;
    }

    public void StartRecognizingSpeech() {
        if (canSearch) {
            canSearch = false;
            speechButtonAnim.Play();
            voiceController.GetSpeech();
        }
    }

    public void OnResultRecieved(string result) {
        GotGenericResult();
        browser.ProcessQuery(result);
    }

    public void OnErrorRecieved(string error) {
        GotGenericResult();
    }

    void GotGenericResult() {
        canSearch = true;
        StopButtonAnim();
    }

    void StopButtonAnim() {
        speechButtonAnim["buttonAnimation"].time = 0;
        speechButtonAnim.Sample();
        speechButtonAnim.Stop();
    }
}

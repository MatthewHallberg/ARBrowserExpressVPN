using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

    public static SceneController Instance { get; private set; }

    public BrowserController browser;
    public VoiceController voiceController;
    public SecondMonitorBehavior secondMonitor;

    public Transform linkParent;

    public Animation speechButtonAnim;

    public Material screenMat;

    public Texture loadingTexture;

    bool canSearch = true;

    private void Awake() {
        Instance = this;
    }

    private void OnEnable() {
        VoiceController.resultRecieved += OnResultRecieved;
    }

    private void OnDisable() {
        VoiceController.resultRecieved -= OnResultRecieved;
    }

    //call to plugin to start recognizing speech input
    public void StartRecognizingSpeech() {
        if (canSearch) {
            canSearch = false;
            speechButtonAnim.Play();
            voiceController.GetSpeech();
        }
    }

    //voice result receieved
    public void OnResultRecieved(string result) {
        GotGenericResult();
        //start loading animation
        AnimLoading.Instance.ShouldLoad(true, result);
        screenMat.mainTexture = loadingTexture;
        //destroy current links
        foreach (Transform child in linkParent) {
            Destroy(child.gameObject);
        }
        //load browser image
        StopAllCoroutines();
        browser.ProcessQuery(result);
    }
    //voice error recieved
    public void OnErrorRecieved(string error) {
        GotGenericResult();
    }

    public void LoadMainTexture(byte[] imageBytes) {
        StartCoroutine(LoadMainTextureRoutine(imageBytes));
    }

    IEnumerator LoadMainTextureRoutine(byte[] imageBytes) {
        Texture2D tempTex = new Texture2D(1024, 768);
        tempTex.LoadImage(imageBytes);
        Debug.Log("WIDTH: " + tempTex.width + " HEIGHT: " + tempTex.height);
        screenMat.mainTexture = tempTex;
        AnimLoading.Instance.ShouldLoad(false, "");
        yield return new WaitForSeconds(1f);
    }

    public void LoadLink(byte[] imageBytes) {
        StartCoroutine(LoadLinkRoutine(imageBytes));
    }

    IEnumerator LoadLinkRoutine(byte[] imageBytes) {
        yield return new WaitForEndOfFrame();
        GameObject link = Instantiate(Resources.Load("Link") as GameObject, linkParent);
        Texture2D tempTex = new Texture2D(1024, 768);
        tempTex.LoadImage(imageBytes);
        link.GetComponent<Renderer>().material.mainTexture = tempTex;
        yield return new WaitForSeconds(1f);
    }

    //stop playing voice button animation
    void GotGenericResult() {
        canSearch = true;
        speechButtonAnim["buttonAnimation"].time = 0;
        speechButtonAnim.Sample();
        speechButtonAnim.Stop();
        secondMonitor.DeactivateMonitor();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondMonitorBehavior : MonoBehaviour {

    public Renderer rend;
    Vector3 originalSize = new Vector3(.5f, .3f, 1);
    Vector3 desiredScale = Vector3.zero;

    private void Awake() {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 6f);
    }

    public void DeactivateMonitor() {
        desiredScale = Vector3.zero;
    }

    public void ActivateMonitor(Texture tex){
        if (desiredScale == originalSize) transform.localScale = originalSize/1.5f;
        desiredScale = originalSize;
        rend.material.mainTexture = tex;
    }
}

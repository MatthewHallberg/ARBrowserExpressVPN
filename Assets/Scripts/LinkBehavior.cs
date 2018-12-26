using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkBehavior : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip growSound;
    public AudioClip clickSound;

    Vector3 originalSize;

    private void Awake() {
        audioSource.PlayOneShot(growSound);
        originalSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Update() {
        transform.localScale = Vector3.Lerp(transform.localScale, originalSize, Time.deltaTime * 6f);
    }

    private void OnMouseDown() {
        audioSource.PlayOneShot(clickSound);
        transform.localScale = originalSize / 2f;
        Texture thisTexture = GetComponent<Renderer>().material.mainTexture;
        SceneController.Instance.screenMat.mainTexture = thisTexture;
    }
}

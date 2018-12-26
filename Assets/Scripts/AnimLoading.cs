using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimLoading : MonoBehaviour {

    public Transform imgLoading;
    public Transform sectionLoading;
    public TextMeshPro TMPtext;

    float rotateSpeed = 300;
    Vector3 axis = new Vector3(0, 0, 1);

    private static AnimLoading _instance;

    public static AnimLoading Instance {
        get { return _instance; }
    }

    private void Awake() {
        _instance = this;
    }

    public void ShouldLoad(bool load, string text) {
        TMPtext.text = text;
        sectionLoading.gameObject.SetActive(load);
    }

    void Update() {
        if (sectionLoading.gameObject.activeSelf) {
            imgLoading.Rotate(axis * Time.deltaTime * rotateSpeed);
        }
    }
}

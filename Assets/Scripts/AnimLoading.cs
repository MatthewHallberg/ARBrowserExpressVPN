using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimLoading : MonoBehaviour {

    public Transform imgLoading;
    public GameObject SectionLoading;

    float rotateSpeed = 300;
    Vector3 axis = new Vector3(0, 0, 1);

    private static AnimLoading _instance;

    public static AnimLoading Instance {
        get { return _instance; }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void ShouldLoad(bool load) {
        SectionLoading.SetActive(load);
    }

    void Update() {
        if (SectionLoading.activeSelf) {
            imgLoading.Rotate(axis * Time.deltaTime * rotateSpeed);
        }
    }
}

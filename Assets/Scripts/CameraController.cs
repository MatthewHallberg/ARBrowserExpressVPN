using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject arCam, editorCam;

    void Awake() {
#if UNITY_EDITOR
        editorCam.SetActive(true);
        arCam.SetActive(false);
#else
        editorCam.SetActive(false);
        arCam.SetActive(true);
#endif
    }
}

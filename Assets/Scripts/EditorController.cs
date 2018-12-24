using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour {

    public GameObject arCam, editorCam,debugInputField, ARTransform;

    void Awake() {
#if UNITY_EDITOR
        editorCam.SetActive(true);
        arCam.SetActive(false);
#else
        editorCam.SetActive(false);
        arCam.SetActive(true);
        debugInputField.SetActive(false);
        ARTransform.transform.position = new Vector3(-100,0,0);
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkBehavior : MonoBehaviour {

    private void OnMouseDown() {
        Texture thisTexture = GetComponent<Renderer>().material.mainTexture;
        SceneController.Instance.screenMat.mainTexture = thisTexture;
    }
}

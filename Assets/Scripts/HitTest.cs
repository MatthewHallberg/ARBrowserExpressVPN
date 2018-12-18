using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class HitTest : MonoBehaviour {

    public ARSessionOrigin sessionOrigin;
    List<ARRaycastHit> s_RaycastHits = new List<ARRaycastHit>();


    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = Input.mousePosition;
            if (sessionOrigin.Raycast(pos, s_RaycastHits, TrackableType.All)) {
                Pose hitPose = s_RaycastHits[0].pose;
                transform.position = hitPose.position;
                transform.rotation = hitPose.rotation;
            }
        }
    }
}

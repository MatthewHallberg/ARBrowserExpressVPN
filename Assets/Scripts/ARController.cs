namespace GoogleARCore.Examples.HelloAR {
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ARController : MonoBehaviour {
        Camera FirstPersonCamera;

        private void Start() {
            FirstPersonCamera = Camera.main;
        }

        // Update is called once per frame
        void Update() {

            // Check if there is a touch
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                // Check if finger is over a UI element
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                    // Raycast against the location the player touched to search for planes.
                    TrackableHit hit;
                    TrackableHitFlags raycastFilter = TrackableHitFlags.Default;
                    Vector3 pos = Input.mousePosition;
                    if (Frame.Raycast(pos.x, pos.y, raycastFilter, out hit)) {
                        // Use hit pose and camera pose to check if hittest is from the
                        // back of the plane, if it is, no need to create the anchor.
                        if ((hit.Trackable is DetectedPlane) &&
                            Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                                hit.Pose.rotation * Vector3.up) < 0) {
                            Debug.Log("Hit at back of the current DetectedPlane");
                        } else {

                            transform.position = hit.Pose.position;
                            transform.rotation = hit.Pose.rotation;

                            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                            transform.parent = anchor.transform;
                        }
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkMenu : MonoBehaviour {

    const float spacing = .015f;
    Vector3 centerPos = new Vector3(0, .035f, 0);

    private void OnTransformChildrenChanged() {
        bool isEven = transform.childCount % 2 == 0;
        foreach (Transform child in this.transform) {
            if (isEven && child.GetSiblingIndex() == 0) {
                child.localPosition = centerPos;
            }
            Vector3 tempPos = centerPos;
            int index = child.GetSiblingIndex() + 1;
            if (index % 2 == 0) {
                if (isEven) {
                    tempPos.x = (spacing * index) - spacing;
                } else {
                    tempPos.x = (spacing * (index));
                }
            } else {
                if (isEven) {
                    tempPos.x = (spacing * (-index - 1)) + spacing;
                } else {
                    tempPos.x = (spacing * (-index + 1));
                }
            }
            child.localPosition = tempPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableBuilding : MonoBehaviour {
    [HideInInspector]
    public List<Collider> coliders = new List<Collider>();
    private bool isSelected;

    public void SetSelected(bool select) {
        isSelected = select;
    }

    private void OnGUI() {
        if(isSelected) {
            GUI.Button(new Rect(100, 300, 100, 30), name);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Building") {
            coliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Building") {
            coliders.Remove(other);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuilding : MonoBehaviour {
    public GameObject[] buildings;
    private BuildingPlacement buildingPlacement;

    void Start() {
        buildingPlacement = GetComponent<BuildingPlacement>();
    }

    public void SpawnWall() {
        buildingPlacement.SetItem(buildings[0]);
    }
}
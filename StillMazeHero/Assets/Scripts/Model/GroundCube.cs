using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCube : MonoBehaviour {

    public Tile tile { get; set; }

    public Color normalColor;
    public Color highlightColor;
    public Color pathColor;

    public Material grassMat;
    public Material rockMat;

    private Color currentColor;
    private Material[] cachedMaterial;

    private LineRenderer line;
    //private MeshRenderer myRend;

    private void OnEnable() {
        //myRend = GetComponent<MeshRenderer>();
        //normalColor = GetComponent<MeshRenderer>().material.color;
        currentColor = normalColor;
        cachedMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().materials;

        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    public void ChangeMaterial(TileType tt) {
        Material inReplaceMat;
        if (tt == TileType.Floor) {
            inReplaceMat = grassMat;
        }else if (tt == TileType.OuterWall) {
            inReplaceMat = rockMat;
        }else {
            inReplaceMat = grassMat;
        }

        Material[] intMaterials = new Material[cachedMaterial.Length];
        for (int i = 0; i < intMaterials.Length; i++) {
            intMaterials[i] = inReplaceMat;
        }

        transform.GetChild(0).GetComponent<MeshRenderer>().materials = intMaterials;
        
    }


    // Set Selection Color
    public void SetSelection(bool isSelected) {
        if (isSelected) {
            currentColor = highlightColor;
        }
        else {
            currentColor = normalColor;
        }
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = currentColor;
    }

    // Set Pathfinding way Color
    public void UpdatePathfindingGraphic( ) {
        
        if (tile.isPathway) {
            //currentColor = pathColor;
            line.enabled = true;
        }
        else {
            //currentColor = normalColor;
            line.enabled = false;
        }
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = currentColor;
    }
}

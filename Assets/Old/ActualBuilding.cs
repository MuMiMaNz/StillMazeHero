//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ActualBuilding : MonoBehaviour {

//    public Material goodMat;
//    private MeshRenderer meshRend;
//    private Material originalMat;

//    public string buildingName = "";

//    private bool isSelected;

//    private void Start() {
//        meshRend = this.transform.Find("3D").GetComponent<MeshRenderer>();
//        originalMat = this.transform.Find("3D").GetComponent<MeshRenderer>().material;
//    }
//    public void SetSelected(bool select) {
//        //Debug.Log(select);
//        isSelected = select;
//    }

//    public void ChangeColor() {
//        if (isSelected) {
//            meshRend.material = goodMat;
//        }else {
//            meshRend.material = originalMat;
//        }
//    }

//    public string GetBuildingame() {
//        return buildingName;
//    }

//    public void Destroy() {
//        Destroy(gameObject);
//    }

//}

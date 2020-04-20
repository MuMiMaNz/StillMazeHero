using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour {
    private Camera cam;

    private PlaceableBuilding placeableBuilding;
    private PlaceableBuilding placeableBuildingOld;
    private Transform currentBuilding;
    private bool hasPlaced = false ;

    public LayerMask buildingMask;

    void Start() {
        cam = Camera.main;
    }
    
    void Update() {

        //Vector3 m = Input.mousePosition;
        //m = new Vector3(m.x, m.y, transform.position.y);
        //Vector3 p = cam.ScreenToWorldPoint(m);

        if (Input.touchCount > 0) {

            Touch touch = Input.touches[0];
            Vector3 m = touch.position;
            m = new Vector3(m.x, m.y, 20);
            Vector3 p = cam.ScreenToWorldPoint(m);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began && hasPlaced == false) {
                if (currentBuilding != null && !hasPlaced) {
                    currentBuilding.position = new Vector3(p.x, 0, p.z);
                }
                
            }
            else if (touch.phase == TouchPhase.Ended) {
                if (IsValidPosition()) {
                    hasPlaced = true;
                }
            }
            //TODO: Touch on screen outside building and dismiss GUI
            else if (touch.phase == TouchPhase.Began && hasPlaced == true) {
                    RaycastHit hit = new RaycastHit();
                    Ray ray = new Ray(new Vector3(p.x, 8, p.z), Vector3.down);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingMask)) {
                        if (placeableBuildingOld != null) {
                            placeableBuildingOld.SetSelected(false);
                        }
                        hit.collider.gameObject.GetComponent<PlaceableBuilding>().SetSelected(true);
                        placeableBuildingOld = hit.collider.gameObject.GetComponent<PlaceableBuilding>();
                    }
                    else {
                        if (placeableBuildingOld != null) {
                            placeableBuildingOld.SetSelected(false);
                        }
                    }
                }
            
        }
    }

    bool IsValidPosition() {
        if (placeableBuilding.coliders.Count > 0) {
            return false;
        }
        return true;
    }

    public void SetItem(GameObject building) {
        hasPlaced = false;
        currentBuilding = ((GameObject)Instantiate(building)).transform;
        placeableBuilding = currentBuilding.GetComponent<PlaceableBuilding>();
    }
}

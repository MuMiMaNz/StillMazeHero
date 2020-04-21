using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
	World World {
		get { return WorldController.Instance.World; }
	}
	public Camera cam;
    public LayerMask notBuildingLayer;
    public LayerMask buildingLayer;

    public PanelController panelController;

    private GameObject preview;//this is the preview object that you will be moving around in the scene
    private BuildingController buildingController;
    public BuildingGraphicController buildingGraphicCntroller;
    
    private bool isBuilding = false;
	private bool isMove = false;

	private void Update()
    {
        if (Input.touchCount == 1) {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Ended && isBuilding && buildingController.CanBuild())//pressing LMB, and isBuiding = true, and the Preview Script -> canBuild = true
        {
                BuildIt();//then build the thing
            }
                DoRay(touch);
        }
        // Rotate building??? - not for now
        //else if (Input.touchCount == 2) {
        //    Touch touch1 = Input.touches[0];
        //    Touch touch2 = Input.touches[1];
        //    if (touch2.phase == TouchPhase.Began && isBuilding)//for rotation
        //   {
        //        preview.transform.Rotate(0f, 90f, 0f);//spins like a top, in 90 degree turns
        //    }
        //}
    }

    public void CancleBuilding() {
        if (Input.touchCount == 1) {
            //Touch touch = Input.touches[0];
            if ( isBuilding)//stop build
            {
                buildingController = preview.GetComponent<BuildingController>();
                foreach (GroundCube cube in buildingController.cubes) {
                    cube.SetSelection(false);
                }
                StopBuild();
            }
        }
    }

	// Lt build panel On touch button retrun PreviewBuilding
	public void PewviewBuilding(string objType,bool _isMove) {
        
        preview = buildingGraphicCntroller.GetPreviewBuilding(objType);
        preview.transform.SetParent(this.transform, true);
        buildingController = preview.GetComponent<BuildingController>();//grab the script that is sitting on the preview
        buildingController.isBuilding = true;
        isBuilding = true;//we can now build
		isMove = _isMove;
	}

    private void StopBuild()
    {
        buildingController.isBuilding = false;
        Destroy(preview);//get rid of the preview
        preview = null;//not sure if you need this actually
        buildingController = null;//
        isBuilding = false;
        panelController.OpenBuildPanel();
    }

    private void BuildIt()//actually build the thing
    {
        buildingController.TryBuild( );
		if (buildingController.CanBuild()) {
			StopBuild();
		}
    }

    private void DoRay(Touch touch)//simple ray cast from the main camera. Notice there is no range
    {
        Ray ray = cam.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (isBuilding) {
            if (Physics.Raycast(ray, out hit, notBuildingLayer))//notice there is a layer that we are worried about
            {
                PositionPreviewBuilding(hit.point);
            }
        }else {
            SelectBuilding(touch);
            
        }
    }

    private void PositionPreviewBuilding(Vector3 _pos)
    {
        int x = Mathf.RoundToInt(_pos.x);//just round the x,y,z values to the nearest int
        //int y = Mathf.RoundToInt(_pos.y);//personal preferance to comment this out. I hard coded in my y value
        int z = Mathf.RoundToInt(_pos.z);

        preview.transform.position = new Vector3(x, 0, z);//set the previews transform postion to a new Vector3 made up of the x,y,z that you roundedToInt

    }
    
    public bool GetIsBuilding()//just returns the isBuilding bool, so it cant get changed by another script
    {
        return isBuilding;
    }

    // Select placed building
    private void SelectBuilding(Touch touch) {
        Ray ray = cam.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (touch.phase == TouchPhase.Began  ) {
            //Debug.Log("Select :" + buildingController);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingLayer)) {
                if (buildingController != null) {
                    buildingController.SetSelected(false);
                    buildingController.ChangeColor();
                }
				GameObject hitGO = hit.collider.gameObject;
				hitGO.GetComponent<BuildingController>().SetSelected(true);
                buildingController = hitGO.GetComponent<BuildingController>();
                buildingController.ChangeColor();
				//Debug.Log(hitGO.transform.position.x + "," + hitGO.transform.position.z);
				buildingController.SetBuildingTileData(new Tile(World, (int)hitGO.transform.position.x, (int)hitGO.transform.position.z));

				panelController.OpenEditPanel(buildingController);
                panelController.CloseBuildPanel();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, notBuildingLayer)) {
                if (buildingController != null) {
                    buildingController.SetSelected(false);
                    buildingController.ChangeColor();
                }
            }
            else {
                if (buildingController != null) {
                    buildingController.SetSelected(false);
                }
            }
        }
    }

}



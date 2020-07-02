using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
	// This Control how to select Minion&Building in build mode

	World World {
		get { return WorldController.Instance.World; }
	}
	public Camera cam;
    public LayerMask environmentLayer;
    public LayerMask buildingMinionLayer;

	public PanelController panelController;

    private GameObject preview;
    private PreviewController previewController;

    public BuildingGraphicController buildingGraphicCntroller;
	public CharacterGraphicController characterGraphicController;

	private bool isPreviewing = false;
	public bool isOpenEditPanel = false;

	private void Update()
    {
		if (WorldController.Instance.gameMode == GameMode.BuildMode) {
			if (Input.touchCount == 1) {
				Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Ended && isPreviewing && previewController.CanBuild())//pressing LMB, and isBuiding = true, and the Preview Script -> canBuild = true
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
    }

    public void CanclePreviewing() {
        if (Input.touchCount == 1) {
            //Touch touch = Input.touches[0];
            if ( isPreviewing)//stop build
            {
					previewController = preview.GetComponent<PreviewController>();
					foreach (GroundCube cube in previewController.cubes) {
						cube.SetSelection(false);
					}
					StopBuild();
            }
        }
    }

	// Lt build panel On touch button retrun PreviewBuilding
	public void PreviewBuilding(string objType) {
        
        preview = buildingGraphicCntroller.GetPreviewBuilding(objType);
        preview.transform.SetParent(this.transform, true);
        previewController = preview.GetComponent<PreviewController>();//grab the script that is sitting on the preview
        previewController.isPreviewing = true;
        isPreviewing = true;
	}

	public void PreviewMinion(string objType) {

		preview = characterGraphicController.GetPreviewCharacter(objType);
		preview.transform.SetParent(this.transform, true);
		//grab the script that is sitting on the preview
		previewController = preview.GetComponent<PreviewController>();
		previewController.isPreviewing = true;
		isPreviewing = true;
	}

	private void StopBuild()
    {
        previewController.isPreviewing = false;
        Destroy(preview);//get rid of the preview
        preview = null;//not sure if you need this actually
        previewController = null;//
        isPreviewing = false;
        panelController.OpenBuildPanel();
    }

    private void BuildIt()
    {
		// Actual Build the building or minion
		previewController.TryBuild(panelController.buildMode);
		if (previewController.CanBuild()) {
			StopBuild();
		}
		
	}

	private void DoRay(Touch touch)
    {
        Ray ray = cam.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (isPreviewing) {
            if (Physics.Raycast(ray, out hit, environmentLayer))
            {
                PositionPreviewBuilding(hit.point);
            }
        }else {
			if(isOpenEditPanel == false)
				SelectBuilding(touch);
            
        }
    }

    private void PositionPreviewBuilding(Vector3 _pos)
    {
        int x = Mathf.RoundToInt(_pos.x);
        //int y = Mathf.RoundToInt(_pos.y);//personal preferance to comment this out. I hard coded in my y value
        int z = Mathf.RoundToInt(_pos.z);

        preview.transform.position = new Vector3(x, 0, z);

    }
    
    public bool GetIsPreviewing()
    {
        return isPreviewing;
    }

    // Select placed building
    private void SelectBuilding(Touch touch) {
        Ray ray = cam.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (touch.phase == TouchPhase.Began  ) {
            //Debug.Log("Select :" + buildingController);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingMinionLayer)) {
                if (previewController != null) {
                    previewController.SetSelected(false);
                    previewController.ChangeColor();
                }
				GameObject hitGO = hit.collider.gameObject;
				//Debug.Log(hitGO.name);
				hitGO.GetComponent<PreviewController>().SetSelected(true);
                previewController = hitGO.GetComponent<PreviewController>();
                previewController.ChangeColor();

				panelController.CloseBuildPanel();
				panelController.OpenEditPanel(previewController);
				isOpenEditPanel = true;
                
            }
			else if (Physics.Raycast(ray, out hit, Mathf.Infinity, environmentLayer)) {
                if (previewController != null) {
                    previewController.SetSelected(false);
                    previewController.ChangeColor();
                }
            }
            else {
                if (previewController != null) {
                    previewController.SetSelected(false);
                }
            }
        }
    }

}



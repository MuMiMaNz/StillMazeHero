using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
	
	private BuildingController currentSelectBuilding;

	public GameObject BuildModePanel;
	public GameObject PlayModePanel;

	public GameObject buildPanelL;
    public GameObject buildPanelR;
    public GameObject editBuildPanel;
    public Text buildingNameText;
    public Button sellButton;
    public Button moveButton;
    public Button closeEditPanelButton;

    public BuildingSelectController touchController;

    private bool isOpenBuildpanel;

    private void Start() {
        OpenBuildPanel();
        editBuildPanel.SetActive(false);
		SetBuildModePanel(true);
		SetPlayModePanel(false);
	}

	
    public void StartBuild(string objType) {
        touchController.PreviewBuilding(objType);
        if (isOpenBuildpanel) { CloseBuildPanel(); }
        else { OpenBuildPanel(); }
    }

	public void SetBuildModePanel(bool _isActive) {
		BuildModePanel.SetActive(_isActive);
	}
	public void SetPlayModePanel(bool _isActive) {
		PlayModePanel.SetActive(_isActive);
	}

	public void OpenBuildPanel() {
        isOpenBuildpanel = true;
        buildPanelL.SetActive(true);
        buildPanelR.SetActive(false);
    }
    public void CloseBuildPanel() {
        isOpenBuildpanel = false;
        buildPanelL.SetActive(false);
        buildPanelR.SetActive(true);
    }


    public void OpenEditPanel(BuildingController selectBuilding) {
        currentSelectBuilding = selectBuilding;
        buildingNameText.text = currentSelectBuilding.GetBuildingType();
        SetEditPanel();
        editBuildPanel.SetActive(true);
        buildPanelR.SetActive(false);
		buildPanelL.SetActive(false);
    }
    
    private void SetEditPanel( ) {
        if(currentSelectBuilding.buildingType == "Goal") {
            sellButton.gameObject.SetActive(false);
        }
        else {
            sellButton.gameObject.SetActive(true);
            sellButton.onClick.AddListener(delegate { SellBuilding(); });
        }
        
        moveButton.onClick.AddListener(delegate { MoveBuilding(); });
        closeEditPanelButton.onClick.AddListener(delegate { CloseEditPanel(); });
        }

    private void CloseEditPanel() {
        editBuildPanel.SetActive(false);
        OpenBuildPanel();
		touchController.isOpenEditPanel = false;
    }

    private void SellBuilding( ) {
        if(currentSelectBuilding == null) { return; }

		// Select Building at Pivot point (Left button) and remove for multi tile
		//Debug.Log("Sell Building At: " + currentSelectBuilding.building.tile.X + "," + currentSelectBuilding.building.tile.Z);
		Tile t = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(currentSelectBuilding.transform.position.x), Mathf.RoundToInt(currentSelectBuilding.transform.position.z));
		t.building.Deconstruct();

		currentSelectBuilding.Destroy();
        currentSelectBuilding = null;
        editBuildPanel.SetActive(false);
        OpenBuildPanel();
        buildPanelR.SetActive(false);
		touchController.isOpenEditPanel = false;
	}

    private void MoveBuilding() {
        if (currentSelectBuilding == null) { return; }
		Debug.Log(currentSelectBuilding.bldPrototype.objectType);
		// Destroy building and create new preview
		Tile t = WorldController.Instance.World.GetTileAt( Mathf.RoundToInt(currentSelectBuilding.transform.position.x), Mathf.RoundToInt(currentSelectBuilding.transform.position.z));
		Debug.Log("Tile :" + t.X + "," + t.Z);
		Debug.Log(t.building.objectType);
		t.building.Deconstruct();
		touchController.PreviewBuilding(currentSelectBuilding.GetBuildingType());


        currentSelectBuilding.Destroy();
        currentSelectBuilding = null;
        editBuildPanel.SetActive(false);
        buildPanelR.SetActive(false);
		touchController.isOpenEditPanel = false;
	}
}

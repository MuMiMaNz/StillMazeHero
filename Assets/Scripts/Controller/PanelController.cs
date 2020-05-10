using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuildMode { Building, Minion, None }

public class PanelController : MonoBehaviour
{
	
	private PreviewController currentSelectPreivew;

	public GameObject BuildModePanel;
	public GameObject PlayModePanel;

	public GameObject BuildListPanel;
	public GameObject MinionListPanel;
	
    public GameObject cancelPanelR;
    public GameObject editBuildPanel;
    public Text buildingNameText;
    public Button sellButton;
    public Button moveButton;
    public Button closeEditPanelButton;

    public SelectController selectController;

	public BuildMode buildMode { get; protected set; }

    private bool isOpenBuildpanel;

    private void Start() {
		BuildingModeOn();
	}

	public void SetPlayMode() {
		BuildModePanel.SetActive(false);
		PlayModePanel.SetActive(true);
	}
	

	public void BuildingModeOn() {
		BuildModePanel.SetActive(true);
		PlayModePanel.SetActive(false);
		buildMode = BuildMode.Building;
		OpenBuildPanel();
		MinionListPanel.SetActive(false);
		editBuildPanel.SetActive(false);
		BuildListPanel.SetActive(true);
		
	}

	public void MinionModeOn() {
		buildMode = BuildMode.Minion;
		OpenMinionPanel();
	}

	public void OpenMinionPanel() {
		BuildModePanel.SetActive(true);
		PlayModePanel.SetActive(false);

		MinionListPanel.SetActive(true);
		BuildListPanel.SetActive(false); ;
		cancelPanelR.SetActive(false);
		editBuildPanel.SetActive(false);
	}

	public void StartBuild(string objType) {

		if (buildMode == BuildMode.Building)
			selectController.PreviewBuilding(objType);

		else if (buildMode == BuildMode.Minion)
			selectController.PreviewMinion(objType);

		if (isOpenBuildpanel) { CloseBuildPanel(); }
        else { OpenBuildPanel(); }
    }
	
	public void OpenBuildPanel() {
        isOpenBuildpanel = true;
		if (buildMode == BuildMode.Building) {
			BuildListPanel.SetActive(true);
			MinionListPanel.SetActive(false);
		}
		else if (buildMode == BuildMode.Minion) {
			BuildListPanel.SetActive(false);
			MinionListPanel.SetActive(true);
		}
		cancelPanelR.SetActive(false);
	}
    public void CloseBuildPanel() {
        isOpenBuildpanel = false;

		BuildListPanel.SetActive(false);
		MinionListPanel.SetActive(false);

		cancelPanelR.SetActive(true);
    }

    public void OpenEditPanel(PreviewController selectBuilding) {
        currentSelectPreivew = selectBuilding;
        buildingNameText.text = currentSelectPreivew.GetBuildingType();
        SetEditPanel();
        editBuildPanel.SetActive(true);
        cancelPanelR.SetActive(false);
		BuildListPanel.SetActive(false);
    }
    
    private void SetEditPanel( ) {
        if(currentSelectPreivew.previewType == "Goal") {
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
		selectController.isOpenEditPanel = false;
    }

    private void SellBuilding( ) {
        if(currentSelectPreivew == null) { return; }

		// Use RoundToInt for cut point 0.5f
		Tile t = WorldController.Instance.World.GetTileAt(Mathf.RoundToInt(currentSelectPreivew.transform.position.x), Mathf.RoundToInt(currentSelectPreivew.transform.position.z));
		t.building.Deconstruct();

		currentSelectPreivew.Destroy();
        currentSelectPreivew = null;
        editBuildPanel.SetActive(false);
        OpenBuildPanel();
        cancelPanelR.SetActive(false);
		selectController.isOpenEditPanel = false;
	}

    private void MoveBuilding() {
        if (currentSelectPreivew == null) { return; }

		// Destroy building and create new preview
		Tile t = WorldController.Instance.World.GetTileAt( Mathf.RoundToInt(currentSelectPreivew.transform.position.x), Mathf.RoundToInt(currentSelectPreivew.transform.position.z));
		t.building.Deconstruct();

		selectController.PreviewBuilding(currentSelectPreivew.GetBuildingType());

        currentSelectPreivew.Destroy();
        currentSelectPreivew = null;
        editBuildPanel.SetActive(false);
        cancelPanelR.SetActive(false);
		selectController.isOpenEditPanel = false;
	}

	
	

}

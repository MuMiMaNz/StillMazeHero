﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    private BuildingController currentSelectBuilding;

    public GameObject buildPanelL;
    public GameObject buildPanelR;
    public GameObject editBuildPanel;
    public Text buildingNameText;
    public Button sellButton;
    public Button moveButton;
    public Button closeEditPanelButton;

    public TouchController touchController;

    private bool isOpenBuildpanel;

    private void Start() {
        OpenBuildPanel();
        editBuildPanel.SetActive(false);
    }

	// Lt build panel On touch button
    public void StartBuild(string objType) {
        touchController.NewBuild(objType);//this "Starts" a new build in the build system
        if (isOpenBuildpanel) { CloseBuildPanel(); }
        else { OpenBuildPanel(); }
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
    }

    private void SellBuilding( ) {
        if(currentSelectBuilding == null) { return; }

        currentSelectBuilding.Destroy();
        currentSelectBuilding = null;
        editBuildPanel.SetActive(false);
        OpenBuildPanel();
        buildPanelR.SetActive(false);
    }

    private void MoveBuilding() {
        if (currentSelectBuilding == null) { return; }

        touchController.NewBuild(currentSelectBuilding.GetBuildingType());

        currentSelectBuilding.Destroy();
        currentSelectBuilding = null;
        editBuildPanel.SetActive(false);
        buildPanelR.SetActive(false);

    }
}

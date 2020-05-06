using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour {

    World World {
        get { return WorldController.Instance.World; }
    }

    private List<GameObject> gosBumped = new List<GameObject>();//list of all buildings and walls the preview bumped into
    [HideInInspector]
    public List<GroundCube> cubes = new List<GroundCube>();//list of all the ground cubes the preview is sitting ontop of/notice this is a GroundCube type list not a gameobject list 

    public string previewType;
	public BuildMode buildMode;
    public Building bldPrototype { get; protected set; }
	public Minion mnnPrototype { get; protected set; }

	public Material goodMat;
    public Material badMat;
    private List<Material> originalMat = new List<Material>();

    //public int numSpaceNeed;

    private MeshRenderer[] meshRend;
    private bool canBuild = false;

    private bool isSelected = false;
    public bool isPreviewing { protected get; set; }

    private void OnEnable()
    {
        //Debug.Log("Building : " + building.objectType);
        meshRend = this.transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRend) {
            originalMat.Add(mesh.material);
        }
        //originalMat = this.transform.Find("3D").GetComponent<MeshRenderer>().material;
        ChangeColor();

		if (buildMode == BuildMode.Building)
			bldPrototype = World.GetBuildingPrototype(previewType);
		else if (buildMode == BuildMode.Minion)
			mnnPrototype = World.GetMinionPrototype(previewType);
    }

    private void OnTriggerEnter(Collider other)
    {
		//hit a building or minion?
		if (other.CompareTag("Building") || other.CompareTag("Wall") || other.CompareTag("Minion"))
        {
            gosBumped.Add(other.gameObject);
        }
		//hit a ground cube?
		if (other.CompareTag("GroundCube"))
        {
            GroundCube gc = other.GetComponent<GroundCube>();
            cubes.Add(gc);
            if (isPreviewing) {
                if (gc.tile.isPathway == false) {
                    gc.SetSelection(true);
                }
            }
        }
        ChangeColor();
    }
	
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Building") || other.CompareTag("Wall") || other.CompareTag("Minion"))
        {
            gosBumped.Remove(other.gameObject);//----notice we're removing it from the list
        }

        if (other.CompareTag("GroundCube"))
        {
            GroundCube gc = other.GetComponent<GroundCube>();
            cubes.Remove(gc);//----removing it from the list
            if (gc.tile.isPathway == false) {
                gc.SetSelection(false);
            }
        }
        ChangeColor();
    }

    //Only concerned about the obj list (Buildings and Walls) if there is nothing in that list then we can build, if there is even 
    // 1 thing in the list then you cant build
    public void ChangeColor() {

        //Debug.Log("isBuilding :" + isBuilding);
        // Check is preview can build
        if (isPreviewing) {
			int spaceNeed =0;
			if (buildMode == BuildMode.Building)
				spaceNeed = bldPrototype.spaceNeed;
			else if (buildMode == BuildMode.Minion)
				spaceNeed = mnnPrototype.spaceNeed;

			if (gosBumped.Count == 0 && spaceNeed <= cubes.Count){
                foreach (MeshRenderer mesh in meshRend) {
                    mesh.material = goodMat;
                    canBuild = true;
                }
            }
            else{
                foreach (MeshRenderer mesh in meshRend) {
                    mesh.material = badMat;
                    canBuild = false;
                }
            }
        }
        // Check is select Actual building
        else {
            if (isSelected) {
                foreach (MeshRenderer mesh in meshRend) {
                    mesh.material = goodMat;
                }
            }
            else {
                for (int i = 0; i < meshRend.Length; i++) {
                    meshRend[i].material = originalMat[i];
                }
            }
        }
    }

    public void TryBuild(BuildMode buildMode) { 
		     
        Tile t = World.GetTileAt((int)transform.position.x , (int)transform.position.z);

		// Try Build a Building
		if (buildMode == BuildMode.Building) {
			// If move Goal building Use Dummy Goal building and set new Goal tile
			if (previewType == "Goal") {
				World.PlaceBuilding("DummyGoal", t);
				World.SetGoalTile(t);
			}
			else {
				// Normal building Use Dummy building 
				World.PlaceBuilding("DummyBuilding", t);
			}
			// to Check if building this tile not obstruct valid pathfinding
			if (WorldController.Instance.StartPathfinding()) {
				// This Valid Pathfinding
				foreach (GroundCube cube in cubes) {
					cube.SetSelection(false);
				}
				t.building.Deconstruct();
				World.PlaceBuilding(previewType, t);
				canBuild = true;
				//destroy the preview
				Destroy(gameObject);
			}
			else {
				// Invalid Prebuild position (Obstruct pathway)
				// TODO: make Alert Dialog
				t.building.Deconstruct();
				canBuild = false;
			}
		}
		// Try place a Minion
		else if (buildMode == BuildMode.Minion) {
			World.PlaceMinion(previewType,t);
		}
    }

    public bool CanBuild()//just returns the canBuild bool....this is so it cant accidently be changed by another script
    {
        return canBuild;
    }

    public void SetSelected(bool select) {
        isSelected = select;
    }

    public string GetBuildingType() {
        return previewType;
    }

	public void Destroy() {
		//Tile t = WorldController.Instance.World.GetTileAt((int)transform.position.x, (int)transform.position.z);
		//t.building.Deconstruct();
		Destroy(gameObject);
	}
}

    

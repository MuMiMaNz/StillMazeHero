using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour {

    World World {
        get { return WorldController.Instance.World; }
    }

    private List<GameObject> gosBumped = new List<GameObject>();//list of all buildings and walls the preview bumped into
    [HideInInspector]
    public List<GroundCube> cubes = new List<GroundCube>();//list of all the ground cubes the preview is sitting ontop of/notice this is a GroundCube type list not a gameobject list 

    public string buildingType;
    public Building bldPrototype { get; protected set; }
    public Material goodMat;
    public Material badMat;
    private List<Material> originalMat = new List<Material>();

    //public int numSpaceNeed;

    private MeshRenderer[] meshRend;
    private bool canBuild = false;

    private bool isSelected = false;
    public bool isBuilding { protected get; set; }

    private void OnEnable()
    {
        //Debug.Log("Building : " + building.objectType);
        meshRend = this.transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRend) {
            originalMat.Add(mesh.material);
        }
        //originalMat = this.transform.Find("3D").GetComponent<MeshRenderer>().material;
        ChangeColor();
        bldPrototype = World.GetBuildingPrototype(buildingType);

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Building trigger collider");
        if(other.CompareTag("Building") || other.CompareTag("Wall"))//hit a building or a wall?...... 
        {
            gosBumped.Add(other.gameObject);//then stick it in the obj list.....
        }

        if (other.CompareTag("GroundCube"))//hit a ground cube?.........
        {
            GroundCube gc = other.GetComponent<GroundCube>();//get the ground cube script that is sitting on this particular gameobject.....
            cubes.Add(gc);//add it to the ground cubes list....
            if (isBuilding) {
                if (gc.tile.isPathway == false) {
                    gc.SetSelection(true);//toggle the selection color of this particular ground cube......
                }
            }
        }
        ChangeColor();//<----no check if the color should be green or red
    }

    //----this is the exact opposit of OnTriggerEnter
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Building") || other.CompareTag("Wall"))
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
        if (isBuilding) {
            if (gosBumped.Count == 0 && bldPrototype.spaceNeed <= cubes.Count){
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

	public void SetBuildingTileData(Tile t) {
		bldPrototype.SetTile( t);
		
	}

    public void TryBuild() {      
        Tile t = World.GetTileAt((int)transform.position.x , (int)transform.position.z);

        // If move Goal building Use Dummy Goal building and set new Goal tile
        if (buildingType == "Goal") {
			World.PlaceBuilding("DummyGoal", t);
            World.SetGoalTile(t);
        }else {
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
            WorldController.Instance.World.PlaceBuilding(buildingType, t);
            canBuild = true;

            //destroy the preview
            Destroy(gameObject);
        }else {
			// Invalid Prebuild position (Obstruct pathway)
			// TODO: make Alert Dialog
			t.building.Deconstruct();
			canBuild = false;
        }

    }

    public bool CanBuild()//just returns the canBuild bool....this is so it cant accidently be changed by another script
    {
        return canBuild;
    }

    ////////////////////////////////////////////////////

    public void SetSelected(bool select) {
        //Debug.Log(select);
        isSelected = select;
    }

    public string GetBuildingType() {
        return buildingType;
    }

    public void Destroy() {
        Tile t = WorldController.Instance.World.GetTileAt((int)transform.position.x, (int)transform.position.z);
        t.RemoveBuilding();
        Destroy(gameObject);
    }
}

    

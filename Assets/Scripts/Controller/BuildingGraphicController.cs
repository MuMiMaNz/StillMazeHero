using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGraphicController : MonoBehaviour {
    World World {
        get { return WorldController.Instance.World; }
    }
	
    Dictionary<Building, GameObject> buildingGameObjectMap;
    Dictionary<string, GameObject> buildingGOS;

    public SelectController touchController;

    void Start() {
        
        buildingGameObjectMap = new Dictionary<Building, GameObject>();

        LoadPrefabs();

        // Register our callback so that our GameObject gets updated whenever
        // the tile's type changes.
        World.RegisterBuildingCreated(OnBuildingCreated);

        // Go through any EXISTING building (i.e. from a save that was loaded OnEnable) and call the OnCreated event manually
        foreach (Building bld in World.buildings) {
            OnBuildingCreated(bld);
        }
    }
    
    //  Load 3D Game Object Here
    void LoadPrefabs() {
        buildingGOS = new Dictionary<string, GameObject>();
        GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs/Buildings/");
        
        foreach (GameObject go in gos) {
            //Debug.Log("LOADED RESOURCE:" + go);
            buildingGOS[go.name] = go;
        }
    }

	// Change building collider to real physics
	public void ChangeBuildingPhysic(bool realPhysics) {
		foreach (Building bld in World.buildings) {
			GameObject bld_go = buildingGameObjectMap[bld];
			bld_go.GetComponent<Collider>().isTrigger = !realPhysics;
			bld_go.GetComponent<Rigidbody>().useGravity = realPhysics;
		}
	}

    public void OnBuildingCreated(Building bld) {
        //Debug.Log("OnBuildingCreated");
        // Create a visual GameObject linked to this data.

        // FIXME: Does not consider multi-tile objects nor rotated objects

        // This creates a new GameObject and adds it to our scene.
        GameObject bld_go = GetGOforBuilding(bld);

        if (bld_go != null) {
            // Add our tile/GO pair to the dictionary.
            buildingGameObjectMap.Add(bld, bld_go);

            bld_go.name = bld.objectType + "_" + bld.tile.X + "_" + bld.tile.Z;
            bld_go.transform.position = new Vector3(bld.tile.X, 0, bld.tile.Z);
            bld_go.transform.SetParent(this.transform.Find(bld.parent), true);

            // Register our callback so that our GameObject gets updated whenever
            // the object's into changes.
            bld.RegisterOnChangedCallback(OnBuildingChanged);
			bld.RegisterOnRemovedCallback(OnBuildingRemoved);
        }

    }

	void OnBuildingRemoved(Building bld) {
		if (buildingGameObjectMap.ContainsKey(bld) == false) {
			Debug.LogError("OnFurnitureRemoved -- trying to Remove graphic for building not in our map.");
			return;
		}

		GameObject bld_go = buildingGameObjectMap[bld];
		Destroy(bld_go);
		buildingGameObjectMap.Remove(bld);
		World.InvalidateTileGraph();
		WorldController.Instance.StartPathfinding();
	}

	void OnBuildingChanged(Building bld) {
        //Debug.Log("OnBuildingChanged");
        // Make sure the building's graphics are correct.
        if (buildingGameObjectMap.ContainsKey(bld) == false) {
            Debug.LogError("OnBuildingChanged -- trying to change visuals for building not in our map.");
            return;
        }

        GameObject bld_goOld = buildingGameObjectMap[bld];
        //Debug.Log(bld_goOld);

        GameObject bld_goNew = GetGOforBuilding(bld);
        bld_goNew.name = bld_goOld.name;
        bld_goNew.transform.position = bld_goOld.transform.position;
        bld_goNew.transform.SetParent(this.transform.Find(bld.parent), true);

        Destroy(bld_goOld);

        // --- Delete Old and Add New Building game object
        buildingGameObjectMap.Remove(bld);
        buildingGameObjectMap.Add(bld, bld_goNew);
    }

    public GameObject GetPreviewBuilding(string objectType) {
        return Instantiate(buildingGOS[objectType]);
    }

    private GameObject GetGOforBuilding(Building bld) {

        if (bld.objectType == "DummyBuilding")
            return null;

        if (bld.linksToNeighbour == false) {
            return Instantiate(buildingGOS[bld.objectType]);
        }

        // Otherwise, the sprite name is more complicated.

        string nbPos = "_";

        // Check for neighbours North, East, South, West
        int x = bld.tile.X;
        int z = bld.tile.Z;

        Tile t;

        t = World.GetTileAt(x, z + 1);
        if (t != null && t.building != null && t.building.objectType == bld.objectType) {
            nbPos += "N";
        }
        t = World.GetTileAt(x + 1, z);
        if (t != null && t.building != null && t.building.objectType == bld.objectType) {
            nbPos += "E";
        }
        t = World.GetTileAt(x, z - 1);
        if (t != null && t.building != null && t.building.objectType == bld.objectType) {
            nbPos += "S";
        }
        t = World.GetTileAt(x - 1, z);
        if (t != null && t.building != null && t.building.objectType == bld.objectType) {
            nbPos += "W";
        }

        //  the string will look like:
        //       Wall_NESW

        switch (nbPos) {
            case "_":
                return Instantiate(buildingGOS[bld.objectType]);

            case "_N":
                return Instantiate(buildingGOS[bld.objectType], transform.position, Quaternion.Euler(0, 90, 0));

            case "_E":
                return Instantiate(buildingGOS[bld.objectType], transform.position, Quaternion.Euler(0, 0, 0));

            case "_S":
                return Instantiate(buildingGOS[bld.objectType], transform.position, Quaternion.Euler(0, 90, 0));

            case "_W":
                return Instantiate(buildingGOS[bld.objectType], transform.position, Quaternion.Euler(0, 0, 0));

            case "_NE":
                return Instantiate(buildingGOS[bld.objectType + "_Corner"], transform.position, Quaternion.Euler(0, 0, 0));

            case "_NES":
                return Instantiate(buildingGOS[bld.objectType + "_Intersec"], transform.position, Quaternion.Euler(0, 90, 0));

            case "_NEW":
                return Instantiate(buildingGOS[bld.objectType + "_Intersec"], transform.position, Quaternion.Euler(0, 0, 0));

            case "_NESW":
                return Instantiate(buildingGOS[bld.objectType + "_Cross"], transform.position, Quaternion.Euler(0, 0, 0));

            case "_NS":
                return Instantiate(buildingGOS[bld.objectType], transform.position, Quaternion.Euler(0, 90, 0));

            case "_NSW":
                return Instantiate(buildingGOS[bld.objectType + "_Intersec"], transform.position, Quaternion.Euler(0, 270, 0));

            case "_NW":
                return Instantiate(buildingGOS[bld.objectType + "_Corner"], transform.position, Quaternion.Euler(0, 270, 0));

            case "_ES":
                return Instantiate(buildingGOS[bld.objectType + "_Corner"], transform.position, Quaternion.Euler(0, 90, 0));

            case "_ESW":
                return Instantiate(buildingGOS[bld.objectType + "_Intersec"], transform.position, Quaternion.Euler(0, 180, 0));

            case "_EW":
                return Instantiate(buildingGOS[bld.objectType], transform.position, Quaternion.Euler(0, 0, 0));

            case "_SW":
                return Instantiate(buildingGOS[bld.objectType + "_Corner"], transform.position, Quaternion.Euler(0, 180, 0));

            default:
                Debug.LogError("GetGOForInstalledObject -- No GameObject" + bld.objectType + "in position: " + nbPos);
                return null;
        }
    }
}

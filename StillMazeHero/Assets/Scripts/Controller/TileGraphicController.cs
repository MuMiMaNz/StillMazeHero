using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraphicController : MonoBehaviour
{
    World World {
        get { return WorldController.Instance.World; }
    }

    public Dictionary<Tile, GameObject> tileGameObjectMap { get; protected set; }
    Dictionary<string, GameObject> tileGOS;

    void Start() {
        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        LoadTilePrefabs();

        for (int x = 0; x < World.Width; x++) {
            for (int z = 0; z < World.Height; z++) {
                // Get the tile data
                Tile tile_data = World.GetTileAt(x, z);
				GameObject tile_go = GetTileGO("GroundCube");
				tile_go.GetComponent<GroundCube>().tile = tile_data;
				tile_go.transform.position = new Vector3(tile_data.X, -1, tile_data.Z);
				tile_go.name = "Tile_" + tile_data.X + "_" + tile_data.Z;

				tile_go.transform.SetParent(this.transform.Find("Tiles"), true);

				// Add our tile/GO pair to the dictionary.
				tileGameObjectMap.Add(tile_data, tile_go);

				OnTileChanged(tile_data);
			}
        }
        World.RegisterTileChanged(OnTileChanged);
		if (WorldController.Instance._StartWithPathfind) {
			WorldController.Instance.StartPathfinding();
		}
    }

    private void LoadTilePrefabs() {
        tileGOS = new Dictionary<string, GameObject>();
        GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs/Tiles/");
        
        foreach (GameObject go in gos) {
            //Debug.Log("LOADED RESOURCE:" + go);
            tileGOS[go.name] = go;
        }
    }


    private GameObject GetTileGO(string tileGOname) {
        return Instantiate(tileGOS[tileGOname]);
    }

    // This function should be called automatically whenever a tile's type gets changed.
    void OnTileChanged(Tile tile_data) {

        //Debug.Log(tile_data.Type);

        if (tileGameObjectMap.ContainsKey(tile_data) == false) {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null) {
            Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        if (tile_data.Type == TileType.Floor) {
            tile_go.GetComponent<GroundCube>().ChangeMaterial(TileType.Floor);
        }
        else if (tile_data.Type == TileType.OuterWall) {
            tile_go.GetComponent<GroundCube>().ChangeMaterial(TileType.OuterWall);
        }
        else if (tile_data.Type == TileType.Empty) {
            tile_go.GetComponent<GroundCube>().ChangeMaterial(TileType.Empty);
        }
        else {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }
}

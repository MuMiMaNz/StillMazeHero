using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    public int Width = 14;
    public int Height = 14;

    // The world and tile data
    public World World { get; protected set; }

    public TileGraphicController tileGraphicController;

    private Path_AStar pathAStar;
    //private Path_AStar oldPathAStar;

    static bool loadWorld = false;
	public bool _StartWithPathfind { get; protected set; }

    // Use this for initialization
    void OnEnable() {

        if (Instance != null) {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        if (loadWorld) {
            loadWorld = false;
            CreateWorldFromSaveFile();
			_StartWithPathfind = true;
		}
        else {
            CreateEmptyWorld();
			_StartWithPathfind = false;
		}
       
    }

	void Update() {
        // TODO: Add pause/unpause, speed controls, etc...
        World.Update(Time.deltaTime);

    }

    // Gets the tile at the unity-space coordinates
    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int z = Mathf.FloorToInt(coord.z);

        return World.GetTileAt(x, z);
    }

    public void NewWorld() {
        Debug.Log("NewWorld button was clicked.");

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		World.PlaceOuterWalledWithTiles();
		StartPathfinding();
    }

    public void SaveWorld() {
        Debug.Log("SaveWorld button was clicked.");

        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, World);
        writer.Close();

        Debug.Log(writer.ToString());

        PlayerPrefs.SetString("SaveGame00", writer.ToString());

    }

    public void LoadWorld() {
        Debug.Log("LoadWorld button was clicked.");

        // Reload the scene to reset all data (and purge old references)
        loadWorld = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void CreateEmptyWorld() {
        // Create a world with Empty tiles
        World = new World(10,10);

        // Center the Camera
        Camera.main.transform.position = new Vector3(World.Width / 2,8f, -0.5f);

    }
    void CreateWorldFromSaveFile() {
        Debug.Log("CreateWorldFromSaveFile");
        // Create a world from our save file data.

        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
        Debug.Log(reader.ToString());
        World = (World)serializer.Deserialize(reader);
        reader.Close();
		
        // Center the Camera
        Camera.main.transform.position = new Vector3(World.Width / 2, 8f, -0.5f);

    }

    public bool StartPathfinding() {
        Debug.Log("Start Path Finding");

        if (World.startTile == null || World.goalTile == null) {
            Debug.LogError("No Start or Goal tile");
            return false;
        }

        Path_AStar oldPathAStar = pathAStar;

        // Update old path way to not a pathway
        if (oldPathAStar != null) {
                foreach (Tile t in oldPathAStar.Path()) {
                    //Debug.Log("Old Path:"+ tileGraphicController.tileGameObjectMap[t] );
                    t.isPathway = false;
                    tileGraphicController.tileGameObjectMap[t].GetComponent<GroundCube>().UpdatePathfindingGraphic();
             }
         }
        // Generate New pathway
        pathAStar = new Path_AStar(World, World.startTile, World.goalTile);
        if (pathAStar.Length() == 0) {
            // No valid Pathfinding way
            Debug.LogError("Path_AStar returned no path to destination!");
            pathAStar = null;
            return false;
        }
        else {
            foreach (Tile t in pathAStar.Path()) {
                t.isPathway = true;
                //Debug.Log(t.X + ","+ t.Z);
                //Debug.Log(tileGraphicController.tileGameObjectMap[t]);
                tileGraphicController.tileGameObjectMap[t].GetComponent<GroundCube>().UpdatePathfindingGraphic();
            }
            return true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public enum GameMode { BuildMode, PlayMode }

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
	public World World { get; protected set; }

	public CameraController cameraController;
	public PlayerController playerControllerl;
	public PanelController panelController;
	public BuildingGraphicController buildingGraphicController;
	public TileGraphicController tileGraphicController;

	public int Width = 14;
    public int Height = 14;

	public GameMode gameMode { get; protected set; }

	private Path_AStar pathAStar;

    static bool loadWorld = false;
	public bool _StartWithPathfind { get; protected set; }

    // Use this for initialization
    void Awake() {

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

		gameMode = GameMode.BuildMode;
    }

	void Update() {
        // TODO: Add pause/unpause, speed controls, etc...
        World.Update(Time.deltaTime);
		//cameraController.Update();
		//movementController.Update(Time.deltaTime);
	}

    // Gets the tile at the unity-space coordinates
    public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int z = Mathf.FloorToInt(coord.z);

        return World.GetTileAt(x, z);
    }

	public void PlayItDude() {
		World.CreatePlayerAtStart();
		gameMode = GameMode.PlayMode;
		panelController.SetPlayMode();
		cameraController.SetPlayModeCam();
		playerControllerl.SeekPlayerGO();
		// Change Building to Real physics
		buildingGraphicController.ChangeBuildingPhysic(true);
		// All Minions start to set Patrol point
		MinionsSetPatrolPoint();
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
        //Debug.Log("LoadWorld button was clicked.");

        // Reload the scene to reset all data (and purge old references)
        loadWorld = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void CreateEmptyWorld() {
        // Create a world with Empty tiles
        World = new World(10,10);

    }

    void CreateWorldFromSaveFile() {
        Debug.Log("CreateWorldFromSaveFile");
        // Create a world from our save file data.

        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
        Debug.Log(reader.ToString());
        World = (World)serializer.Deserialize(reader);
        reader.Close();


    }

	// Start Pathfinding from Start to Goal tile
    public bool StartPathfinding() {
        Debug.Log("Start Path Finding");

        if (World.startTile == null || World.goalTile == null) {
            Debug.LogError("No Start or Goal tile");
            return false;
        }

        Path_AStar oldPathAStar = pathAStar;

        // Update old path way not valid anymore
        if (oldPathAStar != null) {
           foreach (Tile t in oldPathAStar.Path()) {
           //Debug.Log("Old Path:"+ tileGraphicController.tileGameObjectMap[t] );
				t.isPathway = false;
				tileGraphicController.tileGameObjectMap[t].GetComponent<GroundCube>().UpdatePathfindingGraphic();
             }
         }
        // Generate New pathway
        pathAStar = new Path_AStar(World.tileGraph, World.startTile, World.goalTile,0,World.Width-1,0,World.Height-1);
        if (pathAStar.Length() == 0) {
            // No valid Pathfinding way
            Debug.LogError("Path_AStar returned no path to destination!");
            pathAStar = null;
            return false;
        }
        else {
            foreach (Tile t in pathAStar.Path()) {
                t.isPathway = true;
                tileGraphicController.tileGameObjectMap[t].GetComponent<GroundCube>().UpdatePathfindingGraphic();
            }
            return true;
        }
    }

	private void MinionsSetPatrolPoint() {
		foreach(Minion m in World.minions) {
			m.SetValidPatrolPoints(World);
		}
	}

}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

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
	public CharacterGraphicController characterGraphicController;

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
		//Debug.Log(World.tileGraph);

		if (gameMode == GameMode.PlayMode)
			World.UpdateInPlayMode(Time.deltaTime);

		if (gameMode == GameMode.BuildMode)
			World.UpdateInBuildMode(Time.deltaTime);
	}

	private void FixedUpdate() {
		if (gameMode == GameMode.PlayMode)
			World.FixedUpdateInPlayMode(Time.deltaTime);
	}

	// Gets the tile at the unity-space coordinates
	public Tile GetTileAtWorldCoord(Vector3 coord) {
        int x = Mathf.FloorToInt(coord.x);
        int z = Mathf.FloorToInt(coord.z);

        return World.GetTileAt(x, z);
    }

	// Press the mo.fak Play button
	public void PlayItDude() {
		World.CreatePlayerAtStart();
		gameMode = GameMode.PlayMode;
		panelController.SetPlayMode();
		cameraController.SetPlayModeCam();
		playerControllerl.StartPlayMode();
		// Change Building to Real physics
		buildingGraphicController.ChangeBuildingPhysic(true);
		// All Minions start to set Patrol point
		MinionsInPlayMode();
	}

    public void NewWorld() {
        Debug.Log("NewWorld button was clicked.");

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		World.PlaceOuterWalledWithTiles();
		StartPathfinding();
    }

    public void SaveWorld() {
        Debug.Log("SaveWorld button was clicked.");

		PlayerPrefs.SetString("SaveGame00", World.SaveJSON() );

		//XmlSerializer serializer = new XmlSerializer(typeof(World));
		//TextWriter writer = new StringWriter();
		//serializer.Serialize(writer, World);
		//writer.Close();

		//Debug.Log(writer.ToString());

		//PlayerPrefs.SetString("SaveGame00", writer.ToString());

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

		string json = PlayerPrefs.GetString("SaveGame00", null);
		if (string.IsNullOrEmpty(json) == true) {
			Debug.LogError("No Save file JSON"); 
				return;
		}
		WorldSaveObject wso = JsonConvert.DeserializeObject<WorldSaveObject>(json);
		World = new World(wso.Width, wso.Height);
		World.LoadWSO(wso);
		//XmlSerializer serializer = new XmlSerializer(typeof(World));
		//TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
		//Debug.Log(reader.ToString());
		//World = (World)serializer.Deserialize(reader);
		//reader.Close();
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
        pathAStar = new Path_AStar(true, World.startTile, World.goalTile);
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

	private void MinionsInPlayMode() {
		foreach(Minion m in World.minions) {
			m.SetPlayMode(World);
		}
		//characterGraphicController.SetMinionsPlayMode();
	}

}

﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class World : IXmlSerializable {

    // A two-dimensional array to hold our tile data.
    Tile[,] tiles;

	// Array use on Save/Load
	public List<Building> buildings { get; protected set; }
	public List<Enemy> enemies { get; protected set; }
	public Player player {  get; protected set; }

	// The pathfinding graph used to navigate our world map.
	public Path_TileGraph tileGraph;
    public Tile startTile { get; protected set; }
    public Tile goalTile { get; protected set; }

    // Store building prototype data
    Dictionary<string, Building> buildingPrototypes;
	Dictionary<string, Enemy> enemyPrototypes;
	Dictionary<string, Player> playerPrototypes;
	Dictionary<string, Weapon> weaponPrototypes;

	Action<Building> cbBuildingCreated;
    Action<Player> cbPlayerCreated;
	Action<Enemy> cbEnemyCreated;
	Action<Tile> cbTileChanged;


    public int Width { get; protected set; }
    public int Height { get; protected set; }

    // Initializes a new instance of the World class.
    public World(int _width , int _height ) {

        SetupWorld(_width, _height);
    }

    private void SetupWorld(int width,int height) {

        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++) {
            for (int z = 0; z < Height; z++) {

                tiles[x, z] = new Tile(this, x, z);
                // Set TileType
                tiles[x, z].RegisterTileTypeChangedCallback(OnTileChanged);
            }
        }
        Debug.Log("World created with " + (Width * Height) + " tiles.");
        CreateBuildingPrototypes();
		CreateWeaponPrototypes();
		//CreateCharacterPrototypes();


		enemies = new List<Enemy>();
		buildings = new List<Building>();
    }

    // Set Square OuterWall TileType
    public void PlaceOuterWalledWithTiles() {
        
        for (int x = 0; x < Width; x++) {
            for (int z = 0; z < Height; z++) {

                Tile tile_data = GetTileAt(x, z);

                if (x == 0 || z == 0 || x == Width - 1 || z == Height - 1) {
                    // Set Outer wall tile type 
                    tiles[x, z].Type = TileType.OuterWall;

                    // Place Outer gate building with start tile
                    if (tile_data.Z == 0 && tile_data.X == Width / 2) {
                        PlaceBuilding("OuterWall_Gate", tile_data);
                        
                        
                    } // Empty at left&right of Gate center pivot
                    else if (tile_data.Z == 0 && (tile_data.X == (Width / 2) - 1 || tile_data.X == (Width / 2) + 1)) {

                    }// Outer Wall
                    else if (tile_data.Z == 0 || tile_data.Z == Height - 1 || tile_data.X == 0 || tile_data.X == Width - 1) {
                        PlaceBuilding("OuterWall", tile_data);
                    }
                }
                else {
                    // Set Floor tile type
                    tiles[x, z].Type = TileType.Floor;
                    // Goal building with goal tile
                    if (tile_data.Z == Height - 3 && tile_data.X == Width / 2) {
                        PlaceBuilding("Goal", tile_data);
                        
                    }
                }
            }
        }
    }

	public Player CreatePlayerAtStart() {
		Debug.Log("CreatePlayer");
		Player p = PlacePlayer(startTile);

		player = p;
		Debug.Log("Player Name : " + player.name);
		Debug.Log("Player's Weapon : " + player.weapons[0].wName);

		if (cbPlayerCreated != null)
			cbPlayerCreated(p);

		return p;
	}
	 

	public void Update(float deltaTime) {
		foreach (Enemy e in enemies) {
			e.Update(deltaTime);
		}

		foreach (Building b in buildings) {
            b.Update(deltaTime);
        }

    }


    // Set the Goal tile
    public void SetGoalTile(Tile t) {
        Debug.Log("Change Goal Tile to" + t.X + "," + t.Z);
        goalTile = t;
    }

    // Gets the tile data at x and y.
    public Tile GetTileAt(int x, int z) {
        if (x >= Width || x < 0 || z >= Height || z < 0) {
            //Debug.LogError("Tile (" + x + "," + z + ") is out of range.");
            return null;
        }
        return tiles[x, z];
    }




	#region Prototypes



	// All Enemy prototypes data
	//void CreateEnemyPrototypes() {
	//	enemyPrototypes = new Dictionary<string, Enemy>();

	//	enemyPrototypes.Add("Enemy",
	//		new Enemy("Enemy", // Name
	//						100f, // HP
	//						1, //Speed
	//						"Enemy" // Parent
	//						)
	//	);
	//}

	void CreateWeaponPrototypes() {
		weaponPrototypes = new Dictionary<string, Weapon>();

		weaponPrototypes.Add("RedTearSword",
			new Weapon("RedTearSword", // Object type
				"RedTearSword+2", // Name
						"You look at this Bad ass sword and cry in blood", // Description
						10, //ATK
						0, // mATK
						5, // ATKspeed
						0, // DEF
						0, // mDEF
						WeaponType.OneHandMelee,
						1 // Slot
							)
		);
	}

	// All Building prototypes data
	void CreateBuildingPrototypes() {
        buildingPrototypes = new Dictionary<string, Building>();

        buildingPrototypes.Add("InnerWall",
            new Building(
                                "InnerWall",
                                0,  // Impassable
                                1,  // Width
                                1,  // Height
                                "Buildings", // Parent
                                true // Links to neighbours and "sort of" becomes part of a large object
                            )
        );
        buildingPrototypes.Add("OuterWall",
            new Building(
                                "OuterWall",
                                0,  // Impassable
                                1,  // Width
                                1,  // Height
                                "OuterWalls",// Parent
                                true // Links to neighbours and "sort of" becomes part of a large object
                            )
        );
        buildingPrototypes.Add("OuterWall_Gate",
           new Building(
                                "OuterWall_Gate",
                                0,  // Impassable
                                3,  // Width
                                1,  // Height
                                "OuterWalls",// Parent
                                false // Links to neighbours and "sort of" becomes part of a large object
                            )
        );
        buildingPrototypes.Add("Goal",
            new Building(
								"Goal",
                                1,  // passable
                                1,  // Width
                                1,  // Height
                                "Buildings",// Parent
                                false // Links to neighbours and "sort of" becomes part of a large object
                            )
        );
        buildingPrototypes.Add("DummyGoal",
            new Building(
                                "DummyGoal",
                                1,  // passable
                                1,  // Width
                                1,  // Height
                                "Buildings",// Parent
                                false // Links to neighbours and "sort of" becomes part of a large object
                            )
        );
        buildingPrototypes.Add("DummyBuilding",
           new Building(
                                "DummyBuilding",
                                0,  // IMpassable
                                1,  // Width
                                1,  // Height
                                "Buildings",// Parent
                                false // Links to neighbours and "sort of" becomes part of a large object
                            )
        );
    }

	

	#endregion

	#region Place Character&Building

	public Player PlacePlayer( Tile t) {
		//if (characterPrototypes.ContainsKey(playerType) == false) {
		//	Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + playerType);
		//	return null;
		//}

		// TODO: Load Player data and pass it here
		// This is hard-code Player data
		Dictionary<int, string> weapondict = new Dictionary<int, string>();
		weapondict.Add(0, "RedTearSword");
		Player dummyPlayer = new Player("Player", "MuMiMaN", weapondict, 10, 6, 8, 4, 4, 2, 500, 2, "PlayerRoot");
		Player p = Player.PlacePlayer(dummyPlayer, t);

		if (p == null) {
			// Failed to place Character -- most likely there was already something there.
			return null;
		}

		p.RegisterOnRemovedCallback(OnPlayerRemoved);

		return p;
	}

	public Enemy PlaceEnemy(string chrType, Tile t) {

		if (enemyPrototypes.ContainsKey(chrType) == false) {
			Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + chrType);
			return null;
		}

		Enemy e = Enemy.PlaceEnemy(enemyPrototypes[chrType], t);

		if (e == null) {
			// Failed to place Character -- most likely there was already something there.
			return null;
		}

		e.RegisterOnRemovedCallback(OnEnemyRemoved);
		// Don't Save Dummy building
		//if (bld.objectType != "DummyBuilding" && bld.objectType != "DummyGoal" )
		enemies.Add(e);

		return e;
	}

    public Building PlaceBuilding(string bldType, Tile t) {
        //Debug.Log("PlaceInstalledObject");
        // TODO: This function assumes 1x1 tiles -- change this later!

        if (buildingPrototypes.ContainsKey(bldType) == false) {
            Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + bldType);
            return null;
        }

        Building bld = Building.PlaceBuilding(buildingPrototypes[bldType], t);

        if (bld == null) {
            // Failed to place object -- most likely there was already something there.
            return null;
        }

		bld.RegisterOnRemovedCallback(OnBuildingRemoved);
		// Don't Save Dummy building
		//if (bld.objectType != "DummyBuilding" && bld.objectType != "DummyGoal" )
        buildings.Add(bld);

        // Set start and goal tile due to specific building
        if (bld.objectType == "OuterWall_Gate") { 
            startTile = GetTileAt(t.X, t.Z + 1);
            Debug.Log("Start Tile at" + startTile.X + "," + startTile.Z);
        }
        if (bld.objectType == "Goal") {
            goalTile = GetTileAt(t.X, t.Z);
            Debug.Log("Goal Tile at" + goalTile.X + "," + goalTile.Z);
			Debug.Log(goalTile.building.objectType);
        }

        if (cbBuildingCreated != null) {
            cbBuildingCreated(bld);

			if (bld.movementCost != 1) {
				// Since tiles return movement cost as their base cost multiplied
				// buy the furniture's movement cost, a furniture movement cost
				// of exactly 1 doesn't impact our pathfinding system, so we can
				// occasionally avoid invalidating pathfinding graphs
				InvalidateTileGraph();  // Reset the pathfinding system
			}
		}
        return bld;
    }

	#endregion

	public void OnPlayerRemoved(Player p) {
		player = null;
	}

	public void OnEnemyRemoved(Enemy e) {
		enemies.Remove(e);
	}

	public void OnBuildingRemoved(Building bld) {
		buildings.Remove(bld);
	}

	public void RegisterPlayerCreated(Action<Player> callbackfunc) {
		cbPlayerCreated += callbackfunc;
	}

	public void UnregisterPlayerCCreated(Action<Player> callbackfunc) {
		cbPlayerCreated -= callbackfunc;
	}

	public void RegisterEnemyCreated(Action<Enemy> callbackfunc) {
		cbEnemyCreated += callbackfunc;
	}

	public void UnregisterEnemyCreated(Action<Enemy> callbackfunc) {
		cbEnemyCreated -= callbackfunc;
	}

	public void RegisterBuildingCreated(Action<Building> callbackfunc) {
        cbBuildingCreated += callbackfunc;
    }

    public void UnregisterBuildingCreated(Action<Building> callbackfunc) {
        cbBuildingCreated -= callbackfunc;
    }

    public void RegisterTileChanged(Action<Tile> callbackfunc) {
        cbTileChanged += callbackfunc;
    }

    public void UnregisterTileChanged(Action<Tile> callbackfunc) {
        cbTileChanged -= callbackfunc;
    }

    // Gets called whenever ANY tile changes
    void OnTileChanged(Tile t) {
        if (cbTileChanged == null) {
            return;
        }
        cbTileChanged(t);
        InvalidateTileGraph();
    }

    // This should be called whenever a change to the world
    // means that our old pathfinding info is invalid.
    public void InvalidateTileGraph() {
        tileGraph = null;
    }

    public bool IsBuildingPlacementValid(string buildingType, Tile t) {
        return buildingPrototypes[buildingType].IsValidPosition(t);
    }

	public Weapon GetWeaponPrototype(string weaponType) {
		if (weaponPrototypes.ContainsKey(weaponType) == false) {
			Debug.LogError("No Weapon with type: " + weaponType);
			return null;
		}
		return weaponPrototypes[weaponType];
	}

	public Building GetBuildingPrototype(string buildingType) {
        if (buildingPrototypes.ContainsKey(buildingType) == false) {
            Debug.LogError("No Building with type: " + buildingType);
            return null;
        }
        return buildingPrototypes[buildingType];
    }

    //////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// 						SAVING & LOADING
    /// 
    //////////////////////////////////////////////////////////////////////////////////////

    public World() {

    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        // Save info here
        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Height.ToString());

        writer.WriteStartElement("Tiles");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Buildings");
        foreach (Building bld in buildings) {
            writer.WriteStartElement("Building");
            bld.WriteXml(writer);
            writer.WriteEndElement();

        }
        writer.WriteEndElement();

        //writer.WriteStartElement("Characters");
        //foreach (Character c in characters) {
        //    writer.WriteStartElement("Character");
        //    c.WriteXml(writer);
        //    writer.WriteEndElement();

        //}
        //writer.WriteEndElement();

        /*		writer.WriteStartElement("Width");
                writer.WriteValue(Width);
                writer.WriteEndElement();
        */


    }

    public void ReadXml(XmlReader reader) {
        Debug.Log("World::ReadXml");
        // Load info here

        Width = int.Parse(reader.GetAttribute("Width"));
        Height = int.Parse(reader.GetAttribute("Height"));

        SetupWorld(Width, Height);

        while (reader.Read()) {
            switch (reader.Name) {
                case "Tiles":
                    ReadXml_Tiles(reader);
                    break;
                case "Buildings":
                    ReadXml_Buildings(reader);
                    break;
                //case "Characters":
                //    ReadXml_Characters(reader);
                //    break;
            }
        }


    }

    void ReadXml_Tiles(XmlReader reader) {
        Debug.Log("ReadXml_Tiles");
        // We are in the "Tiles" element, so read elements until
        // we run out of "Tile" nodes.

        if (reader.ReadToDescendant("Tile")) {
            // We have at least one tile, so do something with it.

            do {
                int x = int.Parse(reader.GetAttribute("X"));
                int z = int.Parse(reader.GetAttribute("Z"));
                tiles[x, z].ReadXml(reader);
            } while (reader.ReadToNextSibling("Tile"));

        }

    }

    void ReadXml_Buildings(XmlReader reader) {
        Debug.Log("ReadXml_Buildings");

        if (reader.ReadToDescendant("Building")) {
            do {
                int x = int.Parse(reader.GetAttribute("X"));
                int z = int.Parse(reader.GetAttribute("Z"));

                Building bld = PlaceBuilding(reader.GetAttribute("objectType"), tiles[x, z]);
                //Debug.Log(bld.objectType);
                bld.ReadXml(reader);
            } while (reader.ReadToNextSibling("Building"));
        }

    }

    //void ReadXml_Characters(XmlReader reader) {
    //    Debug.Log("ReadXml_Characters");
    //    if (reader.ReadToDescendant("Character")) {
    //        do {
    //            int x = int.Parse(reader.GetAttribute("X"));
    //            int y = int.Parse(reader.GetAttribute("Y"));

    //            Character c = CreateCharacter(tiles[x, y]);
    //            c.ReadXml(reader);
    //        } while (reader.ReadToNextSibling("Character"));
    //    }
    //}
}

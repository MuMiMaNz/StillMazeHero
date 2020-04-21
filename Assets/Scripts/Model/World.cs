using UnityEngine;
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
	public List<Character> characters { get; protected set; }

	// The pathfinding graph used to navigate our world map.
	public Path_TileGraph tileGraph;
    public Tile startTile { get; protected set; }
    public Tile goalTile { get; protected set; }

    // Store building prototype data
    Dictionary<string, Building> buildingPrototypes;
	Dictionary<string, Character> characterPrototypes;

	Action<Building> cbBuildingCreated;
    Action<Character> cbCharacterCreated;
    Action<Tile> cbTileChanged;

    // The tile width of the world.
    public int Width { get; protected set; }
    // The tile height of the world
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
		CreateCharacterPrototypes();

		characters = new List<Character>();
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

	public Character CreatePlayerAtStart() {
		Debug.Log("CreateCharacter");
		Character c = PlaceCharacter("Player",startTile);

		characters.Add(c);

		if (cbCharacterCreated != null)
			cbCharacterCreated(c);

		return c;
	}

	public void Update(float deltaTime) {
		foreach (Character c in characters) {
			c.Update(deltaTime);
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

	// All Character prototypes data
	void CreateCharacterPrototypes() {
		characterPrototypes = new Dictionary<string, Character>();

		characterPrototypes.Add("Player",
			new Character("Player", // Name
							100f, // HP
							1, //Speed
							"Player" // Parent
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
        buildingPrototypes.Add("Base",
            new Building("Base", 0,  2, 2,"Buildings",false ));
    }

	public Character PlaceCharacter(string chrType, Tile t) {

		if (characterPrototypes.ContainsKey(chrType) == false) {
			Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + chrType);
			return null;
		}

		Character c = Character.PlaceCharacter(characterPrototypes[chrType], t);

		if (c == null) {
			// Failed to place Character -- most likely there was already something there.
			return null;
		}

		c.RegisterOnRemovedCallback(OnCharacterRemoved);
		// Don't Save Dummy building
		//if (bld.objectType != "DummyBuilding" && bld.objectType != "DummyGoal" )
		characters.Add(c);

		return c;
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
        }

        if (cbBuildingCreated != null) {
            cbBuildingCreated(bld);
            InvalidateTileGraph();
        }
        return bld;
    }

	public void OnCharacterRemoved(Character c) {
		characters.Remove(c);
	}

	public void OnBuildingRemoved(Building bld) {
		buildings.Remove(bld);
	}

	public void RegisterCharacterCreated(Action<Character> callbackfunc) {
		cbCharacterCreated += callbackfunc;
	}

	public void UnregisterCharacterCreated(Action<Character> callbackfunc) {
		cbCharacterCreated -= callbackfunc;
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

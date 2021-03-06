﻿using UnityEngine;
using System;
using System.Collections.Generic;

using System.Linq;
using Newtonsoft.Json;

public class World {

	// A two-dimensional array to hold our tile data.
	Tile[,] tiles;

	// Array use on Save/Load
	public List<Building> buildings { get; protected set; }
	public List<Minion> minions { get; protected set; }
	public Player player { get; protected set; }

	// The pathfinding graph used to navigate our world map.
	public Path_TileGraph tileGraph;
	public Tile startTile { get; protected set; }
	public Tile goalTile { get; protected set; }

	// Store building prototype data
	Dictionary<string, Building> buildingPrototypes;
	Dictionary<string, Minion> minionPrototypes;
	Dictionary<string, Player> playerPrototypes;
	Dictionary<string, Weapon> weaponPrototypes;

	Action<Building> cbBuildingCreated;
	Action<Player> cbPlayerCreated;
	Action<Minion> cbMinionCreated;
	Action<Tile> cbTileChanged;


	public int Width { get; protected set; }
	public int Height { get; protected set; }

	// Initializes a new instance of the World class.
	public World(int _width, int _height) {

		SetupWorld(_width, _height);
	}

	private void SetupWorld(int width, int height) {

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
		CreateMinionPrototypes();


		minions = new List<Minion>();
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

	public void CreatePlayerAtStart() {
		//Debug.Log("CreatePlayer");
		PlacePlayer(startTile);

		//Debug.Log("Player Name : " + player.name);
		//Debug.Log("Player's Weapon : " + player.weapons[0].wName);

	}
	// Update in Play Mode
	public void UpdateInPlayMode(float deltaTime) {
		foreach (Minion m in minions.ToList()) {
			m.Update(deltaTime);
		}
		player.Update(deltaTime);
	}
	public void FixedUpdateInPlayMode(float deltaTime) {
		foreach (Minion m in minions.ToList()) {
			m.FixedUpdate(deltaTime);
		}
	}
	// Update in Build Mode
	public void UpdateInBuildMode(float deltaTime) {

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



	//All Minion prototypes data
	void CreateMinionPrototypes() {
		minionPrototypes = new Dictionary<string, Minion>();

		minionPrototypes.Add("Yeti",
			new Minion(objectType: "Yeti",
						name: "Yeti",
						description: "Ancient winter mountain giant",
						STR: new Stat(name: "STR", val: 10, isPrimaryStat: true),
						INT: new Stat(name: "INT", val: 1),
						VIT: new Stat(name: "VIT", val: 10),
						DEX: new Stat(name: "DEX", val: 3),
						AGI: new Stat(name: "AGI", val: 2),
						LUK: new Stat(name: "LUK", val: 4),
						DEF: 10,
						mDEF: 5,
						combatType: CombatType.Melee,
						HP: 300,
						speed: 0.5f,
						spaceNeed: 1,
						patrolRange: 2,
						viewRadius: 2.5f,
						viewAngle: 60f,
						ATKRange: 0.8f,
						MeleeATKRange: 0.5f,
						chaseStraightRange: 1.3f,
						parent: "Minion"
							)
		);

		minionPrototypes.Add("Bat",
			new Minion(objectType: "Bat",
						name: "Bat",
						description: "Sleepy in day, do the OT at night",
						STR: new Stat(name: "STR", val: 3),
						INT: new Stat(name: "INT", val: 3),
						VIT: new Stat(name: "VIT", val: 4),
						DEX: new Stat(name: "DEX", val: 8),
						AGI: new Stat(name: "AGI", val: 12, isPrimaryStat: true),
						LUK: new Stat(name: "LUK", val: 5),
						DEF: 6,
						mDEF: 6,
						combatType: CombatType.Range,
						HP: 180,
						speed: 0.7f,
						spaceNeed: 1,
						patrolRange: 2,
						viewRadius: 3.0f,
						viewAngle: 75f,
						ATKRange: 1.4f,
						chaseStraightRange: 0f,
						parent: "Minion"
							)
		);
	}

	void CreateWeaponPrototypes() {
		weaponPrototypes = new Dictionary<string, Weapon>();

		weaponPrototypes.Add("RedTearSword",
			new Weapon(objectType: "RedTearSword",
						wName: "RedTearSword",
						wDescription: "You look at this Bad ass sword and cry in blood",
						wATK: 100, //ATK
						wMagicATK: 0, // mATK
						wATKspeed: 5, // ATKspeed
						wDEF: 0, // DEF
						wMagicDEF: 0, // mDEF
						weaponType: WeaponType.OneHandMelee,
						wSide: WeaponSide.Right,
						wSpace: 1 // Slot
							)
		);

		weaponPrototypes.Add("CrimsonWingShield",
			new Weapon(objectType: "CrimsonWingShield", // Object type
						wName: "CrimsonWingShield", // Name
						wDescription: "Original shield color is white, but time to times its cover with enemy blood", // Description
						wATK: 0, //ATK
						wMagicATK: 0, // mATK
						wATKspeed: 5, // ATKspeed
						wDEF: 10, // DEF
						wMagicDEF: 4, // mDEF
						weaponType: WeaponType.Shield,
						wSide: WeaponSide.Left,
						wSpace: 1 // Slot
							)
		);
	}

	// All Building prototypes data
	void CreateBuildingPrototypes() {
		buildingPrototypes = new Dictionary<string, Building>();

		buildingPrototypes.Add("InnerWall",
			new Building(
								objectType: "InnerWall",
								movementCost: 0,  // Impassable
								width: 1,  // Width
								height: 1,  // Height
								parent: "Buildings", // Parent
								allowMinion: false, // Allow minion on top of building
								linksToNeighbour: true // Links to neighbours and "sort of" becomes part of a large object
							)
		);
		buildingPrototypes.Add("OuterWall",
			new Building(
							   objectType: "OuterWall",
							   movementCost: 0,  // Impassable
							   width: 1,  // Width
							   height: 1,  // Height
							   parent: "OuterWalls",// Parent
							   allowMinion: false, // Allow minion on top of building
							   linksToNeighbour: true // Links to neighbours and "sort of" becomes part of a large object
							)
		);
		buildingPrototypes.Add("OuterWall_Gate",
		   new Building(
								objectType: "OuterWall_Gate",
								movementCost: 0,  // Impassable
								width: 3,  // Width
								height: 1,  // Height
								parent: "OuterWalls",// Parent
								allowMinion: false, // Allow minion on top of building
								linksToNeighbour: false // Links to neighbours and "sort of" becomes part of a large object
							)
		);
		buildingPrototypes.Add("Goal",
			new Building(
								objectType: "Goal",
								movementCost: 1,  // passable
								width: 1,  // Width
								height: 1,  // Height
								parent: "Buildings",// Parent
								allowMinion: false, // Allow minion on top of building
								linksToNeighbour: false // Links to neighbours and "sort of" becomes part of a large object
							)
		);
		buildingPrototypes.Add("DummyGoal",
			new Building(
								objectType: "DummyGoal",
								movementCost: 1,  // passable
								width: 1,  // Width
								height: 1,  // Height
								parent: "Buildings",// Parent
								allowMinion: false, // Allow minion on top of building
								linksToNeighbour: false // Links to neighbours and "sort of" becomes part of a large object
							)
		);
		buildingPrototypes.Add("DummyBuilding",
		   new Building(
								objectType: "DummyBuilding",
								movementCost: 0,  // IMpassable
								width: 1,  // Width
								height: 1,  // Height
								parent: "Buildings",// Parent
								allowMinion: false, // Allow minion on top of building
								linksToNeighbour: false // Links to neighbours and "sort of" becomes part of a large object
							)
		);
	}

	public Weapon GetWeaponPrototype(string weaponType) {
		if (weaponPrototypes.ContainsKey(weaponType) == false) {
			Debug.LogError("No Weapon with type: " + weaponType);
			return null;
		}
		return weaponPrototypes[weaponType];
	}

	public Minion GetMinionPrototype(string minionType) {
		if (minionPrototypes.ContainsKey(minionType) == false) {
			Debug.LogError("No Minion with type: " + minionType);
			return null;
		}
		return minionPrototypes[minionType];
	}

	public Building GetBuildingPrototype(string buildingType) {
		if (buildingPrototypes.ContainsKey(buildingType) == false) {
			Debug.LogError("No Building with type: " + buildingType);
			return null;
		}
		return buildingPrototypes[buildingType];
	}

	#endregion

	#region Place Character&Building

	public void PlacePlayer(Tile t) {
		//if (characterPrototypes.ContainsKey(playerType) == false) {
		//	Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + playerType);
		//	return null;
		//}

		// TODO: Load Player data and pass it here
		// This is hard-code Player data
		Dictionary<int, string> weapondict = new Dictionary<int, string>();
		weapondict.Add(0, "RedTearSword");
		weapondict.Add(1, "CrimsonWingShield");
		List<int> primWeaponSlot = new List<int>();
		primWeaponSlot.Add(0);
		primWeaponSlot.Add(1);

		Player dummyPlayer = new Player("Player", "MuMiMaN",
			weapondict, primWeaponSlot,
			STR: new Stat(name: "STR", val: 5),
			INT: new Stat(name: "INT", val: 3),
			VIT: new Stat(name: "VIT", val: 8),
			DEX: new Stat(name: "DEX", val: 10, isPrimaryStat: true),
			AGI: new Stat(name: "AGI", val: 6),
			LUK: new Stat(name: "LUK", val: 4),
			HP: 500, speed: 2, parent: "PlayerRoot"
			);

		Player p = Player.PlacePlayer(dummyPlayer, t);

		if (p == null) {
			// Failed to place Character -- most likely there was already something there.
			return;
		}

		p.RegisterOnRemovedCallback(OnPlayerRemoved);
		player = p;

		if (cbPlayerCreated != null)
			cbPlayerCreated(p);

	}

	public Minion PlaceMinion(string chrType, Tile t) {

		if (minionPrototypes.ContainsKey(chrType) == false) {
			Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + chrType);
			return null;
		}

		Minion m = Minion.PlaceMinion(minionPrototypes[chrType], t);

		if (m == null) {
			// Failed to place Minion -- most likely there was already something there.
			return null;
		}

		m.RegisterOnRemovedCallback(OnEnemyRemoved);
		minions.Add(m);

		if (cbMinionCreated != null)
			cbMinionCreated(m);

		return m;
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

	public void OnEnemyRemoved(Minion e) {
		minions.Remove(e);
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

	public void RegisterMinionCreated(Action<Minion> callbackfunc) {
		cbMinionCreated += callbackfunc;
	}

	public void UnregisterMinionCreated(Action<Minion> callbackfunc) {
		cbMinionCreated -= callbackfunc;
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



	//////////////////////////////////////////////////////////////////////////////////////
	/// 
	/// 						SAVING & LOADING
	/// 
	//////////////////////////////////////////////////////////////////////////////////////


	public string SaveJSON() {

		List<TileSaveObject> tsList = new List<TileSaveObject>();
		List<BuildingSaveObject> bsList = new List<BuildingSaveObject>();
		List<MinionSaveObject> msList = new List<MinionSaveObject>();

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				tsList.Add(tiles[x, y].SaveTile());
			}
		}
		foreach (Building bld in buildings) {
			bsList.Add(bld.SaveBuilding());
		}
		foreach (Minion m in minions) {
			msList.Add(m.SaveMinion());
		}

		WorldSaveObject worldSaveObject = new WorldSaveObject {
			Width = Width,
			Height = Height,
			tilesSaveObject = tsList,
			buildingsSaveObject = bsList,
			minionsSaveObject = msList
		};
		string json = JsonConvert.SerializeObject(worldSaveObject);
		Debug.Log(json);
		return json;

	}

	public void LoadWSO(WorldSaveObject wso) {

		foreach (TileSaveObject ts in wso.tilesSaveObject) {
			tiles[ts.X, ts.Z].LoadTile(ts);
		}

		foreach (BuildingSaveObject bs in wso.buildingsSaveObject) {
			Building b = PlaceBuilding(bs.objectType, tiles[bs.X, bs.Z]);
			b.LoadBuilding(bs);
		}

		foreach (MinionSaveObject ms in wso.minionsSaveObject) {
			Minion m = PlaceMinion(ms.objectType, tiles[ms.X, ms.Z]);
			m.LoadMinion(ms);
		}
	}
}

[Serializable]
public class WorldSaveObject {
	public int Width;
	public int Height;

	public List<TileSaveObject> tilesSaveObject;
	public List<BuildingSaveObject> buildingsSaveObject;
	public List<MinionSaveObject> minionsSaveObject;

}
	// Old XML File structure

	//   public World() {

	//   }


	//   public XmlSchema GetSchema() {
	//       return null;
	//   }

	//   public void WriteXml(XmlWriter writer) {
	//       // Save info here
	//       writer.WriteAttributeString("Width", Width.ToString());
	//       writer.WriteAttributeString("Height", Height.ToString());

	//       writer.WriteStartElement("Tiles");
	//       for (int x = 0; x < Width; x++) {
	//           for (int y = 0; y < Height; y++) {
	//               writer.WriteStartElement("Tile");
	//               tiles[x, y].WriteXml(writer);
	//               writer.WriteEndElement();
	//           }
	//       }
	//       writer.WriteEndElement();

	//       writer.WriteStartElement("Buildings");
	//       foreach (Building bld in buildings) {
	//           writer.WriteStartElement("Building");
	//           bld.WriteXml(writer);
	//           writer.WriteEndElement();

	//       }
	//       writer.WriteEndElement();

	//	writer.WriteStartElement("Minions");
	//	foreach (Minion m in minions) {
	//		writer.WriteStartElement("Minion");
	//		m.WriteXml(writer);
	//		writer.WriteEndElement();

	//	}
	//	writer.WriteEndElement();

	//	/*		writer.WriteStartElement("Width");
	//               writer.WriteValue(Width);
	//               writer.WriteEndElement();
	//       */


	//}

	//   public void ReadXml(XmlReader reader) {
	//       Debug.Log("World::ReadXml");
	//       // Load info here

	//       Width = int.Parse(reader.GetAttribute("Width"));
	//       Height = int.Parse(reader.GetAttribute("Height"));

	//       SetupWorld(Width, Height);

	//       while (reader.Read()) {
	//           switch (reader.Name) {
	//               case "Tiles":
	//                   ReadXml_Tiles(reader);
	//                   break;
	//               case "Buildings":
	//                   ReadXml_Buildings(reader);
	//                   break;
	//			case "Minions":
	//				ReadXml_Minions(reader);
	//				break;
	//		}
	//       }


	//   }

	//   void ReadXml_Tiles(XmlReader reader) {
	//       Debug.Log("ReadXml_Tiles");
	//       // We are in the "Tiles" element, so read elements until
	//       // we run out of "Tile" nodes.

	//       if (reader.ReadToDescendant("Tile")) {
	//           // We have at least one tile, so do something with it.

	//           do {
	//               int x = int.Parse(reader.GetAttribute("X"));
	//               int z = int.Parse(reader.GetAttribute("Z"));
	//               tiles[x, z].ReadXml(reader);
	//           } while (reader.ReadToNextSibling("Tile"));

	//       }

	//   }

	//   void ReadXml_Buildings(XmlReader reader) {
	//       Debug.Log("ReadXml_Buildings");

	//       if (reader.ReadToDescendant("Building")) {
	//           do {
	//               int x = int.Parse(reader.GetAttribute("X"));
	//               int z = int.Parse(reader.GetAttribute("Z"));

	//               Building bld = PlaceBuilding(reader.GetAttribute("objectType"), tiles[x, z]);
	//               //Debug.Log(bld.objectType);
	//               bld.ReadXml(reader);
	//           } while (reader.ReadToNextSibling("Building"));
	//       }

	//   }

	//void ReadXml_Minions(XmlReader reader) {
	//	Debug.Log("ReadXml_Minions");
	//	if (reader.ReadToDescendant("Minion")) {
	//		do {
	//			int x = int.Parse(reader.GetAttribute("X"));
	//			int y = int.Parse(reader.GetAttribute("Z"));

	//			Minion m = PlaceMinion(reader.GetAttribute("objectType"), tiles[x, y]);
	//			m.ReadXml(reader);
	//		} while (reader.ReadToNextSibling("Minion"));
	//	}
	//}


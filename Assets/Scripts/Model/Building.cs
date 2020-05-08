using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

// Building are things like walls, doors

public class Building : IXmlSerializable {

	//public World World { get; protected set; }

	// This represents the BASE tile of the object -- but in practice, large objects may actually occupy
	// multile tiles.
	public Tile tile {get; protected set;}

    // This "objectType" will be queried by the visual system to know what sprite to render for this object
    public string objectType { get; protected set;}

    // This is a multipler. So a value of "2" here, means you move twice as slowly (i.e. at half speed)
    // Tile types and other environmental effects may be combined.
    // For example, a "rough" tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3)
    // would have a total movement cost of (2+3+3 = 8), so you'd move through this tile at 1/8th normal speed.
    // SPECIAL: If movementCost = 0, then this tile is impassible. (e.g. a wall).
    public float movementCost { get; protected set; }

    // For example, a sofa might be 3x2 (actual graphics only appear to cover the 3x1 area, but the extra row is for leg room.)
    public int width { get; protected set; }
    public int height { get; protected set; }
    public int spaceNeed { get; protected set; }
    public string parent { get; protected set;}

	// Allow minion on top of building
	public bool allowMinion {
		get; protected set;
	}
	// GO change to same type neighbour
	public bool linksToNeighbour {
        get; protected set;
    }

	public Dictionary<string, float> bldParamaters;
	Action<Building> cbOnChanged;
	Action<Building> cbOnRemoved;

	public Action<Building, float> updateActions;

	

	Func<Tile, bool> funcPositionValidation;

	public void Update(float deltaTime) {
		if (updateActions != null) {
			updateActions(this, deltaTime);
		}
	}

	// Empty constructor is used for serialization
	public Building() {
        bldParamaters = new Dictionary<string, float>();
    }

    // Copy Constructor
    protected Building(Building other) {
        this.objectType = other.objectType;
        this.movementCost = other.movementCost;
        this.width = other.width;
        this.height = other.height;
        this.spaceNeed = other.spaceNeed;
        this.parent = other.parent;
		this.allowMinion = other.allowMinion;
        this.linksToNeighbour = other.linksToNeighbour;

        this.bldParamaters = new Dictionary<string, float>(other.bldParamaters);

        if (other.updateActions != null)
            this.updateActions = (Action<Building, float>)other.updateActions.Clone();
    }

    virtual public Building Clone() {
        return new Building(this);
    }

    // Create Building from parameters -- this will probably ONLY ever be used for prototypes
    public Building(string objectType, float movementCost = 1f, int width = 1, int height = 1, string parent = "Buildings",bool allowMinion = false, bool linksToNeighbour = false) {
        this.objectType = objectType;
        this.movementCost = movementCost;
        this.width = width;
        this.height = height;
        this.spaceNeed = width * height;
        this.parent = parent;
		this.allowMinion = allowMinion;

		this.linksToNeighbour = linksToNeighbour;

        this.funcPositionValidation = this.__IsValidPosition;

        bldParamaters = new Dictionary<string, float>();
    }

    static public Building PlaceBuilding(Building proto, Tile tile) {

        if (proto.funcPositionValidation(tile) == false) {
            Debug.LogError("Building.PlaceInstance -- Invalid Position");
            return null;
        }
        Building obj = proto.Clone();

        obj.tile = tile;

		if (tile == tile.World.startTile) {
			Debug.LogError("Can't place building at Start tile !! <('o ')");
			return null;
		}

		// FIXME: This assumes we are 1x1!
		if (tile.PlaceBuilding(obj) == false) {
            // For some reason, we weren't able to place our object in this tile.
            // (Probably it was already occupied.)

            // Do NOT return our newly instantiated object.
            // (It will be garbage collected.)
            return null;
        }

		//Debug.Log("Place Building:" + obj.objectType + "At" + obj.tile.X + "," + obj.tile.Z);

        if (obj.linksToNeighbour) {
            // This type of Building links itself to its neighbours,
            // so we should inform our neighbours that they have a new
            // buddy.  Just trigger their OnChangedCallback.

            Tile t;
            int x = tile.X;
            int z = tile.Z;

            t = tile.World.GetTileAt(x, z + 1);
            if (t != null && t.building != null && t.building.cbOnChanged != null  && t.building.objectType == obj.objectType) {
                // We have a Northern Neighbour with the same object type as us, so
                // tell it that it has changed by firing is callback.
                t.building.cbOnChanged(t.building);
            }
            t = tile.World.GetTileAt(x + 1, z);
            if (t != null && t.building != null && t.building.cbOnChanged != null && t.building.objectType == obj.objectType) {
                t.building.cbOnChanged(t.building);
            }
            t = tile.World.GetTileAt(x, z - 1);
            if (t != null && t.building != null && t.building.cbOnChanged != null && t.building.objectType == obj.objectType) {
                t.building.cbOnChanged(t.building);
            }
            t = tile.World.GetTileAt(x - 1, z);
            if (t != null && t.building != null && t.building.cbOnChanged != null  && t.building.objectType == obj.objectType) {
                t.building.cbOnChanged(t.building);
            }
        }

        return obj;
    }

	public void Deconstruct() {
		Debug.Log("Deconstruct Building");

		tile.RemoveBuilding();

		if (cbOnRemoved != null)
			cbOnRemoved(this);

		//	World.InvalidateTileGraph();
		

		// At this point, no DATA structures should be pointing to us, so we
		// should get garbage-collected.

	}

	public void RegisterOnChangedCallback(Action<Building> callbackFunc) {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Building> callbackFunc) {
        cbOnChanged -= callbackFunc;
    }

	public void RegisterOnRemovedCallback(Action<Building> callbackFunc) {
		cbOnRemoved += callbackFunc;
	}

	public void UnregisterOnRemovedCallback(Action<Building> callbackFunc) {
		cbOnRemoved -= callbackFunc;
	}

	public bool IsValidPosition(Tile t) {
        return funcPositionValidation(t);
    }

    // FIXME: These functions should never be called directly,
    // so they probably shouldn't be public functions of Building
    private bool __IsValidPosition(Tile t) {
        // Make sure tile is FLOOR
        if (t.Type == TileType.Empty) {
            return false;
        }

        // Make sure tile doesn't already have Building
        if (t.building != null) {
            return false;
        }

        return true;
    }

    //public bool __IsValidPosition_Door(Tile t) {
    //    if (__IsValidPosition(t) == false)
    //        return false;
    //    // Make sure we have a pair of E/W walls or N/S walls
    //    return true;
    //}

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("X", tile.X.ToString());
        writer.WriteAttributeString("Z", tile.Z.ToString());
        writer.WriteAttributeString("objectType", objectType);
        //writer.WriteAttributeString( "movementCost", movementCost.ToString() );

        foreach (string k in bldParamaters.Keys) {
            writer.WriteStartElement("Param");
            writer.WriteAttributeString("name", k);
            writer.WriteAttributeString("value", bldParamaters[k].ToString());
            writer.WriteEndElement();
        }

    }

    public void ReadXml(XmlReader reader) {
        // X, Y, and objectType have already been set, and we should already
        // be assigned to a tile.  So just read extra data.

        //movementCost = int.Parse( reader.GetAttribute("movementCost") );

        if (reader.ReadToDescendant("Param")) {
            do {
                string k = reader.GetAttribute("name");
                float v = float.Parse(reader.GetAttribute("value"));
                bldParamaters[k] = v;
            } while (reader.ReadToNextSibling("Param"));
        }
    }

}

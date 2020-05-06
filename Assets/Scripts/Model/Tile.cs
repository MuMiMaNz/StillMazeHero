using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


// TileType is the base type of the tile. In some tile-based games, that might be
// the terrain type. For us, we only need to differentiate between empty space
// and floor (a.k.a. the station structure/scaffold). Walls/Doors/etc... will be
// InstalledObjects sitting on top of the floor.
public enum TileType { Empty, Floor, OuterWall };

public class Tile :IXmlSerializable {

    private TileType _type = TileType.Empty;

    public TileType Type {
        get { return _type; }
        set {
            TileType oldType = _type;
            _type = value;
            // Call the callback and let things know we've changed.

            if (cbTileTypeChanged != null && oldType != _type)
                cbTileTypeChanged(this);
        }
    }

	// Character that stay in this tile ex. Enemy unit.
	// TODO: make a tile that
	public Character character {
		get; protected set;
	}

	// Building is something like a wall, door, or sofa.
	public Building building {
        get; protected set;
    }

	// Is this tile in a Pathfinding way
	public bool isPathway {
        get;  set;
    }

    public World World { get; protected set; }

    public int X { get;  set; }
    public int Z { get;  set; }

    public float movementCost {
        get {
            // Empty and Outer wall type are unwalkable
            if (Type == TileType.Empty || Type == TileType.OuterWall)
                return 0;   // 0 is unwalkable

            if (building == null)
                return 1;

            return 1 * building.movementCost;
        }
    }

    // The function we callback any time our type changes
    Action<Tile> cbTileTypeChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tile"/> class.
    /// </summary>
    /// <param name="world">A World instance.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="z">The y coordinate.</param>
    public Tile(World world, int x, int z) {
        this.World = world;
        this.X = x;
        this.Z = z;
    }

    // Register a function to be called back when our tile type changes.
    public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileTypeChanged += callback;
    }

    // Unregister a callback.
    public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileTypeChanged -= callback;
    }

	public bool PlaceCharacter(Character chrInstance) {
		
		if (character != null) {
			Debug.LogError("Trying to assign a Character to a tile that already has one!");
			return false;
		}

		character = chrInstance;

		return true;
	}

	public bool PlaceBuilding(Building bldInstance) {
        //if (objInstance == null) {
        //    // We are uninstalling whatever was here before.
        //    building = null;
        //    return true;
        //}
        // objInstance isn't null
        if (building != null) {
            Debug.LogError("Trying to assign a building to a tile that already has one!");
            return false;
        }
        // At this point, everything's fine!
        building = bldInstance;

        return true;
    }
	

	public bool RemoveBuilding() {
		//  uninstalling with multi-tile Building.

		if (building == null)
			return false;

		Building b = building;

		// Select Building at Pivot point (Left button) and remove for multi tile
		for (int x_off = X; x_off < (X + b.width); x_off++) {
			for (int z_off = Z; z_off < (Z + b.height); z_off++) {

				Tile t = World.GetTileAt(x_off, z_off);
				t.building = null;
			}
		}

		return true;
	}

	public bool RemoveCharacter() {
		if (character == null) {
			return false;
		}else {
			character = null;
			return true;
		}
	}

	// Tells us if two tiles are adjacent.
	public bool IsNeighbour(Tile tile ,bool diagOkay = false ) {
        // Check to see if we have a difference of exactly ONE between the two
        // tile coordinates.  Is so, then we are vertical or horizontal neighbours.
        return
            Mathf.Abs(this.X - tile.X) + Mathf.Abs(this.Z - tile.Z) == 1   // Check hori/vert adjacency
             || (diagOkay && (Mathf.Abs(this.X - tile.X) == 1 && Mathf.Abs(this.Z - tile.Z) == 1)) // Check diag adjacency
            ;
    }

    /// <summary>
    /// Gets the neighbours.
    /// </summary>
    /// <returns>The neighbours.</returns>
    /// <param name="diagOkay">Is diagonal movement okay?.</param>
    public Tile[] GetNeighbours(bool diagOkay = false) {
        Tile[] ns;

        if (diagOkay == false) {
            ns = new Tile[4];   // Tile order: N E S W
    }
        else {
            ns = new Tile[8];   // Tile order : N E S W NE SE SW NW
        }

        Tile n;

        n = World.GetTileAt(X, Z + 1);
        ns[0] = n;  // Could be null, but that's okay.
        n = World.GetTileAt(X + 1, Z);
        ns[1] = n;  // Could be null, but that's okay.
        n = World.GetTileAt(X, Z - 1);
        ns[2] = n;  // Could be null, but that's okay.
        n = World.GetTileAt(X - 1, Z);
        ns[3] = n;  // Could be null, but that's okay.

        if (diagOkay == true) {
            n = World.GetTileAt(X + 1, Z + 1);
            ns[4] = n;  // Could be null, but that's okay.
            n = World.GetTileAt(X + 1, Z - 1);
            ns[5] = n;  // Could be null, but that's okay.
            n = World.GetTileAt(X - 1, Z - 1);
            ns[6] = n;  // Could be null, but that's okay.
            n = World.GetTileAt(X - 1, Z + 1);
            ns[7] = n;  // Could be null, but that's okay.
        }

        return ns;
    }

    	public XmlSchema GetSchema() {
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		writer.WriteAttributeString( "X", X.ToString() );
		writer.WriteAttributeString( "Z", Z.ToString() );
		writer.WriteAttributeString("Type", ((int)Type).ToString() );
	}

	public void ReadXml(XmlReader reader) {
		Type = (TileType)int.Parse( reader.GetAttribute("Type") );
	}

}

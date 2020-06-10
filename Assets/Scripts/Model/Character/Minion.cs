using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public enum MinionState { Chase, ChaseToPatrol ,Patrol,  Idle }

public class Minion : Character{

	World World {get { return WorldController.Instance.World; }}

	Action<Minion> cbMinionChanged;
	//Action<Minion> cbMinionCoroutine;
	Action<Minion> cbOnRemoved;

	public Dictionary<string, float> bldParamaters;

	public string name { get; protected set; }
	public string description { get; protected set; }
	public int spaceNeed { get; protected set; }

	public MinionState minionState {get; protected set;}
	// Maximum tile that minion will patrol
	public int patrolRange { get; protected set; }
	// Set of patrol point tiles
	public List<Tile> patrolPoints { get; protected set; }
	// FOV
	public bool seePlayer { get;  set; }
	public bool playerInATKRange { get;  set; }
	public float viewRadius { get; protected set; }
	public float viewAngle { get; protected set; }
	public float ATKRange { get; protected set; }

	public bool canTakeDMG { get; protected set; }
	private float involuntaryTime = 0.2f;

	public float X {
		get {
			if (nextTile == null)
				return currTile.X;

			return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
		}
	}
	public float Z {
		get {
			if (nextTile == null)
				return currTile.Z;

			return Mathf.Lerp(currTile.Z, nextTile.Z, movementPercentage);
		}
	}
	public Vector3 directionVector {
		get {
			Vector3 drt = new Vector3(nextTile.X, 0, nextTile.Z) - new Vector3(currTile.X, 0, currTile.Z);
			return drt.normalized;
		}
	}
	private float idleWaitTime = 2f;

	private Tile _currTile;
	public Tile currTile {
		get { return _currTile; }

		protected set {
			//if(_currTile != null) {
			//	_currTile.characters.Remove(this);
			//}
			_currTile = value;
			//_currTile.characters.Add(this);
		}
	}
	// If we aren't moving, then destTile = currTile
	private Tile _destTile;
	public Tile DestTile {
		get { return _destTile; }
		set {
			if(_destTile != value) {
				_destTile = value;
				mPathAStar = null;	// If this is a new destination, then we need to invalidate pathfinding.
			}
		}
	}

	private Tile nextTile;  // The next tile in the pathfinding sequence
	private Path_AStar mPathAStar;
	private Path_TileGraph mTileGraph;
	private float movementPercentage; // Goes from 0 to 1 as we move from currTile to destTile
	
	// Empty constructor is used for serialization
	public Minion() {
		bldParamaters = new Dictionary<string, float>();
	}

	// Use for create prototype
	public Minion(string objectType, string name, string description,
		int STR = 1, int INT = 1, int VIT = 1, int DEX = 1, int AGI = 1, int LUK = 1,
		float HP = 100f, float speed = 1,int spaceNeed=1 ,int patrolRange = 2, float viewRadius = 1.5f ,float viewAngle = 45f,float ATKRange = 0.5f, string parent = "Character") {

		this.objectType = objectType;
		this.name = name;
		this.description = description;
		
		this.HP = HP;
		this.speed = speed;
		this.STR = STR;
		this.INT = INT;
		this.VIT = VIT;
		this.DEX = DEX;
		this.AGI = AGI;
		this.LUK = LUK;

		this.spaceNeed = spaceNeed;
		this.patrolRange = patrolRange;
		this.viewRadius = viewRadius;
		this.viewAngle = viewAngle;
		this.ATKRange = ATKRange;

		this.parent = parent;

		this.seePlayer = false;
		this.playerInATKRange = false;

		bldParamaters = new Dictionary<string, float>();
	}

	static public Minion PlaceMinion(Minion proto, Tile t) {
		//Debug.Log("Minion.PlaceMinion()");
		Minion m = new Minion(proto.objectType, proto.name,proto.description,
			proto.STR, proto.INT, proto.VIT, proto.DEX, proto.AGI, proto.LUK,
			proto.HP, proto.speed,proto.spaceNeed,
			proto.patrolRange, proto.viewRadius, proto.viewAngle,proto.ATKRange, proto.parent);

		m.charStartTile = t;
		m.currTile = t;
		m.nextTile = t;
		m.DestTile = t;

		// If it start tile , cannot place minion
		if (t == t.World.startTile) {
			Debug.LogError("Can't place minion at Start tile !! <('o ')");
			return null;
		}

		if (t.PlaceCharacter(m) == false) {
			// For some reason, we weren't able to place our object in this tile.
			// (Probably it was already occupied.)

			// Do NOT return our newly instantiated object.
			// (It will be garbage collected.)
			return null;
		}

		// TODO : Check if building allow minion on top
		//if (t.PlaceCharacter(e) == false || ) {
		//	return null;
		//}

		return m;
	}

	// Set Patrol Point in 4 Quadrant
	public void SetValidPatrolPoints(World world) {

		minionState = MinionState.Patrol;
		patrolPoints = new List<Tile>();

		if (patrolRange == 0)
			return ;

		// Set tile graph width and height
		int startW = charStartTile.X - patrolRange <=0 ? 0 : charStartTile.X - patrolRange;
		int endW = charStartTile.X + patrolRange >= world.Width-1 ? world.Width - 1 : charStartTile.X + patrolRange;
		//Debug.Log("StartWidth : " + startW + "EndWidth : " + endW);
		int startH = charStartTile.Z - patrolRange <= 0 ? 0 : charStartTile.Z - patrolRange;
		int endH = charStartTile.Z + patrolRange >= world.Height - 1 ? world.Height - 1 : charStartTile.Z + patrolRange;
		//Debug.Log("StartHeight : " + startH + "EndHeight  : " + endH);

		// Create Tile Graph within Patrol range
		mTileGraph = new Path_TileGraph(startW, endW, startH, endH);

		//// Check 4 Quadrant have at least 1 patrol tile?
		Tile LUtile = null;
		Tile RUtile = null;
		Tile LLtile = null;
		Tile RLtile = null;

		// ======Loop Outer in 4 Quadrant============

		// Left-Lower Q ,X(min > max), Z(min > max)
		// Loop the X axis farest tile
		for (int x = startW; x < charStartTile.X; x++) {
			int z = startH;
			Tile checkT = world.GetTileAt(x, z);
			if (LLtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add LLtile to patrol point : " + checkT.X + "," + checkT.Z);
					LLtile = checkT;
				}
			}
		}
		// Loop the Z axis farest tile
		for (int z = startH; z < charStartTile.Z; z++) {
			int x = startW;
			Tile checkT = world.GetTileAt(x, z);
			//Debug.Log("Check tile : " + checkT.X + "," + checkT.Z);

			// Check the farest tile first and Check it not CharTile
			if (LLtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add LLtile to patrol point : " + checkT.X + "," + checkT.Z);
					LLtile = checkT;
				}
			}
		}


		// Left-Upper Q ,X(min > max),Z(max > min)
		// Loop the X axis farest tile
		for (int x = startW; x <= charStartTile.X; x++) {
			int z = endH;
			Tile checkT = world.GetTileAt(x, z);
			if (LUtile == null  && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add LUtile to patrol point : " + checkT.X + "," + checkT.Z);
					LUtile = checkT;
				}
			}
		}
		// Loop the Z axis farest tile
		for (int z = endH ; z >= charStartTile.Z ; z--) {
			int x = startW;
			Tile checkT = world.GetTileAt(x, z);

			// Check the farest tile first and Check it not CharTile
			if (LUtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add LUtile to patrol point : " + checkT.X + "," + checkT.Z);
					LUtile = checkT;
				}
			}
		}


		// Right-Upper Q ,X(max > min),Z(max > min)
		// Loop the X axis farest tile
		for (int x = endW; x >= charStartTile.X; x--) {
			int z = endH;
			Tile checkT = world.GetTileAt(x, z);

			// Check the farest tile first and Check it not CharTile
			if (RUtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add RUtile to patrol point : " + checkT.X + "," + checkT.Z);
					RUtile = checkT;
				}
			}
		}
		// Loop the Z axis farest tile
		for (int z = endH; z >= charStartTile.Z; z--) {
			int x = endW;
			Tile checkT = world.GetTileAt(x, z);

			// Check the farest tile first and Check it not CharTile
			if (RUtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add RUtile to patrol point : " + checkT.X + "," + checkT.Z);
					RUtile = checkT;
				}
			}
		}


		// Right-Lower Q ,X(max > min),Z(min > max)
		// Loop the X axis farest tile
		for (int x = endW; x >= charStartTile.X; x--) {
			int z = startH;
			Tile checkT = world.GetTileAt(x, z);

			// Check the farest tile first and Check it not CharTile
			if (RLtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add RLtile to patrol point : " + checkT.X + "," + checkT.Z);
					RLtile = checkT;
				}
			}
		}
		// Loop the Z axis farest tile
		for (int z = startH; z > charStartTile.Z; z++) {
			int x = startW;
			Tile checkT = world.GetTileAt(x, z);

			// Check the farest tile first and Check it not CharTile
			if (RLtile == null && checkT != charStartTile) {
				mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
				// There is valid pathfinder to this tile
				if (mPathAStar.Length() != 0) {
					Debug.Log("Add RLtile to patrol point : " + checkT.X + "," + checkT.Z);
					RLtile = checkT;
				}
			}
		}

		// ======== Loop Inner in 4 Quadrant ==========

		// Left-Lower Q ,X(min > max), Z(min > max)
		// Loop the inner tile
		if (LLtile == null) {
			for (int x = startW + 1; x < charStartTile.X; x++) {
				for (int z = startH + 1; z < charStartTile.Z; z++) {
					Tile checkT = world.GetTileAt(x, z);
					if (LLtile == null && checkT != charStartTile) {
						mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
						// There is valid pathfinder to this tile
						if (mPathAStar.Length() != 0) {
							Debug.Log("Add LLtile to patrol point : " + checkT.X + "," + checkT.Z);
							LLtile = checkT;
						}
					}
				}
			}
		}


		// Left-Upper Q ,X(min > max),Z(max > min)
		// Loop the X axis farest tile
		if (LUtile == null) {
			for (int x = startW + 1; x <= charStartTile.X; x++) {
				for (int z = endH - 1; z >= charStartTile.Z; z--) {
					Tile checkT = world.GetTileAt(x, z);
					if (LUtile == null && checkT != charStartTile) {
						mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
						// There is valid pathfinder to this tile
						if (mPathAStar.Length() != 0) {
							Debug.Log("Add LUtile to patrol point : " + checkT.X + "," + checkT.Z);
							LUtile = checkT;
						}
					}
				}
			}
		}

		// Right-Upper Q ,X(max > min),Z(max > min)
		// Loop the X axis farest tile
		if (RUtile == null) {
			for (int x = endW - 1; x >= charStartTile.X; x--) {
				for (int z = endH - 1; z >= charStartTile.Z; z--) {
					Tile checkT = world.GetTileAt(x, z);

					// Check the farest tile first and Check it not CharTile
					if (RUtile == null && checkT != charStartTile) {
						mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
						// There is valid pathfinder to this tile
						if (mPathAStar.Length() != 0) {
							Debug.Log("Add RUtile to patrol point : " + checkT.X + "," + checkT.Z);
							RUtile = checkT;
						}
					}
				}
			}
		}


		// Right-Lower Q ,X(max > min),Z(min > max)
		// Loop the X axis farest tile
		if (RLtile == null) {
			for (int x = endW -1; x >= charStartTile.X; x--) {
				for (int z = startH + 1; z < charStartTile.Z; z++) {
					Tile checkT = world.GetTileAt(x, z);

					// Check the farest tile first and Check it not CharTile
					if (RLtile == null && checkT != charStartTile) {
						mPathAStar = new Path_AStar(false, charStartTile, checkT, mTileGraph, startW, endW, startH, endH);
						// There is valid pathfinder to this tile
						if (mPathAStar.Length() != 0) {
							Debug.Log("Add RLtile to patrol point : " + checkT.X + "," + checkT.Z);
							RLtile = checkT;
						}
					}
				}
			}
		}


		if (LUtile != null) patrolPoints.Add(LUtile);
		if (RUtile != null) patrolPoints.Add(RUtile);
		if (LLtile != null) patrolPoints.Add(LLtile);
		if (RLtile != null) patrolPoints.Add(RLtile);

	}

	private void PatrolMovement(float deltaTime) {

		// Check if current tile in patrol points then remove it
		List<Tile> newPatrolPoints = new List<Tile>(patrolPoints);
		if (patrolPoints.Contains(currTile)) {
			newPatrolPoints.Remove(currTile);
		}
		// Minion reach patrol destination tile
		if (currTile == DestTile) {
			// Set Idle state for 2 seconds
			minionState = MinionState.Idle;
			if (!WaitedInSeconds(deltaTime,idleWaitTime)) return;

			// then set new Destination
			minionState = MinionState.Patrol;
			DestTile = newPatrolPoints[UnityEngine.Random.Range(0, newPatrolPoints.Count)];
			//Debug.Log("Patrol to :" + DestTile.X + "," + DestTile.Z);
		}
	}

	private float timer = 0;
	private float timerMax = 0;
	private bool WaitedInSeconds(float deltaTime,float seconds) {
		timerMax = seconds;
		timer += deltaTime;

		if (timer >= timerMax) {
			timer = 0;
			return true; //max reached - waited x - seconds
		}
		return false;
	}

	void DoMovement(float deltaTime,bool useWorldTG, Path_TileGraph tg = null) {
		// We're already were we want to be.
		if (currTile == DestTile) {
			mPathAStar = null;
			return;	
		}
		// currTile = The tile I am currently in (and may be in the process of leaving)
		// nextTile = The tile I am currently entering
		// destTile = Our final destination -- we never walk here directly, but instead use it for the pathfinding

		if(nextTile == null || nextTile == currTile) {
			// Get the next tile from the pathfinder.
			if(mPathAStar == null || mPathAStar.Length() == 0) {
				// Generate a path to our destination
				if (useWorldTG) {
					mPathAStar = new Path_AStar(true, currTile, DestTile);
				}else {
					mPathAStar = new Path_AStar(false, currTile, DestTile, tg);
				}
			
				if(mPathAStar.Length() == 0) {
					Debug.LogError("Path_AStar returned no path to destination!");
					return;
				}
				// Dump the first tile, because nextTile == currTile
				nextTile = mPathAStar.Dequeue();
			}

			// Grab the next waypoint from the pathing system!
			nextTile = mPathAStar.Dequeue();

			if ( nextTile == currTile ) {
				Debug.LogError("Update_DoMovement - nextTile is currTile?");
			}
		}

/*		if(pathAStar.Length() == 1) {
			return;
		}
*/
		// At this point we should have a valid nextTile to move to.

		// What's the total distance from point A to point B?
		// We are going to use Euclidean distance FOR NOW...
		// But when we do the pathfinding system, we'll likely
		// switch to something like Manhattan or Chebyshev distance
		float distToTravel = Mathf.Sqrt(
			Mathf.Pow(currTile.X-nextTile.X, 2) + 
			Mathf.Pow(currTile.Z-nextTile.Z, 2)
		);

		/*if(nextTile.IsEnterable() == ENTERABILITY.Never) {
			// Most likely a wall got built, so we just need to reset our pathfinding information.
			// FIXME: Ideally, when a wall gets spawned, we should invalidate our path immediately,
			//		  so that we don't waste a bunch of time walking towards a dead end.
			//		  To save CPU, maybe we can only check every so often?
			//		  Or maybe we should register a callback to the OnTileChanged event?
			Debug.LogError("FIXME: A character was trying to enter an unwalkable tile.");
			nextTile = null;	// our next tile is a no-go
			pathAStar = null;	// clearly our pathfinding info is out of date.
			return;
		}
		else if ( nextTile.IsEnterable() == ENTERABILITY.Soon ) {
			// We can't enter the NOW, but we should be able to in the
			// future. This is likely a DOOR.
			// So we DON'T bail on our movement/path, but we do return
			// now and don't actually process the movement.
			return;
		} */

		// How much distance can be travel this Update?
		float distThisFrame = speed / nextTile.movementCost * deltaTime;

		// How much is that in terms of percentage to our destination?
		float percThisFrame = distThisFrame / distToTravel;

		// Add that to overall percentage travelled.
		movementPercentage += percThisFrame;

		if(movementPercentage >= 1) {
			// We have reached our destination

			// TODO: Get the next tile from the pathfinding system.
			//       If there are no more tiles, then we have TRULY
			//       reached our destination.

			currTile = nextTile;
			movementPercentage = 0;
			// FIXME?  Do we actually want to retain any overshot movement?
		}
	}

	public void Update(float deltaTime) {
		// If see Player , Chase him ! do A*pathfinding in all World tile
		if (seePlayer) {
			minionState = MinionState.Chase;
			
			DestTile = WorldController.Instance.World.GetTileAt(
				Mathf.RoundToInt(World.player.X),
				Mathf.RoundToInt(World.player.Z));

			
			DoMovement(deltaTime,true); 
		}
		// If not see Player
		else {
			// Patrol Mode do A*pathfinding in just Patrol Range tile
			if (patrolPoints.Count > 0 && (minionState == MinionState.Patrol || minionState == MinionState.Idle)) {

				PatrolMovement(deltaTime);

				DoMovement(deltaTime,false, mTileGraph);
			}
			// If previosly chasing player and then don't see Player, come back to patrol
			if (minionState == MinionState.Chase) {
				// Wait for 2 seconds if really not see player and comeback to patrol
				Debug.Log("Wait");
				WaitedInSeconds(deltaTime,2);
				minionState = MinionState.ChaseToPatrol;

			}
			if(minionState == MinionState.ChaseToPatrol) {
				DestTile = charStartTile;
				//Debug.Log(DestTile.X + "," + DestTile.Z);
				DoMovement(deltaTime, true);

				if (currTile == DestTile) {
					minionState = MinionState.Patrol;
				}
			}
		}

		
	}

	public void FixedUpdate(float deltaTime) {
		if (cbMinionChanged != null)
			cbMinionChanged(this);
	}

	public void RemoveMinion() {
		Debug.Log("Remove Minion");

		charStartTile.RemoveCharacter();

		if (cbOnRemoved != null)
			cbOnRemoved(this);

		// At this point, no DATA structures should be pointing to us, so we
		// should get garbage-collected.

	}

	public void RegisterOnChangedCallback(Action<Minion> cb) {
		cbMinionChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Minion> cb) {
		cbMinionChanged -= cb;
	}

	//public void RegisterCouroutineCallback(Action<Minion> cb) {
	//	cbMinionCoroutine += cb;
	//}

	//public void UnregisterCouroutineCallback(Action<Minion> cb) {
	//	cbMinionCoroutine -= cb;
	//}

	public void RegisterOnRemovedCallback(Action<Minion> callbackFunc) {
		cbOnRemoved += callbackFunc;
	}

	public void UnregisterOnRemovedCallback(Action<Minion> callbackFunc) {
		cbOnRemoved -= callbackFunc;
	}

	// ----- Save Data --------

	public XmlSchema GetSchema() {
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		writer.WriteAttributeString("X", charStartTile.X.ToString());
		writer.WriteAttributeString("Z", charStartTile.Z.ToString());
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

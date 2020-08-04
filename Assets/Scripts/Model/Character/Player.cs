using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

	World World {
		get { return WorldController.Instance.World; }
	}

	Action<Player> cbPlayerChanged;
	Action<Player> cbOnRemoved;

	public string name { get; protected set; }
	//public List<Weapon> weapons { get; protected set; }
	// Use Dictionary to Write/Load data
	protected Dictionary<int, string> weaponsDict;
	public List<Weapon> weapons { get; protected set; }
	public List<Weapon> primaryWeapons { get; protected set; }
	protected List<int> primWeaponSlot;

	public float X { get;  set; }
	public float Z { get;  set; }

	// Empty constructor is used for serialization
	public Player() {
		//weapons = new List<Weapon>();
		weapons = new List<Weapon>();
		primaryWeapons = new List<Weapon>();
		primWeaponSlot = new List<int>();
		weaponsDict = new Dictionary<int, string>();
	}

	// Copy Constructor -- don't call this directly, unless we never
	// do ANY sub-classing. Instead use Clone(), which is more virtual.
	protected Player(Player other) {
		//Debug.Log("Protected Player function");
		this.name = other.name;
		this.objectType = other.objectType;
		this.MaxHP = other.HP;
		this.HP = other.HP;
		this.speed = other.speed;

		this.STR = other.STR;
		this.INT = other.INT;
		this.VIT = other.VIT;
		this.DEX = other.DEX;
		this.AGI = other.AGI;
		this.LUK = other.LUK;

		this.ATKIntervalTime = other.ATKIntervalTime;
		this.involuntaryTime = 0.1f;

		this.parent = other.parent;

		canTakeDMG = true;

		weapons = new List<Weapon>();
		primaryWeapons = new List<Weapon>();
		primWeaponSlot = other.primWeaponSlot;
		//Debug.Log(this.primWeaponSlot.Count);

		// Load weapon to Weapon iventory slot 
		if (other.weaponsDict != null) {
			this.weaponsDict = new Dictionary<int, string>(other.weaponsDict);

			foreach (KeyValuePair<int ,string> kv in weaponsDict) {
				Weapon w = World.GetWeaponPrototype(kv.Value);
				weapons.Add(w);
	
				// Set primary weapon
				foreach (int ws in primWeaponSlot) {
					//Debug.Log("Primaray Slot :" + ws);
					if(ws == kv.Key) {
						primaryWeapons.Add(w);
						//Debug.Log("Primary Weapon :" + w.wName);
					}
				}
			}
		}
		else {
			this.weaponsDict = new Dictionary<int, string>();
		}
		//this.bldParamaters = new Dictionary<string, float>(other.bldParamaters);

		//if (other.updateActions != null)
		//	this.updateActions = (Action<Building, float>)other.updateActions.Clone();
	}

	// Make a copy of the current furniture.  Sub-classed should
	// override this Clone() if a different (sub-classed) copy
	// constructor should be run.
	virtual public Player Clone() {
		return new Player(this);
	}

	// Create Player from parameters - used for prototypes
	public Player(string objectType, string name, 
		Dictionary<int, string> weaponsDict, List<int> primWeaponSlot,
		Stat STR , Stat INT , Stat VIT,
		Stat DEX , Stat AGI , Stat LUK , 
		int HP = 100, float speed = 1, string parent = "Character",
		float ATKIntervalTime = 0.6f
		) {

		this.name = name;
		this.objectType = objectType;
		this.MaxHP = HP;
		this.HP = HP;
		this.speed = speed;

		this.STR = STR;
		this.INT = INT;
		this.VIT = VIT;
		this.DEX = DEX;
		this.AGI = AGI;
		this.LUK = LUK;

		this.ATKIntervalTime = ATKIntervalTime;

		this.parent = parent;

		this.weaponsDict = weaponsDict;
		this.primWeaponSlot = primWeaponSlot;
		//Debug.Log(this.primWeaponSlot.Count);
		//foreach(string w in weaponsDict.Values) {
		//	weapons.Add(World.GetWeaponPrototype(w));
		//}
	}
	

	// TODO : Set primary waepon by player click select weapon
	public void SetPrimaryWeapon() {
		
	}

	static public Player PlacePlayer(Player proto, Tile t) {

		Player p = proto.Clone();

		p.charStartTile = t;
		p.X = t.X;
		p.Z = t.Z;

		if (t.PlacePlayer(p) == false) {
			return null;
		}

		// Not check is Place tile have Character for make sure that Player can start
		// BEWARE : Must not have Character/Building on start tile
		//if (tile.PlaceBuilding(obj) == false) {
		//	return null;
		//}

		return p;
	}

	public float TakeDamage(Minion m) {

		if (canTakeDMG == false)
			return 0f;

		Stat primStat = m.GetPrimaryStat();

		float AtkDMG = ((primStat.BaseValue + primStat.BuffValue) * UnityEngine.Random.Range(0f, 2.0f)) ;

		int finalDMG = Mathf.RoundToInt(AtkDMG - (DEF * 2.5f));
		HP -= finalDMG;

		//Debug.Log("Player HP :" + HP);

		canTakeDMG = false;
		return finalDMG;
	}

	private float takeDMGTimeCounter = 0f;

	public void Update(float deltaTime) {
		if (canTakeDMG == false) {
			Debug.Log(takeDMGTimeCounter);
			takeDMGTimeCounter += deltaTime;
			if (takeDMGTimeCounter >= involuntaryTime) {
				canTakeDMG = true;
				takeDMGTimeCounter = 0f;
			}
		}
	}

	public void RegisterOnChangedCallback(Action<Player> cb) {
		cbPlayerChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Player> cb) {
		cbPlayerChanged -= cb;
	}

	public void RegisterOnRemovedCallback(Action<Player> callbackFunc) {
		cbOnRemoved += callbackFunc;
	}

	public void UnregisterOnRemovedCallback(Action<Player> callbackFunc) {
		cbOnRemoved -= callbackFunc;
	}

}

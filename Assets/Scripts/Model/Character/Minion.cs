using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Character{

	Action<Minion> cbMinionChanged;
	Action<Minion> cbOnRemoved;

	public string name { get; protected set; }
	public string description { get; protected set; }
	public int spaceNeed { get; protected set; }

	public Minion(string objectType, string name, string description, int STR = 1, int INT = 1, int VIT = 1, int DEX = 1, int AGI = 1, int LUK = 1, float HP = 100f, float speed = 1,int spaceNeed=1 , string parent = "Character") {

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

		//this.X = charStartTile.X;
		//this.Z = charStartTile.Z;
		this.spaceNeed = spaceNeed;
		this.parent = parent;

		//bldParamaters = new Dictionary<string, float>();
	}

	static public Minion PlaceMinion(Minion proto, Tile t) {

		Minion e = new Minion(proto.objectType, proto.name,proto.description,
			proto.STR, proto.INT, proto.VIT, proto.DEX, proto.AGI, proto.LUK,
			proto.HP, proto.speed,proto.spaceNeed, proto.parent);

		e.charStartTile = t;
		e.X = t.X;
		e.Z = t.Z;

		// If it start tile , cannot place minion
		if(t == WorldController.Instance.World.startTile) {
			Debug.LogError("Can't place minion at Start tile !! <('o ')");
			return null;
		}

		// TODO : Check if building allow minion
		//if (t.PlaceCharacter(e) == false || ) {
		//	return null;
		//}

		return e;
	}

	public void Update(float deltaTime) {
		//Debug.Log("Character Update");

		if (cbMinionChanged != null)
			cbMinionChanged(this);
	}

	public void RemoveMinion() {
		Debug.Log("Remove Minion");

		charStartTile.RemoveCharacter();

		if (cbOnRemoved != null)
			cbOnRemoved(this);

		//World.InvalidateTileGraph();

		// At this point, no DATA structures should be pointing to us, so we
		// should get garbage-collected.

	}

	public void RegisterOnChangedCallback(Action<Minion> cb) {
		cbMinionChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Minion> cb) {
		cbMinionChanged -= cb;
	}

	public void RegisterOnRemovedCallback(Action<Minion> callbackFunc) {
		cbOnRemoved += callbackFunc;
	}

	public void UnregisterOnRemovedCallback(Action<Minion> callbackFunc) {
		cbOnRemoved -= callbackFunc;
	}

}

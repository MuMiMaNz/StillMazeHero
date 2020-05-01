using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character{

	Action<Enemy> cbEnemyChanged;
	Action<Enemy> cbOnRemoved;

	public string name { get; protected set; }

	public Enemy(string objectType, string name, int STR = 1, int INT = 1, int VIT = 1, int DEX = 1, int AGI = 1, int LUK = 1, float HP = 100f, float speed = 1, string parent = "Character") {

		this.name = name;
		this.objectType = objectType;
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
		this.parent = parent;

		//bldParamaters = new Dictionary<string, float>();
	}

	static public Enemy PlaceEnemy(Enemy proto, Tile t) {

		Enemy e = new Enemy(proto.objectType, proto.name,
			proto.STR, proto.INT, proto.VIT, proto.DEX, proto.AGI, proto.LUK,
			proto.HP, proto.speed, proto.parent);

		e.charStartTile = t;
		e.X = t.X;
		e.Z = t.Z;

		if (t.PlaceCharacter(e) == false) {
			return null;
		}

		return e;
	}

	public void Update(float deltaTime) {
		//Debug.Log("Character Update");

		if (cbEnemyChanged != null)
			cbEnemyChanged(this);
	}

	public void RemoveCharacter() {
		Debug.Log("Remove Character");

		charStartTile.RemoveCharacter();

		if (cbOnRemoved != null)
			cbOnRemoved(this);

		//World.InvalidateTileGraph();

		// At this point, no DATA structures should be pointing to us, so we
		// should get garbage-collected.

	}

	public void RegisterOnChangedCallback(Action<Enemy> cb) {
		cbEnemyChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Enemy> cb) {
		cbEnemyChanged -= cb;
	}

	public void RegisterOnRemovedCallback(Action<Enemy> callbackFunc) {
		cbOnRemoved += callbackFunc;
	}

	public void UnregisterOnRemovedCallback(Action<Enemy> callbackFunc) {
		cbOnRemoved -= callbackFunc;
	}

}

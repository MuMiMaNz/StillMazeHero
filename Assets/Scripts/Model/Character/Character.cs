﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

	//Action<Character> cbCharacterChanged;
	//Action<Character> cbOnRemoved;

	public string objectType { get; protected set; }
	public string parent { get; protected set; }

	public int MaxHP { get; protected set; }
	public int HP { get; protected set; }
	public float speed { get; protected set; }
	public float ATKIntervalTime { get; protected set; }
	public float ATKAnimTime { get;  set; }

	public float involuntaryTime { get; protected set; }
	public bool canTakeDMG { get; protected set; }

	public Stat STR { get; protected set; }
	public Stat INT { get; protected set; }
	public Stat VIT { get; protected set; }
	public Stat DEX { get; protected set; }
	public Stat AGI { get; protected set; }
	public Stat LUK { get; protected set; }

	public int DEF { get; protected set; }
	public int mDEF { get; protected set; }

	public Tile charStartTile { get; protected set; }

	public Stat GetPrimaryStat() {
		if (STR.isPrimaryStat) return STR;
		else if (INT.isPrimaryStat) return INT;
		else if (VIT.isPrimaryStat) return VIT;
		else if (DEX.isPrimaryStat) return DEX;
		else if (AGI.isPrimaryStat) return AGI;
		else if (LUK.isPrimaryStat) return LUK;
		else return null;
	}


	//public Character() {

	//}

	//protected Character(Character other) {
	//	this.objectType = other.objectType;
	//	this.HP = other.HP;
	//	this.speed = other.speed;

	//	this.STR = other.STR;
	//	this.INT = other.INT;
	//	this.VIT = other.VIT;
	//	this.DEX = other.DEX;
	//	this.AGI = other.AGI;
	//	this.LUK = other.LUK;

	//	//this.X = other.X;
	//	//this.Z = other.Z;
	//	this.parent = other.parent;

	//	//this.bldParamaters = new Dictionary<string, float>(other.bldParamaters);

	//	//if (other.updateActions != null)
	//	//	this.updateActions = (Action<Building, float>)other.updateActions.Clone();
	//}

	//virtual public Character Clone() {
	//	return new Character(this);
	//}

	//public Character(string objectType,int STR = 1,int INT = 1, int VIT = 1, int DEX = 1, int AGI = 1, int LUK = 1,  float HP = 100f, float speed = 1, string parent = "Character") {
	//	this.objectType = objectType;
	//	this.HP = HP;
	//	this.speed = speed;
	//	this.STR = STR;
	//	this.INT = INT;
	//	this.VIT = VIT;
	//	this.DEX = DEX;
	//	this.AGI = AGI;
	//	this.LUK = LUK;
		
	//	//this.X = charStartTile.X;
	//	//this.Z = charStartTile.Z;
	//	this.parent = parent;
		
	//	//bldParamaters = new Dictionary<string, float>();
	//}

	//static public Character PlaceCharacter(Character proto, Tile t) {
		
	//	Character c = proto.Clone();

	//	c.charStartTile = t;
	//	c.X = t.X;
	//	c.Z = t.Z;

	//	if (t.PlaceCharacter(c) == false) {
	//		return null;
	//	}

	//	return c;
	//}

	//public void Update(float deltaTime) {
	//	//Debug.Log("Character Update");
		
	//	if (cbCharacterChanged != null)
	//		cbCharacterChanged(this);
	//}

	//public void RemoveCharacter() {
	//	Debug.Log("Remove Character");

	//	charStartTile.RemoveCharacter();

	//	if (cbOnRemoved != null)
	//		cbOnRemoved(this);

	//	//World.InvalidateTileGraph();

	//	// At this point, no DATA structures should be pointing to us, so we
	//	// should get garbage-collected.

	//}

	//public void RegisterOnChangedCallback(Action<Character> cb) {
	//	cbCharacterChanged += cb;
	//}

	//public void UnregisterOnChangedCallback(Action<Character> cb) {
	//	cbCharacterChanged -= cb;
	//}

	//public void RegisterOnRemovedCallback(Action<Character> callbackFunc) {
	//	cbOnRemoved += callbackFunc;
	//}

	//public void UnregisterOnRemovedCallback(Action<Character> callbackFunc) {
	//	cbOnRemoved -= callbackFunc;
	//}
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

	Action<Player> cbPlayerChanged;
	Action<Player> cbOnRemoved;

	public string name { get; protected set; }
	//public List<Weapon> weapons { get; protected set; }
	// Use Dictionary to Write/Load data
	protected Dictionary<int, string> weaponsDict;

	// Empty constructor is used for serialization
	public Player() {
		//weapons = new List<Weapon>();
		weaponsDict = new Dictionary<int, string>();
	}

	// Copy Constructor -- don't call this directly, unless we never
	// do ANY sub-classing. Instead use Clone(), which is more virtual.
	protected Player(Player other) {
		this.name = other.name;
		this.objectType = other.objectType;
		this.HP = other.HP;
		this.speed = other.speed;

		this.STR = other.STR;
		this.INT = other.INT;
		this.VIT = other.VIT;
		this.DEX = other.DEX;
		this.AGI = other.AGI;
		this.LUK = other.LUK;

		//this.X = other.X;
		//this.Z = other.Z;
		this.parent = other.parent;

		if (other.weaponsDict != null) {
			this.weaponsDict = new Dictionary<int, string>(other.weaponsDict);
		}else {
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

	// Create furniture from parameters -- this will probably ONLY ever be used for prototypes
	public Player(string objectType, string name, int STR = 1, int INT = 1, int VIT = 1, int DEX = 1, int AGI = 1, int LUK = 1, float HP = 100f, float speed = 1, string parent = "Character") {

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

		//weapons = new List<Weapon>();

		//bldParamaters = new Dictionary<string, float>();
	}
	

	static public Player PlacePlayer(Player proto, Tile t) {

		Player p = proto.Clone();

		p.charStartTile = t;
		p.X = t.X;
		p.Z = t.Z;

		if (t.PlaceCharacter(p) == false) {
			return null;
		}

		return p;
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

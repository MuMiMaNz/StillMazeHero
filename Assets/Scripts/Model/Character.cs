﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

	Action<Character> cbCharacterChanged;
	Action<Character> cbOnRemoved;

	public string objectType { get; protected set; }
	public string parent { get; protected set; }

	public float HP { get; protected set; }
	public float speed { get; protected set; }
	
	public float X { get; protected set; }
	public float Z { get; protected set; }

	public Tile charStartTile { get; protected set; }

	protected Character(Character other) {
		this.objectType = other.objectType;
		this.HP = other.HP;
		this.speed = other.speed;
		//this.X = other.X;
		//this.Z = other.Z;
		this.parent = other.parent;

		//this.bldParamaters = new Dictionary<string, float>(other.bldParamaters);

		//if (other.updateActions != null)
		//	this.updateActions = (Action<Building, float>)other.updateActions.Clone();
	}

	virtual public Character Clone() {
		return new Character(this);
	}

	public Character(string objectType, float HP = 100f, float speed = 1, string parent = "Character") {
		this.objectType = objectType;
		this.HP = HP;
		this.speed = speed;
		
		//this.X = charStartTile.X;
		//this.Z = charStartTile.Z;
		this.parent = parent;
		
		//bldParamaters = new Dictionary<string, float>();
	}

	static public Character PlaceCharacter(Character proto, Tile t) {
		
		Character c = proto.Clone();

		c.charStartTile = t;
		c.X = t.X;
		c.Z = t.Z;

		if (t.PlaceCharacter(c) == false) {
			return null;
		}

		return c;
	}

	public void Update(float deltaTime) {
		//Debug.Log("Character Update");
		
		if (cbCharacterChanged != null)
			cbCharacterChanged(this);
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

	public void RegisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged -= cb;
	}

	public void RegisterOnRemovedCallback(Action<Character> callbackFunc) {
		cbOnRemoved += callbackFunc;
	}

	public void UnregisterOnRemovedCallback(Action<Character> callbackFunc) {
		cbOnRemoved -= callbackFunc;
	}
}

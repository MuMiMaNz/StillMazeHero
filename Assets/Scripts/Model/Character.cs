using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

	Action<Character> cbCharacterChanged;

	public string objectType { get; protected set; }
	public string parent { get; protected set; }

	public float HP { get; protected set; }
	public float speed { get; protected set; }
	//public Tile currentTile { get; set; }
	public float X { get; protected set; }
	public float Z { get; protected set; }



	//public Character(Tile startTile) {
	//	currentTile = startTile;
	//}

	protected Character(Character other) {
		this.objectType = other.objectType;
		this.HP = other.HP;
		this.speed = other.speed;
		//this.currentTile = other.currentTile;
		this.X = other.X;
		this.Z = other.Z;
		this.parent = other.parent;

		//this.bldParamaters = new Dictionary<string, float>(other.bldParamaters);

		//if (other.updateActions != null)
		//	this.updateActions = (Action<Building, float>)other.updateActions.Clone();
	}

	virtual public Character Clone() {
		return new Character(this);
	}

	public Character(string objectType, float X, float Z, float HP = 100f, float speed = 1, string parent = "Character") {
		this.objectType = objectType;
		this.HP = HP;
		this.speed = speed;
		//this.currentTile = currentTile;
		this.X = X;
		this.Z = Z;
		this.parent = parent;
		
		//bldParamaters = new Dictionary<string, float>();
	}

	public Character PlaceCharacter(Character proto, float X, float Z) {
		
		Character obj = proto.Clone();

		//obj.currentTile = tile;
		obj.X = X;
		obj.Z = Z;

		return obj;
	}

	public void Update(float deltaTime) {
		//Debug.Log("Character Update");
		
		if (cbCharacterChanged != null)
			cbCharacterChanged(this);
	}

	public void RegisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged -= cb;
	}
}

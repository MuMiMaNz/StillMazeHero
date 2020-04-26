using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    
	public string name { get; protected set; }

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

		//bldParamaters = new Dictionary<string, float>();
	}

	static public Player PlacePlayer(Player proto, Tile t) {

		Player p = new Player(proto.objectType, proto.name,
			proto.STR, proto.INT, proto.VIT, proto.DEX, proto.AGI, proto.LUK,
			proto.HP, proto.speed, proto.parent);

		p.charStartTile = t;
		p.X = t.X;
		p.Z = t.Z;

		if (t.PlaceCharacter(p) == false) {
			return null;
		}

		return p;
	}

}

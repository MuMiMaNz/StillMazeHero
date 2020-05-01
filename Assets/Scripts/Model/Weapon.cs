using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon  {

	public enum WeaponType {Melee, Range }

	public string wName { get; protected set; }
	public string wDescription { get; protected set; }

	public int wATK { get; protected set; }
	public int wMagicATK { get; protected set; }
	public int wATKspeed { get; protected set; }

	public int wDEF { get; protected set; }
	public int wMagicDEF { get; protected set; }

	public WeaponType weaponType { get; protected set; }
	public int wSlot { get; protected set; }


}

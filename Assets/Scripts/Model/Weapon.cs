using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { OneHandMelee, TwoHandMelee, Shield, Range }
public enum WeaponSide { Left, Right }

public class Weapon  {


	public string objectType { get; protected set; }
	public string wName { get; protected set; }
	public string wDescription { get; protected set; }

	public int wATK { get; protected set; }
	public int wMagicATK { get; protected set; }
	public int wATKspeed { get; protected set; }

	public int wDEF { get; protected set; }
	public int wMagicDEF { get; protected set; }

	public WeaponType weaponType { get; protected set; }
	public WeaponSide wSide { get; protected set; }
	//public int wSlotNO { get; protected set; }
	public int wSpace { get; protected set; }
	

	//public bool primary { get; protected set; }

	protected Weapon(Weapon other) {
		this.objectType = other.objectType;
		this.wName = other.wName;
		this.wDescription = other.wDescription;

		this.wATK = other.wATK;
		this.wMagicATK = other.wMagicATK;
		this.wATKspeed = other.wATKspeed;

		this.wDEF = other.wDEF;
		this.wMagicDEF = other.wMagicDEF;

		this.weaponType = other.weaponType;
		this.wSide = other.wSide;
		this.wSpace = other.wSpace;

		//this.primary = false;
	}

	public virtual Weapon Clone() {
		return new Weapon(this);
	}

	public Weapon(string objectType, string wName, string wDescription,
		int wATK, int wMagicATK,int wATKspeed, int wDEF, int wMagicDEF,
		 WeaponType weaponType,WeaponSide wSide,int wSpace) {
		this.objectType = objectType;
		this.wName = wName;
		this.wDescription = wDescription;

		this.wATK = wATK;
		this.wMagicATK = wMagicATK;
		this.wATKspeed = wATKspeed;

		this.wDEF = wDEF;
		this.wMagicDEF = wMagicDEF;

		this.weaponType = weaponType;
		this.wSide = wSide;
		this.wSpace = wSpace;

		//primary = false;
	}

	//public void SetPrimaryWeapon(bool isPrimary) {
	//	primary = isPrimary;
	//}
	//public void SetSlotNO(int no) {
	//	wSlotNO = no;
	//}
}

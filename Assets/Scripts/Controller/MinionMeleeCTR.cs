using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMeleeCTR : MonoBehaviour
{
    // This Script attach to MinionMelee GO
	public GameObject MinionGO;
	World World {
		get { return WorldController.Instance.World; }
	}	private CharacterGraphicController cgc;
	private HealthBar playerHPBar ;
	private Player p;
	private Minion m;

	private void OnEnable() {
		cgc = GameObject.Find("CharacterController").GetComponent<CharacterGraphicController>();
	}
	
	public void SetPlayMode(){
		m = DictionaryHelper.KeyByValue<Minion,GameObject>(cgc.minionGameObjectMap, MinionGO);
		p = World.player;
		playerHPBar = GameObject.Find("PlayerHPBar").GetComponent<HealthBar>();
		playerHPBar.HealthBarChange(p.HP,p.MaxHP);
	}
	public void RemoveMinionMeleeCTR(){
		m = null;
		p = null;
		playerHPBar = null;
		cgc = null;
	}

	private void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag == "Player" && m.minionState == MinionState.Attack) {
			//Debug.Log(MinionGO.name + "  Hit  " + other.gameObject.name);
			FloatingTextController.CreateFloatingDMG(p.TakeDamage(m), other.transform);
		
			// Update Health Bar
			playerHPBar.HealthBarChange(p.HP,p.MaxHP);
		}
	}
    
}

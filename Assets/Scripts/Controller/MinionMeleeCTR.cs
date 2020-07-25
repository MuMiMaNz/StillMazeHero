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
	private PlayerController playerController;

	private void OnEnable() {
		//playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		cgc = GameObject.Find("CharacterController").GetComponent<CharacterGraphicController>();
	}

	private void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag == "Player" ) {
			//Debug.Log(MinionGO.name + "  Hit  " + other.gameObject.name);
			Minion m = DictionaryHelper.KeyByValue<Minion,GameObject>(cgc.minionGameObjectMap, MinionGO);
			Player p = World.player;

			p.TakeDamage(m);
		}
	}
    
}

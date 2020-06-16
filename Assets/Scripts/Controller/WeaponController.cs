using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

	// This Script attach to weapons
	private CharacterGraphicController cgc;
	private PlayerController playerController;

	private void OnEnable() {
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		cgc = GameObject.Find("CharacterController").GetComponent<CharacterGraphicController>();
	}

	private void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag == "Minion" && playerController.normalAttack == true) {
			//Debug.Log(this.name + " Hit " + other.gameObject.name);
			
			Minion m = DictionaryHelper.KeyByValue<Minion,GameObject>(cgc.minionGameObjectMap, other.gameObject);
			FloatingTextController.CreateFloatingDMG(m.CalAndTakeDamage().ToString("F0"), other.transform); 

		}
	}


}

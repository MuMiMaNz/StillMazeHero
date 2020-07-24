using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMeleeCTR : MonoBehaviour
{
    // This Script attach to MinionMelee GO
	private CharacterGraphicController cgc;
	private PlayerController playerController;

	private void OnEnable() {
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		cgc = GameObject.Find("CharacterController").GetComponent<CharacterGraphicController>();
	}

	private void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag == "Player" ) {
			Debug.Log(this.name + "  Hit  " + other.gameObject.name);
			
			
			
		}
	}
    
}

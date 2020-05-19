using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

	// This Script attach to weapons

	private PlayerController playerController;

	private void Start() {
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
	}

	private void OnTriggerEnter(Collider other) {

		Debug.Log(playerController.normalAttack);

		if (other.gameObject.tag == "Minion" && playerController.normalAttack == true) {
			Debug.Log(this.name + " Hit " + other.gameObject.name);
		}
	}


}

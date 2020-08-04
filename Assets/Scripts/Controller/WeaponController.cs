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
			
			//Find Minion and Minion GO
			Minion m = DictionaryHelper.KeyByValue<Minion,GameObject>(cgc.minionGameObjectMap, other.gameObject);
			GameObject m_go = cgc.minionGameObjectMap[m];

			// Calculate DMG
			m.MinionGethit();
			if (m.canTakeDMG == true) {
				FloatingTextController.CreateFloatingDMG(m.CalAndTakeDamage(), other.transform);
			}
			
			// Update Health Bar
			GameObject m_canvas = m_go.transform.Find("CharCanvas").gameObject;
			if (m_canvas.activeSelf == false){
				m_canvas.SetActive(true);
			}
			HealthBar hb = m_canvas.GetComponentInChildren<HealthBar>();
			hb.HealthBarChange(m.HP,m.MaxHP);
			
		}
	}


}

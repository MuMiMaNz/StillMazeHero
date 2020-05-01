using System.Collections.Generic;
using UnityEngine;

public class CharacterGraphicController : MonoBehaviour {

	World World {
		get { return WorldController.Instance.World; }
	}
	Dictionary<Player, GameObject> playerGameObjectMap;
	Dictionary<Enemy, GameObject> enemyGameObjectMap;
	Dictionary<string, GameObject> characterGOS;
	Dictionary<string, GameObject> weaponGOS;

	public PlayerController playerController;

	void Start() {
		playerGameObjectMap = new Dictionary<Player, GameObject>();
		enemyGameObjectMap = new Dictionary<Enemy, GameObject>();

		LoadCharacterPrefabs();
		LoadWeaponPrefabs();
		// Register our callback so that our Character gets created
		World.RegisterPlayerCreated(OnPlayerCreated);
		World.RegisterEnemyCreated(OnEnemyCreated);

	}
	
	//private void FixedUpdate() {

	//	if (WorldController.Instance.gameMode == GameMode.PlayMode) {
	//		// Update GO position
	//		foreach (Character c in World.characters) {
	//			GameObject c_go = characterGameObjectMap[c];
	//			if (c.objectType == "Player") {

	//				Rigidbody rb = c_go.GetComponent<Rigidbody>();

	//				rb.MovePosition(rb.position + playerController.playerMoveDT * Time.fixedDeltaTime);

	//				//c_go.GetComponent<Rigidbody>().MovePosition(c_go.GetComponent<Rigidbody>().position + moveDT * Time.deltaTime);
	//				World.player.X = c_go.transform.position.x;
	//				World.player.Z = c_go.transform.position.z;
	//			}
	//		}
	//	}
	//}

	//  Load 3D Game Object Here
	void LoadCharacterPrefabs() {
		characterGOS = new Dictionary<string, GameObject>();
		GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs/Characters/");

		foreach (GameObject go in gos) {
			//Debug.Log("LOADED RESOURCE:" + go);
			characterGOS[go.name] = go;
		}
	}
	void LoadWeaponPrefabs() {
		weaponGOS = new Dictionary<string, GameObject>();
		GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs/Weapons/");

		foreach (GameObject go in gos) {
			//Debug.Log("LOADED Weapon:" + go);
			weaponGOS[go.name] = go;
		}
	}

	public void OnPlayerCreated(Player p) {
		// This creates a new GameObject and adds it to our scene.
		GameObject p_go = GetGOforCharacter(p);

		if (p_go != null) {
			// Add our tile/GO pair to the dictionary.
			playerGameObjectMap.Add(p, p_go);

			p_go.name = p.objectType;
			p_go.transform.position = new Vector3(p.charStartTile.X, -0.5f, p.charStartTile.Z);
			p_go.transform.SetParent(this.transform.Find(p.parent), true);
			// Register our callback so that our GameObject gets updated whenever
			// the object's into changes.
			p.RegisterOnRemovedCallback(OnPlayerRemoved);

			// Add Weapon&Armor to Player GO
			foreach (Weapon w in p.weapons) {
				if (w.wSlotNO == 0) {
					GameObject w_go = GetGOforWeapon(w);
					w_go.name = w.wName;
					w_go.transform.SetParent(p_go.transform.Find("root/weaponShield_r"), false);
					w_go.transform.localPosition = new Vector3(0, 0, 0);
					// Set to Primary weapon
					w.SetPrimaryWeapon(true);
				}
			}
		}
	}

	public void OnEnemyCreated(Enemy e) {
		//Debug.Log("OnCharacterCreated");
		// Create a visual GameObject linked to this data.

		// FIXME: Does not consider multi-tile objects nor rotated objects

		// This creates a new GameObject and adds it to our scene.
		GameObject e_go = GetGOforCharacter(e);

		if (e_go != null) {
			// Add our tile/GO pair to the dictionary.
			enemyGameObjectMap.Add(e, e_go);

			e_go.name = e.objectType + "_" + e.charStartTile.X + "_" + e.charStartTile.Z;
			
			e_go.transform.position = new Vector3(e.charStartTile.X, -0.5f, e.charStartTile.Z);
			e_go.transform.SetParent(this.transform.Find(e.parent), true);

			// Register our callback so that our GameObject gets updated whenever
			// the object's into changes.
			//c.RegisterOnChangedCallback(OnCharacterChanged);
			e.RegisterOnRemovedCallback(OnEnemyRemoved);
		}
	}

	void OnPlayerRemoved(Player p) {
		if (playerGameObjectMap.ContainsKey(p) == false) {
			Debug.LogError("OnFurnitureRemoved -- trying to Remove graphic for character not in our map.");
			return;
		}

		GameObject p_go = playerGameObjectMap[p];
		Destroy(p_go);
		playerGameObjectMap.Remove(p);
	}

	void OnEnemyRemoved(Enemy e) {
		if (enemyGameObjectMap.ContainsKey(e) == false) {
			Debug.LogError("OnFurnitureRemoved -- trying to Remove graphic for character not in our map.");
			return;
		}

		GameObject e_go = enemyGameObjectMap[e];
		Destroy(e_go);
		enemyGameObjectMap.Remove(e);
	}

	//void OnCharacterChanged(Character c) {
	//	//Debug.Log("OnCharacterChanged");
	//	// Make sure the character's graphics are correct.
	//	if (characterGameObjectMap.ContainsKey(c) == false) {
	//		Debug.LogError("OnCharacterChanged -- trying to change visuals for character not in our map.");
	//		return;
	//	}

	//	GameObject c_goOld = characterGameObjectMap[c];
	//	//Debug.Log(c_goOld);

	//	GameObject c_goNew = GetGOforCharacter(c);
	//	c_goNew.name = c_goOld.name;
	//	c_goNew.transform.position = c_goOld.transform.position;
	//	c_goNew.transform.SetParent(this.transform.Find(c.parent), true);

	//	Destroy(c_goOld);

	//	// --- Delete Old and Add New Character game object
	//	characterGameObjectMap.Remove(c);
	//	characterGameObjectMap.Add(c, c_goNew);
	//}

	public GameObject GetPreviewCharacter(string objectType) {
		return Instantiate(characterGOS[objectType]);
	}

	private GameObject GetGOforCharacter(Character c) {

		if (c.objectType == "DummyCharacter")
			return null;

		return Instantiate(characterGOS[c.objectType]);
	}

	private GameObject GetGOforWeapon(Weapon w) {
		return Instantiate(weaponGOS[w.objectType]);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGraphicController : MonoBehaviour {

	World World {
		get { return WorldController.Instance.World; }
	}

	Dictionary<Character, GameObject> characterGameObjectMap;
	Dictionary<string, GameObject> characterGOS;

	public TouchController touchController;

	void Start() {

		characterGameObjectMap = new Dictionary<Character, GameObject>();

		LoadPrefabs();

		// Register our callback so that our GameObject gets updated whenever
		// the tile's type changes.
		World.RegisterCharacterCreated(OnCharacterCreated);

		// Go through any EXISTING character (i.e. from a save that was loaded OnEnable) and call the OnCreated event manually
		foreach (Character c in World.characters) {
			OnCharacterCreated(c);
		}
	}

	//  Load 3D Game Object Here
	void LoadPrefabs() {
		characterGOS = new Dictionary<string, GameObject>();
		GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs/Characters/");

		foreach (GameObject go in gos) {
			//Debug.Log("LOADED RESOURCE:" + go);
			characterGOS[go.name] = go;
		}
	}

	public void OnCharacterCreated(Character c) {
		//Debug.Log("OnCharacterCreated");
		// Create a visual GameObject linked to this data.

		// FIXME: Does not consider multi-tile objects nor rotated objects

		// This creates a new GameObject and adds it to our scene.
		GameObject c_go = GetGOforCharacter(c);

		if (c_go != null) {
			// Add our tile/GO pair to the dictionary.
			characterGameObjectMap.Add(c, c_go);

			if (c.objectType == "Player") {
				c_go.name = "Player";
			}else {
				c_go.name = c.objectType + "_" + c.charStartTile.X + "_" + c.charStartTile.Z;
			}
			c_go.transform.position = new Vector3(c.charStartTile.X, -0.5f, c.charStartTile.Z);
			c_go.transform.SetParent(this.transform.Find(c.parent), true);

			// Register our callback so that our GameObject gets updated whenever
			// the object's into changes.
			//c.RegisterOnChangedCallback(OnCharacterChanged);
			c.RegisterOnRemovedCallback(OnCharacterRemoved);
		}

	}

	void OnCharacterRemoved(Character c) {
		if (characterGameObjectMap.ContainsKey(c) == false) {
			Debug.LogError("OnFurnitureRemoved -- trying to Remove graphic for character not in our map.");
			return;
		}

		GameObject c_go = characterGameObjectMap[c];
		Destroy(c_go);
		characterGameObjectMap.Remove(c);
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
}

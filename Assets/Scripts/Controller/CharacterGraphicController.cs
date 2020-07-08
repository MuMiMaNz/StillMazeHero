using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGraphicController : MonoBehaviour {

	World World {
		get { return WorldController.Instance.World; }
	}
	private Dictionary<Player, GameObject> playerGameObjectMap;
	public Dictionary<Minion, GameObject> minionGameObjectMap { get; protected set; }
	private Dictionary<string, GameObject> characterGOS;
	private Dictionary<string, GameObject> weaponGOS;

	public PlayerController playerController;
	private GameObject playerGO;
		// Minion FOW
	public LayerMask targetMask;
	public LayerMask obstacleMask;

	void Start() {
		playerGameObjectMap = new Dictionary<Player, GameObject>();
		minionGameObjectMap = new Dictionary<Minion, GameObject>();

		LoadCharacterPrefabs();
		LoadWeaponPrefabs();
		// Register our callback so that our Character gets created
		World.RegisterPlayerCreated(OnPlayerCreated);
		World.RegisterMinionCreated(OnMinionCreated);

		// from a save that was loaded and call the OnCreated 
		foreach (Minion m in World.minions) {
			OnMinionCreated(m);
		}
	}

	//  Load 3D Game Object Here
	void LoadCharacterPrefabs() {
		characterGOS = new Dictionary<string, GameObject>();
		GameObject[] gos = Resources.LoadAll<GameObject>("Prefabs/Characters/");

		foreach (GameObject go in gos) {
			//Debug.Log("LOADED Character name:" + go.name);
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

	#region Player Graphic

	public void OnPlayerCreated(Player p) {
		// This creates a new GameObject and adds it to our scene.
		GameObject p_go = GetGOforCharacter(p);
		playerGO = p_go;

		if (p_go != null) {
			// Add our tile/GO pair to the dictionary.
			playerGameObjectMap.Add(p, p_go);

			p_go.name = p.objectType;
			p_go.transform.position = new Vector3(p.charStartTile.X, -0.5f, p.charStartTile.Z);
			p_go.transform.SetParent(this.transform.Find(p.parent), true);
			
			p.RegisterOnRemovedCallback(OnPlayerRemoved);

			// Add Weapon&Armor to Player GO
			foreach (Weapon w in p.primaryWeapons) {
				bool isTwoHandWeapon = false; // Check is Two-handed weapon?

				if (w.wSide == WeaponSide.Right ) {
					GameObject w_go = GetGOforWeapon(w);
					w_go.name = w.wName;
					w_go.transform.SetParent(p_go.transform.Find("root/weaponShield_r"), false);
					w_go.transform.localPosition = new Vector3(0, 0, 0);
					
					if(w.weaponType == WeaponType.TwoHandMelee || w.weaponType == WeaponType.Range) {
						isTwoHandWeapon = true;
					}
				}else if (w.wSide == WeaponSide.Left && isTwoHandWeapon==false) {
					GameObject w_go = GetGOforWeapon(w);
					w_go.name = w.wName;
					w_go.transform.SetParent(p_go.transform.Find("root/weaponShield_l"), false);
					w_go.transform.localPosition = new Vector3(0, 0, 0);
					
				}
			}
		}
	}

	private void OnPlayerRemoved(Player p) {
		if (playerGameObjectMap.ContainsKey(p) == false) {
			Debug.LogError("OnFurnitureRemoved -- trying to Remove graphic for character not in our map.");
			return;
		}

		GameObject p_go = playerGameObjectMap[p];
		Destroy(p_go);
		playerGameObjectMap.Remove(p);
	}
	#endregion

	#region Minion Graphic

	public void OnMinionCreated(Minion m) {

		GameObject m_go = GetGOforCharacter(m);

		if (m_go != null) {
			// Add our tile/GO pair to the dictionary.
			minionGameObjectMap.Add(m, m_go);

			m_go.name = m.objectType + "_" + m.X + "_" + m.Z;
			
			m_go.transform.position = new Vector3(m.X, 0f, m.Z);
			m_go.transform.SetParent(this.transform.Find(m.parent), true);

			// Create Character mini-canvas
			CharCanvasController.CreateHealthBar(m_go.transform);

			// Register our callback so that our GameObject gets updated whenever
			// the object's into changes.
			m.RegisterOnChangedCallback(OnMinionChanged);
			
			m.RegisterOnRemovedCallback(OnMinionRemoved);

			StartCoroutine("FindTargetsWithDelay", m);
			//m.RegisterCouroutineCallback(MinionCoroutine);
		}
	}

	private void OnMinionChanged(Minion m) {
		//Debug.Log("OnMinionChanged");

		if (minionGameObjectMap.ContainsKey(m) == false) {
			Debug.LogError("OnCharacterChanged -- trying to change visuals for Minion not in our map.");
			return;
		}
		GameObject m_go = minionGameObjectMap[m];
		Animator m_anim = m_go.GetComponent<Animator>();

		// Find Canvas in child
		GameObject m_canvas = m_go.transform.Find("CharCanvas").gameObject;
		HealthBar hb = m_canvas.GetComponentInChildren<HealthBar>();
		hb.HealthBarChange(m.HP,m.MaxHP);
		// Rotate canvas to main camera
		m_canvas.transform.position = m_go.transform.position + new Vector3(0, 0, 0.5f);
		m_canvas.transform.rotation = Camera.main.transform.rotation;

		// Set position
		if (m.minionState == MinionState.ChaseStraight) {
			Vector3 actualTargetPos = new Vector3(playerGO.transform.position.x, 0, playerGO.transform.position.z);
			m_go.transform.position = Vector3.MoveTowards(m_go.transform.position, actualTargetPos, m.speed * Time.deltaTime);
			m.currTile = new Tile(World, Mathf.RoundToInt(m_go.transform.position.x), Mathf.RoundToInt(m_go.transform.position.z));
		}
		else {
			m_go.transform.position = new Vector3(m.X, 0, m.Z);
		}
		// Set front rotation
		if (m.directionVector != Vector3.zero) {
			if (m.seePlayer == false) {
				m_go.transform.rotation = Quaternion.Slerp(m_go.transform.rotation, Quaternion.LookRotation(m.directionVector), 0.08f);	
			}
			else {
				Vector3 direction = playerGO.transform.position - m_go.transform.position;
				m_go.transform.rotation = Quaternion.Slerp(m_go.transform.rotation, Quaternion.LookRotation(direction), 0.08f);
				//m_go.transform.LookAt(playerGO.transform);
			}
		}
		
		// Set Animator variable
		switch (m.minionState)
		{
			case MinionState.Idle:
				m_anim.SetBool("isIdle",true);
				m_anim.SetBool("isWalk",false);
				m_anim.SetBool("isAttack",false);
				m_anim.SetBool("isGetHit", false);
				m_anim.SetBool("isDie", false);
				break ;
			case MinionState.Patrol:
				m_anim.SetBool("isIdle",false);
				m_anim.SetBool("isWalk",true);
				m_anim.SetBool("isAttack",false);
				m_anim.SetBool("isGetHit", false);
				m_anim.SetBool("isDie", false);
				break ;
			case MinionState.Chase:
				m_anim.SetBool("isIdle",false);
				m_anim.SetBool("isWalk",true);
				m_anim.SetBool("isAttack",false);
				m_anim.SetBool("isGetHit", false);
				m_anim.SetBool("isDie", false);
				break ;
			case MinionState.Attack:
				m_anim.SetBool("isIdle",false);
				m_anim.SetBool("isWalk",false);
				m_anim.SetBool("isAttack",true);
				m_anim.SetBool("isGetHit", false);
				m_anim.SetBool("isDie", false);
				break ;
			case MinionState.GetHit:
				m_anim.SetBool("isIdle", false);
				m_anim.SetBool("isWalk", false);
				m_anim.SetBool("isAttack", false);
				m_anim.SetBool("isGetHit", true);
				m_anim.SetBool("isDie", false);

				break;
			case MinionState.Die:
				m_anim.SetBool("isIdle", false);
				m_anim.SetBool("isWalk", false);
				m_anim.SetBool("isAttack", false);
				m_anim.SetBool("isDie", true);
				break;
			default:
				break;
		}
		if (m.minionState == MinionState.Die &&
			m_anim.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
			m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
			m.Death();
		}
		if(m.minionState == MinionState.GetHit && m.seePlayer == false) {
			Vector3 direction = playerGO.transform.position - m_go.transform.position;
			m_go.transform.rotation = Quaternion.Slerp(m_go.transform.rotation, Quaternion.LookRotation(direction), 0.08f);
			m.seePlayer = true;
		}
	}

	private IEnumerator FindTargetsWithDelay(Minion m) {
		//Debug.Log("Coroutine FindVisibleTargets");
		while (m.minionState != MinionState.Die) {
			yield return new WaitForSeconds(.2f);
			FindVisibleTargets(m);
		}
	}

	private void FindVisibleTargets(Minion m) {

		if (minionGameObjectMap.ContainsKey(m) == false) { return; }

		GameObject m_go = minionGameObjectMap[m];

		Collider[] overlaps = Physics.OverlapSphere(m_go.transform.position, m.viewRadius, targetMask);

		if (overlaps.Length <= 0) { m.seePlayer = false; return; }

		for (int i = 0; i < overlaps.Length ; i++) {

			if (overlaps[i] != null) {

				Transform target = overlaps[i].transform;

				Vector3 directionBetween = (target.position - m_go.transform.position).normalized;
				//directionBetween.y *= 0;
				float distanceBetween = Vector3.Distance(target.position, m_go.transform.position);

				float angle = Vector3.Angle(m_go.transform.forward, directionBetween);

				// If Player is in minion angle FOV
				if (angle <= m.viewAngle) {
					
					// No obstruction in minion ray to player
					Ray ray = new Ray(m_go.transform.position, target.position - m_go.transform.position);
					if (!Physics.Raycast(m_go.transform.position, target.position - m_go.transform.position, m.viewRadius, obstacleMask)) {
						//Debug.Log(m_go.name + "  See Player !");
						m.seePlayer = true;

						// If Player in ATK range
						if (distanceBetween < m.chaseStraightRange && distanceBetween > m.ATKRange) {
							m.playerInChaseStraight = true;
							m.playerInATKRange = false;
						}
						else if (distanceBetween <= m.ATKRange){
							m.playerInChaseStraight = false;
							m.playerInATKRange = true;
						}else{
							m.playerInChaseStraight = false;
							m.playerInATKRange = false;
						}
					}else {
						m.seePlayer = false;
						m.playerInChaseStraight = false;
						m.playerInATKRange = false;
					}
				}
				else {
					m.seePlayer = false;
					m.playerInChaseStraight = false;
					m.playerInATKRange = false;
				}
			}
		}
	}


	private void OnMinionRemoved(Minion m) {
		if (minionGameObjectMap.ContainsKey(m) == false) {
			Debug.LogError("OnMinionRemoved -- trying to Remove graphic for character not in our map.");
			return;
		}

		GameObject m_go = minionGameObjectMap[m];
		Destroy(m_go);
		minionGameObjectMap.Remove(m);
		World.minions.Remove(m);
		
	}

	#endregion

	public GameObject GetPreviewCharacter(string objectType) {
		//Debug.Log("Preview" + objectType);
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

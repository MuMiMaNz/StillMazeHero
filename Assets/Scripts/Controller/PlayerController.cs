using System.Collections;
using System.Collections.Generic;
using TouchControlsKit;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	World World {
		get { return WorldController.Instance.World; }
	}
	public CameraController camController;
	private Transform camTransform;

	public Vector3 joyMoveDT { get; protected set; }

	public bool normalAttack { get;  set; }
	private bool pressedAttack;

	public GameObject playerGO { get; protected set; }
	private Rigidbody rb;
	private Animator playerAnim;

	//private string[] randomAttacks = { "Attack01", "Attack02" };

	//private float moveFT = 0.75f;
	private Quaternion playerRotateDT;

	// Field of view
	public float viewRadius = 1.5f;
	[Range(0, 360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	// Start Play mode
	public void StartPlayMode() {

		// Find Player gameobject
		GameObject[] playerTag = GameObject.FindGameObjectsWithTag("Player");
		if (playerTag.Length != 1) {
			Debug.LogError("More than 1 Player tag GO");
		} else {
			
			StartCoroutine("FindTargetsWithDelay", .2f);

			playerGO = playerTag[0];
			rb = playerGO.GetComponent<Rigidbody>();
			playerAnim = playerGO.GetComponent<Animator>();
			playerAnim.SetBool("canMove", true);
			playerRotateDT = rb.rotation;
			joyMoveDT = Vector3.zero;
			camTransform = camController.transform;
		}
	}

	public void Update() {

		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			// Get joystick move and update Animator
			Vector2 move = TCKInput.GetAxis("Joystick");
			joyMoveDT = new Vector3(move.x , 0, move.y );
			playerAnim.SetFloat("speed", joyMoveDT.magnitude);

			// Check canMove
			bool canMove = playerAnim.GetBool("canMove");
			// Get button press
			pressedAttack = TCKInput.GetAction("AtkButton", EActionEvent.Down);

			if (pressedAttack && canMove) {
				PlayerNormalAttack();
			}
			//camController.MoveCamOnPlayerMove(move);
		}
	}

	private void FixedUpdate() {

		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			// Joystick is moving
			if (joyMoveDT != Vector3.zero && playerAnim.GetBool("canMove")) { 
				
				// Adjust jostict direction to camera angle
				Vector3 newPos = camTransform.rotation * joyMoveDT;
				newPos.y = 0f;
				// Move position
				rb.MovePosition(rb.position + newPos * World.player.speed * Time.fixedDeltaTime);

				// Rotate player
				Quaternion oldRotate = rb.rotation;
				rb.rotation = Quaternion.Slerp(oldRotate, Quaternion.LookRotation(newPos.normalized), 0.5f);
				
				// Save data to player character
				World.player.X = playerGO.transform.position.x;
				World.player.Z = playerGO.transform.position.z;
				//Debug.Log(World.player.X + "," + World.player.Z);
			}
		}
	}

	private void PlayerNormalAttack() {

		//chosing random attack from array.
		//play the target animation in 0.1 second.
		normalAttack = true;
		playerAnim.CrossFade("Attack01",0.1f);
		//playerAnim.CrossFade(randomAttacks[Random.Range(0, randomAttacks.Length)], 0.1f); // Play Random attack animation
		pressedAttack = false;

		// Detect weapon box colider to enemy

		// Calculate damage to enemy
	}

	//void Start() {
	//	StartCoroutine("FindTargetsWithDelay", .2f);
	//}


	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void FindVisibleTargets() {
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(playerGO.transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - playerGO.transform.position).normalized;
			if (Vector3.Angle(playerGO.transform.forward, dirToTarget) < viewAngle / 2) {
				float dstToTarget = Vector3.Distance(playerGO.transform.position, target.position);

				if (!Physics.Raycast(playerGO.transform.position, dirToTarget, dstToTarget, obstacleMask)) {
					visibleTargets.Add(target);
				}
			}
		}
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += playerGO.transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}

	

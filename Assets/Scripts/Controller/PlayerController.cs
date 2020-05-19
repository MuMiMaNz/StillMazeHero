﻿using TouchControlsKit;
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
	private string[] randomAttacks = { "Attack01", "Attack02" };

	private float moveFT = 0.75f;
	private Quaternion playerRotateDT;

	public void SeekPlayerGO() {

		GameObject[] playerTag = GameObject.FindGameObjectsWithTag("Player");
		if (playerTag.Length != 1) {
			Debug.LogError("More than 1 Player tag GO");
		} else {
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
				rb.MovePosition(rb.position + newPos * moveFT * Time.fixedDeltaTime);

				// Rotate player
				Quaternion oldRotate = rb.rotation;
				rb.rotation = Quaternion.Slerp(oldRotate, Quaternion.LookRotation(newPos.normalized), 0.5f);
				
				// Save data to player character
				World.player.X = playerGO.transform.position.x;
				World.player.Z = playerGO.transform.position.z;
			}
		}
	}

	private void PlayerNormalAttack() {

		//chosing random attack from array.
		//play the target animation in 0.1 second.
		normalAttack = true;
		playerAnim.CrossFade(randomAttacks[Random.Range(0, randomAttacks.Length)], 0.1f);
		pressedAttack = false;

		// Detect weapon box colider to enemy

		// Calculate damage to enemy
	}
}
	

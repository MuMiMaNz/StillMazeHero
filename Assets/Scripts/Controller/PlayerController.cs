using TouchControlsKit;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	World World {
		get { return WorldController.Instance.World; }
	}
	public Vector3 playerMoveDT { get; protected set; }

	//[Range(0.01f,1f)]
	private float moveFT = 0.75f;

	private void Start() {
		playerMoveDT = Vector3.zero;
	}

	public void Update() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			
			Vector2 move = TCKInput.GetAxis("Joystick");
			//Debug.Log("Joystick moveX : " + move.x);
			//Debug.Log("Joystick moveXY : " + move.y);
			//World.player.X += move.x * moveFT * Time.fixedDeltaTime;
			//World.player.Z += move.y * moveFT * Time.fixedDeltaTime;

			playerMoveDT = new Vector3(move.x * moveFT, 0, move.y * moveFT);
			//Debug.Log("Move X :" + move.x);
			//Debug.Log("Move Y :" + move.y);
		}
	}

	private void FixedUpdate() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			GameObject c_go = GameObject.Find("Player");

			Rigidbody rb = c_go.GetComponent<Rigidbody>();
			// Move position
			rb.MovePosition(rb.position + playerMoveDT * Time.fixedDeltaTime);
			// Rotate player
			Vector2 move = TCKInput.GetAxis("Joystick");

			rb.rotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.y));


			World.player.X = c_go.transform.position.x;
			World.player.Z = c_go.transform.position.z;
		}
	}


}

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
	private Quaternion playerRotateDT;
	public GameObject playerGO { get; protected set; }
	private Rigidbody rb;
	//[Range(0.01f,1f)]
	private float moveFT = 0.75f;

	public void SeekPlayerGO() {
		
		playerGO = GameObject.Find("Player");
		rb = playerGO.GetComponent<Rigidbody>();
		playerRotateDT = rb.rotation;
		joyMoveDT = Vector3.zero;
		camTransform = camController.transform;
	}


	public void Update() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			
			Vector2 move = TCKInput.GetAxis("Joystick");

			joyMoveDT = new Vector3(move.x , 0, move.y );
			//Debug.Log(joyMoveDT.x);

			//camController.MoveCamOnPlayerMove(move);
		}
	}

	private void FixedUpdate() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
		
			if (joyMoveDT != Vector3.zero) {
				
				// Adjust jostict direction to camera angle
				Vector3 newPos = camTransform.rotation * joyMoveDT;
				newPos.y = 0f;
				//Vector3 newPos = Vector3.zero;
				//if (joyMoveDT.z > 0) newPos += camTransform.forward;
				//if (joyMoveDT.z == 0) newPos = camTransform.forward;
				//if (joyMoveDT.z < 0) newPos += -camTransform.forward;
				//if (joyMoveDT.x > 0) newPos += camTransform.right;
				//if (joyMoveDT.x == 0) newPos += camTransform.right;
				//if (joyMoveDT.x < 0) newPos += -camTransform.right;

				// Move position
				rb.MovePosition(rb.position + newPos * moveFT * Time.fixedDeltaTime);

				// Rotate player

				Quaternion oldRotate = rb.rotation;
				rb.rotation = Quaternion.Slerp(oldRotate, Quaternion.LookRotation(newPos.normalized), 0.5f);

				
				// Save data to player character
				World.player.X = playerGO.transform.position.x;
				World.player.Z = playerGO.transform.position.z;
			}
			

			//rb.rotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.y).normalized);

			
		}
	}


}

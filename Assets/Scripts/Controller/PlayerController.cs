using TouchControlsKit;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	World World {
		get { return WorldController.Instance.World; }
	}
	public Vector3 playerMoveDT;

	//[Range(0.01f,1f)]
	private float moveFT = 0.75f;
	public bool isHitWall ;

	private void Start() {
		playerMoveDT = Vector3.zero;
		isHitWall = false;
	}

	public void Update() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			
			Vector2 move = TCKInput.GetAxis("Joystick");
			//Debug.Log("Joystick moveX : " + move.x);
			//Debug.Log("Joystick moveXY : " + move.y);
			//World.player.X += move.x * moveFT * Time.fixedDeltaTime;
			//World.player.Z += move.y * moveFT * Time.fixedDeltaTime;

			playerMoveDT = new Vector3(move.x * moveFT, 0, move.y * moveFT);

		}
	}
	
	


}


//using UnityEngine;
//using TouchControlsKit;

//public class MovementController : MonoBehaviour {

//	World World {
//		get { return WorldController.Instance.World; }
//	}
//	public Vector3 playerMoveDT;

//	//[Range(0.01f,1f)]
//	private float moveFT = 0.75f;
	
//	public void Update() {
//		if (WorldController.Instance.gameMode == GameMode.PlayMode) {

//			// Move player data
//			Vector2 move = TCKInput.GetAxis("Joystick");
//			//Debug.Log("Joystick moveX : " + move.x);
//			//Debug.Log("Joystick moveXY : " + move.y);
//			//World.player.X += move.x * moveFT * Time.fixedDeltaTime;
//			//World.player.Z += move.y * moveFT * Time.fixedDeltaTime;

//			// Direction in Graphic update
//			playerMoveDT = new Vector3(move.x * moveFT, 0, move.y * moveFT);
//		}
//	}
//}


using UnityEngine;
using TouchControlsKit;

public class MovementController : MonoBehaviour
{

	World World {
		get { return WorldController.Instance.World; }
	}
	//[Range(0.01f,1f)]
	private float moveFT = 0.75f;
	// Move player data
	public void Update(float deltaTime) {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			Vector2 move = TCKInput.GetAxis("Joystick");
			//Debug.Log("Joystick moveX : " + move.x);
			//Debug.Log("Joystick moveXY : " + move.y);
			World.player.X +=   move.x * moveFT * deltaTime;
			World.player.Z +=   move.y * moveFT * deltaTime;
		}
	}
}

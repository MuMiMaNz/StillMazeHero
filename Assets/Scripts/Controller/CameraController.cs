using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	World World {
		get { return WorldController.Instance.World; }
	}
	
	void Start() {
		// Center the Camera
		this.transform.position = new Vector3(World.Width / 2, 8f, -0.5f);
	}
	
    public void LateUpdate() {
        if(WorldController.Instance.gameMode == GameMode.BuildMode) {
			this.transform.position = new Vector3(World.Width / 2, 8f, -0.5f);
		}else if (WorldController.Instance.gameMode == GameMode.PlayMode) {

			Vector3 newPos = new Vector3(World.player.X, 0, World.player.Z) + new Vector3(0, 0.5f, -1f);
			this.transform.position = Vector3.Slerp(this.transform.position, newPos, 0.8f);
			this.transform.rotation = Quaternion.Euler(38, 0, 0);
		}
    }
}

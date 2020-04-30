using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	World World {
		get { return WorldController.Instance.World; }
	}

	public PlayerController playerController;

	Rect topRight;

	Vector3 FirstPoint;
	Vector3 SecondPoint;
	float xAngle;
	float yAngle;
	float xAngleTemp;
	float yAngleTemp;
	Vector3 distanceCam;

	void Start() {
		topRight = new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2);

		xAngle = 0;
		yAngle = 0;

		SetBuildModeCam();
	}

	public void LateUpdate() {
		if (WorldController.Instance.gameMode == GameMode.BuildMode) {
			this.transform.position = new Vector3(World.Width / 2, 8f, -0.5f);
		} else if (WorldController.Instance.gameMode == GameMode.PlayMode) {

			// Camera follow player with distance
			//Vector3 camPos  = new Vector3(playerController.playerGO.transform.position.x, 0, playerController.playerGO.transform.position.z);


			Vector3 camPos = new Vector3(World.player.X, 0, World.player.Z) + distanceCam;
			this.transform.position = Vector3.Slerp(this.transform.position, camPos, 0.07f);
			//this.transform.rotation = Quaternion.Euler(38, 0, 0);

			//Swipe to rotate cam
			if (Input.touchCount > 0) {

				bool isTouchInRotateArea = false;
				foreach (Touch t in Input.touches) {
					 isTouchInRotateArea = topRight.Contains(t.position);
				}
				if (isTouchInRotateArea && playerController.joyMoveDT == Vector3.zero) {
					if (Input.GetTouch(0).phase == TouchPhase.Began) {
						FirstPoint = Input.GetTouch(0).position;
						xAngleTemp = xAngle;
						yAngleTemp = yAngle;
					}
					if (Input.GetTouch(0).phase == TouchPhase.Moved) {
						SecondPoint = Input.GetTouch(0).position;
						// Rotate Camera
						xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
						yAngle = yAngleTemp + (SecondPoint.y - FirstPoint.y) * 90 / Screen.height;
						this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);

						distanceCam = (-playerController.playerGO.transform.forward * 0.7f) + new Vector3(0, 0.5f, 0);

						//// Move Camera
						//Debug.Log((SecondPoint.x - FirstPoint.x) / Screen.width);
						//// X-axis drag from screen width : -1 to 1
						//float XDrag = (SecondPoint.x - FirstPoint.x) / Screen.width;
						//this.transform.position += new Vector3(XDrag,0, XDrag) *-2;
					}
				}
			}
		}
	}

	public void SetBuildModeCam() {
		Debug.Log("Set Build mode cam");
		xAngle = 0;
		yAngle = 70;
		
		this.transform.position = new Vector3(World.Width / 2, 8f, -0.5f);
		this.transform.rotation = Quaternion.Euler(70, 0, 0);
	}

	public void SetPlayModeCam() {
		Debug.Log("Set Play mode cam");
		xAngle = 0;
		yAngle = 38;

		//Vector3 newPos = new Vector3(World.player.X, 0, World.player.Z) + new Vector3(0, 0.5f, -1f);
		//this.transform.position = Vector3.Slerp(this.transform.position, newPos, 5f);
		distanceCam = new Vector3(0, 0.5f, -1f);
		this.transform.rotation = Quaternion.Euler(38, 0, 0);
	}
	
}

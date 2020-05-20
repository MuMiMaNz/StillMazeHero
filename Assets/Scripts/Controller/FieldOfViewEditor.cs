using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {

			// Draw Player's FOW
			PlayerController fow = (PlayerController)target;
			Handles.color = Color.white;
			Handles.DrawWireArc(fow.playerGO.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
			Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
			Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

			Handles.DrawLine(fow.playerGO.transform.position, fow.playerGO.transform.position + viewAngleA * fow.viewRadius);
			Handles.DrawLine(fow.playerGO.transform.position, fow.playerGO.transform.position + viewAngleB * fow.viewRadius);

			Handles.color = Color.red;
			foreach (Transform visibleTarget in fow.visibleTargets) {
				Handles.DrawLine(fow.playerGO.transform.position, visibleTarget.position);
			}
		}
	}

}
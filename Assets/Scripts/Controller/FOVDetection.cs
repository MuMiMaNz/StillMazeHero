using UnityEngine;

//[CustomEditor(typeof(PlayerController))]
//public class FieldOfViewPlayerEditor : Editor {

//	void OnSceneGUI() {
//		if (WorldController.Instance.gameMode == GameMode.PlayMode) {

//			// Draw Player's FOW
//			PlayerController fow = (PlayerController)target;
//			Handles.color = Color.white;
//			Handles.DrawWireArc(fow.playerGO.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
//			Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
//			Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

//			Handles.DrawLine(fow.playerGO.transform.position, fow.playerGO.transform.position + viewAngleA * fow.viewRadius);
//			Handles.DrawLine(fow.playerGO.transform.position, fow.playerGO.transform.position + viewAngleB * fow.viewRadius);

//			Handles.color = Color.red;
//			foreach (Transform visibleTarget in fow.visibleTargets) {
//				Handles.DrawLine(fow.playerGO.transform.position, visibleTarget.position);
//			}
//		}
//	}
//}

//[CustomEditor(typeof(CharacterGraphicController))]
//public class FieldOfViewMinionEditor : Editor {

//	void OnSceneGUI() {
//		if (WorldController.Instance.gameMode == GameMode.PlayMode) {

//			// Draw Minion's FOW
//			CharacterGraphicController cgc = (CharacterGraphicController)target;
//			foreach (Minion m in WorldController.Instance.World.minions) {

//				GameObject mn_go = cgc.minionGameObjectMap[m];

//				Handles.color = Color.white;
//				Handles.DrawWireArc(mn_go.transform.position, Vector3.up, Vector3.forward, 360, cgc.viewRadius);

//				Vector3 viewAngleA = cgc.DirFromAngle(-cgc.viewAngle / 2);
//				Vector3 viewAngleB = cgc.DirFromAngle(cgc.viewAngle / 2);

//				Handles.DrawLine(mn_go.transform.position, mn_go.transform.position + viewAngleA * cgc.viewRadius);
//				Handles.DrawLine(mn_go.transform.position, mn_go.transform.position + viewAngleB * cgc.viewRadius);

//				Handles.color = Color.red;

//				if (cgc.playerTarget != null)
//					Handles.DrawLine(mn_go.transform.position, cgc.playerTarget.position);

//				//foreach (Transform visibleTarget in cgc.visibleTargets) {
//				//	Handles.DrawLine(cgc.playerGO.transform.position, visibleTarget.position);
//				//}
//			}
//		}
//	}
//}

public class FOVDetection : MonoBehaviour {

	public CharacterGraphicController cgc;

	private Transform player;
	private float maxAngle;
	private float maxRadius;

	private bool isInFov = false;

	private void OnDrawGizmos() {
		if (WorldController.Instance.gameMode == GameMode.PlayMode) {
			maxRadius = cgc.viewRadius;
			maxAngle = cgc.viewAngle;
			player = GameObject.Find("Player").transform;

			foreach (Minion m in WorldController.Instance.World.minions) {

				GameObject mn_go = cgc.minionGameObjectMap[m];

				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(mn_go.transform.position, maxRadius);

				Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, mn_go.transform.up) * mn_go.transform.forward * maxRadius;
				Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, mn_go.transform.up) * mn_go.transform.forward * maxRadius;

				Gizmos.color = Color.blue;
				Gizmos.DrawRay(mn_go.transform.position, fovLine1);
				Gizmos.DrawRay(mn_go.transform.position, fovLine2);

				if (m.seePlayer == false)
					Gizmos.color = Color.red;
				else
					Gizmos.color = Color.green;
				Gizmos.DrawRay(mn_go.transform.position, (player.position - mn_go.transform.position).normalized * maxRadius);

				Gizmos.color = Color.black;
				//Debug.Log(mn_go.transform.forward);
				Gizmos.DrawRay(mn_go.transform.position, mn_go.transform.forward * maxRadius);
			}
		}
	}

	//public static bool inFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius) {

	//	Collider[] overlaps = new Collider[10];
	//	int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps);

	//	for (int i = 0; i < count + 1; i++) {

	//		if (overlaps[i] != null) {

	//			if (overlaps[i].transform == target) {

	//				Vector3 directionBetween = (target.position - checkingObject.position).normalized;
	//				directionBetween.y *= 0;

	//				float angle = Vector3.Angle(checkingObject.forward, directionBetween);

	//				if (angle <= maxAngle) {

	//					Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
	//					RaycastHit hit;

	//					if (Physics.Raycast(ray, out hit, maxRadius)) {

	//						if (hit.transform == target)
	//							return true;
	//					}
	//				}
	//			}
	//		}
	//	}

	//	return false;
	//}


	//private void Update() {
	//	if (WorldController.Instance.gameMode == GameMode.PlayMode) {
	//		player = GameObject.Find("Player").transform;
	//		foreach (Minion m in WorldController.Instance.World.minions) {

	//			GameObject mn_go = cgc.minionGameObjectMap[m];
	//			isInFov = inFOV(mn_go.transform, player, maxAngle, maxRadius);
	//		}
	//	}
	//}

}
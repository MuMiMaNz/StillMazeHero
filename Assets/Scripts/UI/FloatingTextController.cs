
using TouchControlsKit;
using UnityEngine;

public class FloatingTextController : MonoBehaviour{

	private static FloatingText floatText;
	private static GameObject canvas;

	public void Start() {
		canvas = GameObject.Find("PlayModeCanvas");
		floatText = Resources.Load<FloatingText>("Prefabs/UI/FloatingDMG");
		
	}

	public static void CreateFloatingDMG(float dmg, Transform location) {

		//if (dmg == 0f)
		//	return;

		FloatingText instance = Instantiate(floatText);
		Vector2 screenPostition = Camera.main.WorldToScreenPoint(
			location.position);
		//new Vector2(location.position.x + Random.Range(-.5f, .5f),
		//location.position.y + Random.Range(-.5f, .5f)));

		//Debug.Log("Screen Position : " + screenPostition.x + "," + screenPostition.y);
		Vector2 stupidPrivotCanvasPos = screenPostition - new Vector2(Screen.width / 2, Screen.height / 2);

		instance.transform.position = stupidPrivotCanvasPos;
		instance.transform.SetParent(canvas.transform, false);
		
		instance.SetText(dmg.ToString("F0"));
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCanvasController : MonoBehaviour
{
	private static HealthBar healthBar;
	private static GameObject CharCanvas;

	public void OnEnable() {
		
		CharCanvas = Resources.Load<GameObject>("Prefabs/UI/CharCanvas");
		healthBar = CharCanvas.GetComponentInChildren<HealthBar>();
		
	}

	public static GameObject CreateHealthBar(Transform parent) {

		GameObject instance = Instantiate(CharCanvas);

		//Vector2 screenPostition = Camera.main.WorldToScreenPoint(
		//	location.position);

		//Vector2 stupidPrivotCanvasPos = screenPostition - new Vector2(Screen.width / 2, Screen.height / 2);

		instance.name = "CharCanvas";
		instance.transform.position = Vector3.zero;
		instance.transform.SetParent(parent, false);

		return instance;
	}
}

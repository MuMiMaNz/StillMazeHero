
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	private Image barImage;
	private Text barText;

	private void Awake() {
		barImage = GetComponent<Image>();
		barText = GetComponentInChildren<Text>();
	}

	public void HealthBarChange(float HP, float MaxHP) {
		if(HP < 0f) { HP = 0f; }
		barImage.fillAmount = HP/MaxHP;
		barText.text = HP.ToString("F0") + " / " + MaxHP.ToString("F0");
	}

}

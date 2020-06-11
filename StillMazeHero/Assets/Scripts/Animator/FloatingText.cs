
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
	// This Script attach to 
	//private Animator animatorText;
	private Text floatText;

    void OnEnable()
    {
		Animator animatorText = GetComponentInChildren<Animator>();
		AnimatorClipInfo[] clipInfo = animatorText.GetCurrentAnimatorClipInfo(0);
		Destroy(gameObject, clipInfo[0].clip.length);
		floatText = animatorText.GetComponent<Text>();
		//Debug.Log(floatText);
    }

	public void SetText(string text) {
		floatText.text = text;
	}

}

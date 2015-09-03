using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FightScreenChickenStatAutoAdjust : MonoBehaviour {
	
	private Text text;
	private Slider slider;

	public void Awake() {
		text = GetComponentInChildren<Text>();
		slider = GetComponent<Slider>();
		OnChange();
	}

	public void OnChange() {
		if(text) {
			text.text = slider.value + " / " + slider.maxValue;
		}
	}
}

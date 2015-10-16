using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderText : MonoBehaviour {

	public Text text;
	private Slider slider;
	
	public void Awake() {
		slider = GetComponent<Slider>();
		OnChange();
	}
	
	public virtual void OnChange() {
		if(text && slider) {
			text.text = "" + slider.value;
		}
	}
}

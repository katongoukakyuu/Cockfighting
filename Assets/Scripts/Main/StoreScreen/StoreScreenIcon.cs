using UnityEngine;
using System.Collections;

public class StoreScreenIcon : MonoBehaviour {

	public void ButtonPressed() {
		StoreManager.Instance.SetSelectedItem(this.name);
	}
}

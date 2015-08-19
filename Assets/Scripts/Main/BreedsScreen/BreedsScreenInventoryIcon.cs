using UnityEngine;
using System.Collections;

public class BreedsScreenInventoryIcon : MonoBehaviour {

	public void ButtonPressed() {
		FeedsManager.Instance.SetSelectedItem(this.name);
	}
}

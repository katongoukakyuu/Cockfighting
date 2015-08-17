using UnityEngine;
using System.Collections;

public class FeedsScreenInventoryIcon : MonoBehaviour {

	public void ButtonPressed() {
		FeedsManager.Instance.SetSelectedItem(this.name);
	}
}

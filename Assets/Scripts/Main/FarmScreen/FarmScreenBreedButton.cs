using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenBreedButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas breedCanvas;
	
	public void ButtonPressed() {
		mainCanvas.gameObject.SetActive (false);
		breedCanvas.gameObject.SetActive (true);
		Camera.main.GetComponent<GridOverlay>().ToggleCanHoverOnMap(false);
		BreedsManager.Instance.Initialize ();
	}
	
}


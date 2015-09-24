using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenBuildButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas buildStructuresCanvas;

	public Animator mainCanvasAnimation;

	public BuildScreenImagePanel imagePanel;

	public void ButtonPressed() {
		mainCanvasAnimation.SetBool("isHidden", true);
		Invoke ("BuildFunctions", 0.2f);

	}

	void BuildFunctions()
	{
		imagePanel.SetBuildings (DatabaseManager.Instance.LoadBuildings());
		
		mainCanvas.gameObject.SetActive (false);
		buildStructuresCanvas.gameObject.SetActive (true);
		Camera.main.GetComponent<GridOverlay>().ToggleCanHoverOnMap(false);
	}
	
}


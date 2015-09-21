using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenMailboxButton : MonoBehaviour {
	
	public Canvas mainCanvas;
	public Canvas mailboxCanvas;
	
	public void ButtonPressed() {
		mainCanvas.gameObject.SetActive (false);
		mailboxCanvas.gameObject.SetActive (true);
		Camera.main.GetComponent<GridOverlay>().ToggleCanHoverOnMap(false);
		MailboxManager.Instance.Initialize ();
	}
	
}


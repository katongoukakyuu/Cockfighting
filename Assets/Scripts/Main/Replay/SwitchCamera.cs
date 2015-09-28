using UnityEngine;
using System.Collections;

public class SwitchCamera : MonoBehaviour {

	public GameObject[] cameras;

	int i = 0;

	void Start() {
		if(cameras.Length == 0)	return;

		if(cameras[0].GetComponent<CameraTekken>() != null) {
			StartCoroutine(AttemptInitializeTekken());
		}
	}

	// Update is called once per frame
	void Update () {
		if(cameras.Length == 0)	return;

		if(Input.GetKeyDown(KeyCode.Space) || 
		   (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ) {
			i = (i+1) % cameras.Length;
			for(int x = 0; x < cameras.Length; x++) {
				cameras[x].GetComponent<Camera>().enabled = false;
				cameras[x].GetComponent<AudioListener>().enabled = false;
				if(cameras[x].GetComponent<CameraTekken>() != null) {
					cameras[x].GetComponent<CameraTekken>().enabled = false;
				}
			}
			
			cameras[i].GetComponent<Camera>().enabled = true;
			cameras[i].GetComponent<AudioListener>().enabled = true;
			if(cameras[i].GetComponent<CameraTekken>() != null) {
				GameObject[] chickens = ReplayManager.Instance.GetChickens();
				if(chickens.Length == 2) {
					cameras[i].GetComponent<CameraTekken>().enabled = true;
					cameras[i].GetComponent<CameraTekken>().Initialize(chickens[0],chickens[1]);
				}
			}
		}
	}

	IEnumerator AttemptInitializeTekken() {
		GameObject[] chickens = new GameObject[2];
		do {
			yield return new WaitForSeconds(0.1f);
			chickens = ReplayManager.Instance.GetChickens();
			if(chickens[0] != null && chickens[1] != null) {
				cameras[i].GetComponent<CameraTekken>().enabled = true;
				cameras[i].GetComponent<CameraTekken>().Initialize(chickens[0],chickens[1]);
				yield break;
			}
		} while(chickens[0] == null || chickens[1] == null);
	}
}

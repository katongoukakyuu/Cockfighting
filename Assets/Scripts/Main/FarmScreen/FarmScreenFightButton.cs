using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using System.IO;

public class FarmScreenFightButton : MonoBehaviour {

	public Canvas mainCanvas;
	public Canvas fightCanvas;

	public CanvasGroup mainCanvasGroup;
	public CanvasGroup FightCanvasGroup;

	public float fadeTimeOne = 0.5f ;


	void Update()
	{
		if ( mainCanvasGroup.alpha == 0)
		{

			mainCanvas.gameObject.SetActive (false);
			mainCanvasGroup.alpha = 1;
		}
	}
	public void ButtonPressed() {

		StartCoroutine(DoFadeOutOne());

		StartCoroutine(DoFadeInTwo());

		fightCanvas.gameObject.SetActive (true);
		FightManager.Instance.Initialize ();
	}

	IEnumerator DoFadeOutOne(){
		//		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		while (mainCanvasGroup.alpha > 0){
			mainCanvasGroup.alpha -= Time.deltaTime / fadeTimeOne;
			yield return null;
		}
		yield return null;
	}


	IEnumerator DoFadeInTwo(){
		while (FightCanvasGroup.alpha < 1){
			FightCanvasGroup.alpha += Time.deltaTime / fadeTimeOne;
			yield return null;
		}
		yield return null;
	}
}


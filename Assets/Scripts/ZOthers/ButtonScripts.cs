 using UnityEngine;
using System.Collections;

public class ButtonScripts : MonoBehaviour {

	public Animator Market;
	public Animator Arena;

	public GameObject FarmGUI;
	public GameObject BlackOverLay;
	


	public void openMarketPanel()
	{
		Market.enabled = true;
		Market.SetBool ("isHidden", false);
		FarmGUI.SetActive(false);
		BlackOverLay.SetActive(true);

	}

	public void closemMarketPanel()
	{
		Market.SetBool ("isHidden", true);
		FarmGUI.SetActive (true);
		BlackOverLay.SetActive (false);

	}

	public void openArenaPanel()
	{
		Arena.enabled = true;
		Arena.SetBool ("isHidden",false);
		FarmGUI.SetActive(false);
		BlackOverLay.SetActive (true);
	}

	public void closeArenaPanel()
	{
		Arena.SetBool("isHidden", true);
		FarmGUI.SetActive (true);
		BlackOverLay.SetActive(false);
	}

	public void toFarm()
	{
		Application.LoadLevel ("graphic test");
	}

	public void toArena()
	{
		Application.LoadLevel("fight");
	}

	public void toUntextured()
	{
		Application.LoadLevel("untextured assets");
	}

	public void toControlPanel()
	{
		Application.LoadLevel("Control Panel");
	}

	public void toFight()
	{
		Application.LoadLevel("Fight Ring");
	}

}

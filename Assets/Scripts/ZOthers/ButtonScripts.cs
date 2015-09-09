 using UnityEngine;
using System.Collections;

public class ButtonScripts : MonoBehaviour {

	public Animator Market;
	public Animator Arena;

	public GameObject FarmGUI;
	public GameObject BlackOverLay;

	public GameObject ModeSection;
	public GameObject FightScreen;

	public GameObject camera;


	void Open()
	{
		print ("Open!");
		FarmGUI.SetActive(false);
		BlackOverLay.SetActive(true);
		((MonoBehaviour)camera.GetComponent("CameraControls")).enabled = false;
	}

	void Close()
	{
		FarmGUI.SetActive (true);
		BlackOverLay.SetActive (false);
		((MonoBehaviour)camera.GetComponent("CameraControls")).enabled = true;
	}

	public void openMarketPanel()
	{
		Market.enabled = true;
		Market.SetBool ("isHidden", false);
		Open ();
	}

	public void closemMarketPanel()
	{
		Market.SetBool ("isHidden", true);
		Close ();
	}

	public void openArenaPanel()
	{
		Arena.enabled = true;
		Arena.SetBool ("isHidden",false);
		Open ();
	}

	public void closeArenaPanel()
	{
		Arena.SetBool("isHidden", true);
		ModeSection.SetActive (true);
		FightScreen.SetActive(false);
		Close();
	}

	public void OpenFight()
	{
		ModeSection.SetActive (false);
		FightScreen.SetActive(true);
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

	public void toProgFarm()
	{
		Application.LoadLevel ("Farm");
	}
}

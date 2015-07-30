using UnityEngine;
using System.Collections;

public class ButtonScripts : MonoBehaviour {

	public void toMarket()
	{
		Application.LoadLevel ("products graphic test");
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

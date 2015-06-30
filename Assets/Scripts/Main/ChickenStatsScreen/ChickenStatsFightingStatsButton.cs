using UnityEngine;
using System.Collections;

public class ChickenStatsFightingStatsButton : MonoBehaviour {

	public GameObject fightingStatsPanel;

	public void ButtonPressed() {
		fightingStatsPanel.SetActive (!fightingStatsPanel.activeSelf);
	}
}

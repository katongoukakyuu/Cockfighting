using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightScreenViewMatch : MonoBehaviour {

	private IDictionary<string,object> match;
	
	public void ButtonPressed() {
		MatchViewManager.Instance.Initialize(match);
	}

	public void SetMatch(IDictionary<string,object> i) {
		match = i;
	}
}

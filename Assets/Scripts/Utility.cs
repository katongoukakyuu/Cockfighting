using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utility : MonoBehaviour {

	public static void PrintDictionary(IDictionary<string,object> id) {
		foreach(KeyValuePair<string,object> kv in id) {
			print(kv.Key.ToString() + ": " + kv.Value.ToString());
		}
	}

	public static int GetPayout(int betAmount, IDictionary<string,object> bettingOdds, bool isLlamado) {
		if(isLlamado) {
			return (int) betAmount * 2 * int.Parse (bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString()) / int.Parse (bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString());
		}
		else {
			return (int) betAmount * 2 * int.Parse (bettingOdds[Constants.DB_KEYWORD_LLAMADO_ODDS].ToString()) / int.Parse (bettingOdds[Constants.DB_KEYWORD_DEHADO_ODDS].ToString());
		}
	}
}

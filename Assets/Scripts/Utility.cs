using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utility : MonoBehaviour {

	public static void PrintDictionary(IDictionary<string,object> id) {
		foreach(KeyValuePair<string,object> kv in id) {
			print(kv.Key.ToString() + ": " + kv.Value.ToString());
		}
	}
}

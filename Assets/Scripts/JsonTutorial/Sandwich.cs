using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sandwich {
	public string name;
	public string bread;
	public float price;
	public List<string> ingredients = new List<string>();
}

[System.Serializable]
public class Sandwiches {
	public List<Sandwich> sandwiches = new List<Sandwich>();
}

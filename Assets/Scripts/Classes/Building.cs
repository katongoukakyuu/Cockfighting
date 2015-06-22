using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Building {

	public int id;
	public string name;

	public int xSize;
	public int ySize;
	public int xCenter;
	public int yCenter;

	public string prefabName;
	public string imageName;
}

[System.Serializable]
public class Buildings {
	public List<Building> b = new List<Building>();
}

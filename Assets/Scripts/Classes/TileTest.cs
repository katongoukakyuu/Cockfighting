using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TileTest {

	public int x;
	public int z;
	public bool isFilled;

}

[System.Serializable]
public class TileGroup {

	public Tile[,] tiles;
	public int xStart;
	public int zStart;
	public int xEnd;
	public int zEnd;

}

[System.Serializable]
public class PlayerTiles {

	public Tile[,] tiles;
	public List<TileGroup> tileGroups = new List<TileGroup>();
	public int xSize;
	public int zSize;

}
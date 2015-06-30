using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

	public GameObject tile;
	public int xSize = 10;
	public int zSize = 10;
	public float tileSize = 1f;

	public GameObject[,] tiles;

	private static TileMap instance;
	private TileMap() {}
	
	public static TileMap Instance {
		get {
			if(instance == null) {
				instance = (TileMap)GameObject.FindObjectOfType(typeof(TileMap));
			}
			return instance;
		}
	}

	void Start() {
		tiles = new GameObject[xSize,zSize];
		GameObject board = new GameObject();
		board.name = "Board";
		for(int x = 0; x < xSize; x++) {
			for(int z = 0; z < zSize; z++) {
				GameObject g = (GameObject)Instantiate (tile, new Vector3(x*tileSize, 0, z*tileSize), Quaternion.identity);
				g.name = "Tile " + x + " " + z;
				g.transform.parent = board.transform;
				g.AddComponent<Tile>();
				tiles[x,z] = g;
			}
		}
	}

}

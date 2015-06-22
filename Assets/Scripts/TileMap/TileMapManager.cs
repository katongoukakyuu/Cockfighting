using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(TileMap))]
[RequireComponent (typeof(TileMapMouse))]
public class TileMapManager : MonoBehaviour {

	private TileMap tileMap;
	private TileMapMouse tileMapMouse;
	private static TileMapManager instance;

	private TileMapManager() {}

	public static TileMapManager Instance {
		get {
			if(instance == null) {
				instance = (TileMapManager)GameObject.FindObjectOfType(typeof(TileMapManager));
			}
			return instance;
		}
	}

	void Awake () {
		tileMap = GetComponent<TileMap> ();
		tileMapMouse = GetComponent<TileMapMouse> ();
	}

	public GameObject[,] GetTiles() {
		return Instance.tileMap.tiles;
	}

	public Vector3 GetPosition() {
		return Instance.tileMapMouse.position;
	}
}

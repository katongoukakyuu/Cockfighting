using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(TileMap))]
[RequireComponent (typeof(TileMapMouse))]
public class TileMapManager : MonoBehaviour {

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
		tileMapMouse = GetComponent<TileMapMouse> ();
	}

	public GameObject[,] GetTiles() {
		return TileMap.Instance.tiles;
	}

	public Vector3 GetPosition() {
		return tileMapMouse.position;
	}
}

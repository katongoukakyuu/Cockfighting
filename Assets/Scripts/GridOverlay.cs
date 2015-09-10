﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridOverlay : MonoBehaviour {
	
	public GameObject plane;
	public GameObject tile;
	
	public bool showMain = true;
	public bool showSub = false;
	
	public int gridSizeX;
	public int gridSizeY;
	public int gridSizeZ;
	
	public float smallStep;
	public float largeStep;
	
	public float startX;
	public float startY;
	public float startZ;
	
	private float offsetY = 0;
	private float scrollRate = 0.1f;
	private float lastScroll = 0f;

	private GameObject[,] tiles;
	private float tileOriginalScale = 0.1f;
	private Vector2 position = Vector2.zero;
	
	private Material lineMaterial;
	private IDictionary<Color, Material> materialsByColor = new Dictionary<Color, Material>();
	
	public Color mainColor = new Color(0f,1f,0f,1f);
	public Color subColor = new Color(0f,0.5f,0f,1f);

	public Material[] selectedTileColors;
	public Material[] deselectedTileColors;
	
	void Start () 
	{
		int xSize = (int) (gridSizeX / largeStep);
		int zSize = (int) (gridSizeZ / largeStep);
		tiles = new GameObject[xSize, zSize];
		GameObject board = new GameObject();
		board.name = "Board";
		for(int x = 0; x < xSize; x++) {
			for(int z = 0; z < zSize; z++) {
				GameObject g = (GameObject)Instantiate (tile);
				g.transform.localScale = new Vector3(tileOriginalScale * largeStep,
				                                     tileOriginalScale * largeStep,
				                                     tileOriginalScale * largeStep);
				g.transform.position = new Vector3((x * largeStep) + startX + (largeStep/2), 
				                                   startY, 
				                                   (z * largeStep) + startZ + (largeStep/2));
				g.name = "Tile " + x + " " + z;
				g.transform.parent = board.transform;
				g.AddComponent<Tile>();
				g.GetComponent<Tile>().position = new Vector2(x, z);
				if(z < (int)(zSize/2)) {
					g.GetComponent<Tile>().matSelected = selectedTileColors[0];
					g.GetComponent<Tile>().matDeselected = deselectedTileColors[0];
				}
				else {
					g.GetComponent<Tile>().matSelected = selectedTileColors[1];
					g.GetComponent<Tile>().matDeselected = deselectedTileColors[1];
				}
				tiles[x,z] = g;
			}
		}
	}
	
	void Update () 
	{
		if(lastScroll + scrollRate < Time.time)
		{
			if(Input.GetKey(KeyCode.KeypadPlus)) 
			{
				print ("keypad plus");
				plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y + smallStep, plane.transform.position.z);
				offsetY += smallStep;
				lastScroll = Time.time;
			}
			if(Input.GetKey(KeyCode.KeypadMinus))
			{
				print ("keypad minus");
				plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y - smallStep, plane.transform.position.z);
				offsetY -= smallStep;
				lastScroll = Time.time;
			}
		}

		Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit;
		if (Physics.Raycast(screenRay, out hit))
		{
			if(hit.collider.gameObject.tag == "Map") {
				Vector2 newPos;
				if(hit.collider.gameObject.GetComponent<Tile>() != null) {
					newPos = hit.collider.gameObject.GetComponent<Tile>().position;
				}
				else {
					print ("Tile component is null! Returning world position");
					Vector3 pos = hit.collider.gameObject.transform.position;
					newPos = new Vector2(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
				}

				if(position != newPos) {
					position = newPos;
					//OnTileHoverChange();
					Debug.Log ("map tile is " + position);
				}
			}
		}
		
		if(Input.GetMouseButton(0)) {
			//Debug.Log ("clicked on " + TileMapManager.Instance.GetPosition());
		}
	}
	
	private Material GetLineMaterial(Color color)
	{
		Material lineMaterial;
		if( !materialsByColor.TryGetValue(color, out lineMaterial) ) 
		{
			lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
			                            " Properties { _Color (\"Main Color\", Color) = ("+color.r+","+color.g+","+color.b+","+color.a+") } " +
			                            " SubShader { Pass { " +
			                            " Blend SrcAlpha OneMinusSrcAlpha " +
			                            " ZWrite Off Cull Off Fog { Mode Off } " +
			                            " Color[_Color] " +
			                            " BindChannels {" +
			                            " Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			
			materialsByColor.Add(color, lineMaterial);
		}
		return lineMaterial;
	}

	
	void OnPostRender() 
	{        
		GL.Begin( GL.LINES );
		
		if(showSub)
		{
			if(Application.platform != RuntimePlatform.Android) {
				GL.Color(subColor);
			}
			lineMaterial = GetLineMaterial(subColor);
			lineMaterial.SetPass( 0 );

			//Layers
			for(float j = 0; j <= gridSizeY; j += smallStep)
			{
				//X axis lines
				for(float i = 0; i <= gridSizeZ; i += smallStep)
				{
					GL.Vertex3( startX, j + offsetY, startZ + i);
					GL.Vertex3( startX + gridSizeX, j + offsetY, startZ + i);
				}
				
				//Z axis lines
				for(float i = 0; i <= gridSizeX; i += smallStep)
				{
					GL.Vertex3( startX + i, j + offsetY, startZ);
					GL.Vertex3( startX + i, j + offsetY, startZ + gridSizeZ);
				}
			}
			
			//Y axis lines
			for(float i = 0; i <= gridSizeZ; i += smallStep)
			{
				for(float k = 0; k <= gridSizeX; k += smallStep)
				{
					GL.Vertex3( startX + k, startY + offsetY, startZ + i);
					GL.Vertex3( startX + k, gridSizeY + offsetY, startZ + i);
				}
			}
		}
		
		if(showMain)
		{
			if(Application.platform != RuntimePlatform.Android) {
				GL.Color(mainColor);
			}
			lineMaterial = GetLineMaterial(mainColor);
			lineMaterial.SetPass( 0 );

			//Layers
			for(float j = 0; j <= gridSizeY; j += largeStep)
			{
				//X axis lines
				for(float i = 0; i <= gridSizeZ; i += largeStep)
				{
					GL.Vertex3( startX, j + offsetY, startZ + i);
					GL.Vertex3( startX + gridSizeX, j + offsetY, startZ + i);
				}
				
				//Z axis lines
				for(float i = 0; i <= gridSizeX; i += largeStep)
				{
					GL.Vertex3( startX + i, j + offsetY, startZ);
					GL.Vertex3( startX + i, j + offsetY, startZ + gridSizeZ);
				}
			}
			
			//Y axis lines
			for(float i = 0; i <= gridSizeZ; i += largeStep)
			{
				for(float k = 0; k <= gridSizeX; k += largeStep)
				{
					GL.Vertex3( startX + k, startY + offsetY, startZ + i);
					GL.Vertex3( startX + k, gridSizeY + offsetY, startZ + i);
				}
			}
		}
		
		
		GL.End();
	}
}
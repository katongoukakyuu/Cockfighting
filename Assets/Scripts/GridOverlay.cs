using UnityEngine;
using UnityEngine.EventSystems;
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
	private int xSize, zSize;
	private float tileOriginalScale = 0.1f;
	private Vector2 position = Vector2.zero;
	
	private Material lineMaterial;
	private IDictionary<Color, Material> materialsByColor = new Dictionary<Color, Material>();
	
	public Color mainColor = new Color(0f,1f,0f,1f);
	public Color subColor = new Color(0f,0.5f,0f,1f);

	public bool tilesSelectable = false;
	public Material[] selectedTileColors;
	public Material[] deselectedTileColors;

	public delegate void OnTileHoverChangeEvent();
	public event OnTileHoverChangeEvent OnTileHoverChange;

	private bool canHoverOnMap = false;
	private bool canClickOnMap = false;
	private bool canRenderLines = false;
	private Tile selectedTile;
	
	void Awake () 
	{
		xSize = (int) (gridSizeX / largeStep);
		zSize = (int) (gridSizeZ / largeStep);
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
				if(tilesSelectable && selectedTileColors.Length > 0 && deselectedTileColors.Length > 0) {
					if(x < (int)(xSize/2)) {
						g.GetComponent<Tile>().matSelected = selectedTileColors[0];
						g.GetComponent<Tile>().matDeselected = deselectedTileColors[0];
					}
					else {
						g.GetComponent<Tile>().matSelected = selectedTileColors[1];
						g.GetComponent<Tile>().matDeselected = deselectedTileColors[1];
					}
				}
				else {
					// Set material alpha to 0 and render it in Fade mode
					/*Material m = g.GetComponent<Renderer>().material;
					Color c = m.color;
					c.a = 0;
					m.color = c;
					m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					m.SetInt("_ZWrite", 0);
					m.DisableKeyword("_ALPHATEST_ON");
					m.EnableKeyword("_ALPHABLEND_ON");
					m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					m.renderQueue = 3000;*/
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
				// print ("keypad plus");
				plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y + smallStep, plane.transform.position.z);
				offsetY += smallStep;
				lastScroll = Time.time;
			}
			if(Input.GetKey(KeyCode.KeypadMinus))
			{
				// print ("keypad minus");
				plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y - smallStep, plane.transform.position.z);
				offsetY -= smallStep;
				lastScroll = Time.time;
			}
		}

		if(canHoverOnMap) {
			UpdateSelectedTile();
		}

		if(canClickOnMap) {
			if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
				if(!EventSystem.current.IsPointerOverGameObject()) {
					UpdateSelectedTile();
				}
			}
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
		if(!canRenderLines)	return;

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

	void UpdateSelectedTile() {
		Ray screenRay;
		if(Input.touchCount > 0) {
			screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch (0).position.x,
			                                                     Input.GetTouch (0).position.y,
			                                                     0));
		}
		else {
			screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		}
		
		RaycastHit hit;
		if (Physics.Raycast(screenRay, out hit))
		{
			if(hit.collider.gameObject.tag == "Map") {
				Vector2 newPos;
				if(hit.collider.gameObject.GetComponent<Tile>() != null) {
					newPos = hit.collider.gameObject.GetComponent<Tile>().position;
					selectedTile = hit.collider.gameObject.GetComponent<Tile>();
				}
				else {
					// print ("Tile component is null! Returning world position");
					Vector3 pos = hit.collider.gameObject.transform.position;
					newPos = new Vector2(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
				}

				if(position != newPos) {
					position = newPos;
					if(OnTileHoverChange != null) {
						OnTileHoverChange();
					}
					// print ("map tile is " + position);
				}
			}
		}
	}

	public Vector2[] GetBoardCorners() {
		Vector2[] boardCorners = new Vector2[2];
		boardCorners [0] = new Vector2 (tiles [0, 0].transform.position.x, tiles [0, 0].transform.position.z);
		boardCorners [1] = new Vector2 (tiles [xSize-1, zSize-1].transform.position.x, tiles [xSize-1, zSize-1].transform.position.z);
		return boardCorners;
	}

	public GameObject[,] GetTiles() {
		return tiles;
	}

	public void ToggleCanHoverOnMap(bool b) {
		this.canHoverOnMap = b;
	}

	public void ToggleCanClickOnMap(bool b) {
		this.canClickOnMap = b;
	}

	public void ToggleCanRenderLines(bool b) {
		this.canRenderLines = b;
	}

	public Tile GetSelectedTile() {
		return selectedTile;
	}
}
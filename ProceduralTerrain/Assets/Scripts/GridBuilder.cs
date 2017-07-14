using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Valve.VR;

public class GridBuilder : MonoBehaviour {

	public int TerrainArea;
	public int ChunksSize;
	public Texture2D BaseImage;
	public Texture2D HighResImage;
	public GameObject player;
	public Vector2 positionchange; 
	public int smallestResolution;
	public int outlength;
	public Texture2D testnormalmap;
	public Material terrainmaterialtest;

	public int traillength;

	public GameObject VRCamera;

	//calculated after running tests that say a resolution of about 2.3 pixels per unit of chunksize is the max unity can handle
	private int maxresolution;
	private int ChunkstoFillGrid;
	//calculated knowing unity can handle roughly 1297 chunks at the max resolution before hitting 60FPS
	//stores the images at the higher resolution that will be in scene and their position
	private Texture2D[] ChunkHeightmapsHighRes;
	private Texture2D[] ChunkHeightsmapsLowRes;
	private GameObject[] Terrains;
	//a dictionary for the grid which stores the xy coordinates of the image for that chunk
	private int[,] Grid;

	private float[][,] heightmapslow;
	private float[][,] heightmapshigh;

	private bool instantiated = false;
	private Vector2 prevpos; 

	// Use this for initialization
	void Start () {
		prevpos = new Vector2 (-1, -1); 
		//currenlty baseimage must not be larger than 1024 or memory will overload
		//TerrainArea = BaseImage.width;
		//if (TerrainArea > 1024) {
			//Debug.Log ("Your resolution is too high to load the entire starting area, would have to segment original lowRes image. Coming in future."); 
			//return; 
		//}
		SetupTerrainChunks ();
	}
	
	// Update is called once per frame
	void Update () {
		//handle the changing to highres here
		if (instantiated == true) {

			//only updates on change
			if (prevpos.x != player.transform.position.x || prevpos.y != player.transform.position.z) {
				SwapHighRes (player.transform.position.x, player.transform.position.z, prevpos);
				prevpos.x = player.transform.position.x;
				prevpos.y = player.transform.position.z;
				positionchange.x = player.transform.position.x;
				positionchange.y = player.transform.position.z;
			}
		}

	}

	private void SetupTerrainChunks(){
		FindResolution ();
		//we're working with 32 because its the lowest possible resolution for lowres terrain
		//if (HighResImage.width > (maxresolution-1) * (int)TerrainArea / 32) {
			//Debug.Log ("Your higher resolution coming in is too high. Image will not fit on grid.");
			//return;
		//}
		BuildGrid ();

	}

	private void FindResolution(){
		maxresolution = (int)(ChunksSize * 2.5f);
		int closest = 5;
		for (int x = 1; x <= 7;) {
			if (Mathf.Abs (Mathf.Pow (2, closest) - maxresolution) <= Mathf.Abs (Mathf.Pow (2, closest + x) - maxresolution)) {
				//Debug.Log ("Closer to lower");
				x++;
			} else {
				//Debug.Log ("Closer to Higher");
				closest += 1;
			}
			//Debug.Log (Mathf.Abs (Mathf.Pow (2, closest) - maxresolution) + " " + Mathf.Abs (Mathf.Pow (2, closest + x) - maxresolution));
		}
		maxresolution = (int)Mathf.Pow (2, closest) + 1;
		//Debug.Log ("Max Resolution: " + maxresolution);

	}

	private void BuildGrid(){
		//start with lowest resolution, then we can scale accordingly
		int arraysize = (int)BaseImage.height / smallestResolution;
		ChunkHeightmapsHighRes = new Texture2D[arraysize*arraysize];
		ChunkHeightsmapsLowRes = new Texture2D[arraysize*arraysize];

		heightmapslow = new float[arraysize * arraysize] [,];
		heightmapshigh = new float[arraysize * arraysize] [,];

		for(int j = 0; j < arraysize*arraysize; j++){
			heightmapshigh[j] = new float[maxresolution, maxresolution];
			heightmapslow [j] = new float[smallestResolution, smallestResolution];
		}

		//Debug.Log ("The arraysize is: " + arraysize);
		Grid = new int[arraysize,arraysize];
		//initialize grid
		int index = 0; 
		for(int i = 0; i < arraysize; i++){
			for (int k = 0; k < arraysize; k++) {
				Grid [i, k] = index;
				Texture2D heightmaplow = SplitInitialImage (i, k, arraysize);
				Texture2D heightmaphigh = SplitHighResImage (i, k, arraysize);
				ChunkHeightsmapsLowRes [index] = heightmaplow;
				ChunkHeightmapsHighRes [index] = heightmaphigh;
				//Debug.Log ("Index of chunk: " + index);
				index++; 
			}
		}
		//Debug.Log ("The images in ChunkLowRes is: " + ChunkHeightsmapsLowRes.Length); 
		//Debug.Log ("The images in ChunkHighRes is: " + ChunkHeightmapsHighRes.Length);
		//initialized to that researched max amount of the max resolution that can be in scene

		//Debug.Log ("The amount of chunks in the grid is: " + Grid.Length);
		//Debug.Log ("The length of the highRes image array is: " + ChunkHeightmapsHighRes.Length + " and the lenght of the LowResImageArray is: " + ChunkHeightsmapsLowRes.Length); 
		InstantiateGrid (arraysize); 
	}

	private Texture2D SplitInitialImage(int posx, int posy, int arraysize){
		int height = BaseImage.height;
		//Debug.Log ("The height of the original image is: " + height); 

		int pixelsinchunk = height / arraysize;
		//Debug.Log ("Pixels in chunk: " + pixelsinchunk); 
		Texture2D newchunkimage = new Texture2D (pixelsinchunk, pixelsinchunk);
		//go through image
		//Debug.Log("Position in original image: " + posx + " " + posy);
		for (int i = 0; i < pixelsinchunk+1; i++) {
			for (int k = 0; k < pixelsinchunk+1; k++) {						
				newchunkimage.SetPixel (i, k, BaseImage.GetPixel (posx*pixelsinchunk+i,posy*pixelsinchunk+k));
				//smooth newchunk image, not
			}

		}
		//Debug.Log ("The length of the pixel chunk is: " + newchunkimage.width)
		//File.Create("C:/users/swri11/Documents/UnityProjects/ProceduralTerrain/Assets/SplitImage" + (posx+posy) + ".jpg").Dispose();
		//File.WriteAllBytes ("C:/users/swri11/Documents/UnityProjects/ProceduralTerrain/Assets/SplitImage" + (posx + posy) + ".jpg", newchunkimage.EncodeToJPG ());

		return newchunkimage;
		//add image to Grid, still arbitrary 0 until its the higher image with index
	}

	private Texture2D SplitHighResImage(int posx, int posy, int arraysize){
		int height = HighResImage.height;

		int pixelsinchunk2 = HighResImage.height/arraysize;
		//Debug.Log ("Pixels in highRes chunk: " + pixelsinchunk2);
		Texture2D newchunkimageHigh = new Texture2D (pixelsinchunk2, pixelsinchunk2);
		for (int i = 0; i < pixelsinchunk2; i++) {
			for (int k = 0; k < pixelsinchunk2; k++) {						
				newchunkimageHigh.SetPixel (i, k, HighResImage.GetPixel (posx * pixelsinchunk2 + i, posy * pixelsinchunk2 + k));
				//smooth newchunk image, not
			}

		}
		//File.Create("C:/users/swri11/Documents/UnityProjects/ProceduralTerrain/Assets/SplitImage" + "highres" + (posx+posy) + ".jpg").Dispose();
		//File.WriteAllBytes ("C:/users/swri11/Documents/UnityProjects/ProceduralTerrain/Assets/SplitImage" + "highres" + (posx + posy) + ".jpg", newchunkimageHigh.EncodeToJPG ());
		return newchunkimageHigh;
	}

	private void InstantiateGrid(int size){
		//create terrain array now that we know how many chunks there will be
		Terrains = new GameObject[size*size];
		int total = 0; 
		for (int i = 0; i < size; i++) {
			for (int k = 0; k < size; k++) {
				TerrainData tempp = new TerrainData ();

				//Lowest possible resolution
				tempp.size = new Vector3 (smallestResolution, 10, smallestResolution);
				SplatPrototype[] splats = new SplatPrototype[1];
				SplatPrototype splat = new SplatPrototype ();
				splats [0] = splat;
				splat.texture = ChunkHeightsmapsLowRes [total];
				splat.normalMap = testnormalmap;
				tempp.splatPrototypes = splats;
				LoadHeightmap (ChunkHeightsmapsLowRes[total], tempp, total, heightmapslow[total]); 
				GameObject newterrain = (GameObject)Terrain.CreateTerrainGameObject (tempp);

				//we are not going to use collisions because in theory those would be told to us, so we turn them off here
				newterrain.GetComponent<TerrainCollider>().enabled = false;

				newterrain.name = "Terrain" + total;
				newterrain.transform.position = new Vector3 (ChunksSize * i, 0, ChunksSize * k);
				tempp.size = new Vector3 (ChunksSize, 5, ChunksSize);
				newterrain.GetComponent<Terrain> ().castShadows = false;
				newterrain.GetComponent<Terrain> ().materialType = Terrain.MaterialType.Custom;
				newterrain.GetComponent<Terrain> ().materialTemplate = terrainmaterialtest;
				newterrain.GetComponent<Terrain> ().Flush ();
				Terrains [total] = newterrain;

				total++; 

			}
		}

		//skip the first line, assign neighbors
		for (int j = size; j < Terrains.Length-1; j++) {
			if (j % size > 1 && j+size < Terrains.Length-1) {
				Terrains [j].GetComponent<Terrain> ().SetNeighbors (Terrains [j - 1].GetComponent<Terrain>(),
					Terrains [j + size].GetComponent<Terrain>(), Terrains [j + 1].GetComponent<Terrain>(), Terrains [j - size].GetComponent<Terrain>());
			}
		}
		instantiated = true;
	}


		

	private void LoadHeightmap( Texture2D texture, TerrainData data, int premadeindex, float[,] map)
	{
		if (heightmapslow [premadeindex][0,0] == 0) {
			TextureScaler.scaled (texture, data.heightmapWidth, data.heightmapHeight, FilterMode.Trilinear);
			Color[] colors = texture.GetPixels ();
			float[,] m_heightValues = new float[(int)Mathf.Sqrt (colors.Length), (int)Mathf.Sqrt (colors.Length)];
			//Debug.Log ("The length of the heightmap is: " + colors.Length); 
			data.heightmapResolution = (int)Mathf.Sqrt (colors.Length) + 1;
			// Run through array and read height values.
			int index = 0;
			for (int z = 0; z < Mathf.Sqrt (colors.Length); z++) {
				
				for (int x = 0; x < Mathf.Sqrt (colors.Length); x++) {	
					m_heightValues [z, x] = colors [index].r;
					index++;
				}
			}
				
			// Now set terrain heights.
			data.size = new Vector3 ((int)Mathf.Sqrt (colors.Length), 5, (int)Mathf.Sqrt (colors.Length));
			map = m_heightValues;
		}
		SetHightMap (premadeindex, data, map);

	}

	private void SetHightMap(int index, TerrainData terrain, float[,] heightmap){
		terrain.SetHeights( 0, 0, heightmap);
	}

	private void SwapHighRes(float posx, float posz, Vector2 prevposition){


		//pos divided by 32, due to arraysize will give grid pos when rounded up
		//gonna need to change this to account for original position, using 7 as hard code currently
		int gridx = Mathf.CeilToInt(posx/ChunksSize)-1;
		int gridz = Mathf.CeilToInt (posz/ChunksSize)-1; 
		//Debug.Log ("Center of the player position on grid is: " + gridx + " " + gridz);

		//also change the grid in a square around the player
		for(int i = gridx-traillength; i <= gridx+traillength; i++){
			for (int k = gridz - traillength; k <= gridz + traillength; k++) {
				if (i < 0 || i > (int)TerrainArea / smallestResolution || k < 0 || k > (int)TerrainArea / smallestResolution) {
					continue;
				}
				if (i <= gridx - traillength|| i >= gridx + traillength || k <= gridz - traillength || k >= gridz + traillength) {
					GameObject changingTerrain = Terrains [Grid [i, k]];
					Texture2D heightmapChange = ChunkHeightsmapsLowRes [Grid [i, k]];
					//Debug.Log ("Index of changing grid is: " + gridx + gridy);
					changingTerrain.GetComponent<Terrain> ().terrainData.heightmapResolution = smallestResolution;
					LoadHeightmap (heightmapChange, changingTerrain.GetComponent<Terrain> ().terrainData, k+(i*k), heightmapslow[k+(i*k)]);
					changingTerrain.GetComponent<Terrain> ().terrainData.size = new Vector3 (ChunksSize, 5, ChunksSize);

		
				} else {
					//change to maxresolution
					//Debug.Log("The changed location is: " + i + k);
					if (gridx <= i+outlength && gridx >= i-outlength && gridz <= k+outlength && gridz >= k-outlength ) {
						GameObject changingTerrain = Terrains [Grid [i, k]];
						TerrainData oldcollider = changingTerrain.GetComponent<TerrainCollider> ().terrainData;

						Texture2D heightmapChange = ChunkHeightmapsHighRes [Grid [i, k]];
						//Debug.Log ("Index of changing grid is: " + gridx + gridy);
						changingTerrain.GetComponent<Terrain> ().terrainData.heightmapResolution = maxresolution;
						changingTerrain.GetComponent<Terrain> ().castShadows = false;
						LoadHeightmap (heightmapChange, changingTerrain.GetComponent<Terrain> ().terrainData, k+(i*k), heightmapshigh[k+(i*k)]);
						changingTerrain.GetComponent<Terrain> ().terrainData.size = new Vector3 (ChunksSize, 5, ChunksSize);
						changingTerrain.GetComponent<Terrain> ().Flush ();
					}
				}

			}
		}
	}

	void LoadStreamingTerrain(){
		//this is the important script for grabbing the images from file as they come in and parsing them to put them correctly in the grid

	}

	int[,] GrowGrid(int[,] grid, int newx, int newy){
		int[,] temp = grid; 
		grid = new int[newx, newy];
		temp.CopyTo (grid, 0);
		return grid;
	
	}
}


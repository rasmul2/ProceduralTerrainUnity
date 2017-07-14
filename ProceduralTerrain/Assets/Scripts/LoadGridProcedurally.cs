using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LoadGridProcedurally : MonoBehaviour {
	public GameObject player;

	public int terrainlength;
	public int terrainwidth;
	public int chunksize;

	public int speed;

	public Texture2D BaseImage;


	public Texture2D placeholder;
	public Material TerrainMaterial;

	public string imagesfolder;
	public int outlength;

	public int traillength;

	private int [,] Grid;
	private int[,] Gridlowres;
	private List<Texture2D> heightmaphighres;
	private List<Texture2D> heightmaplowres;
	private List<GameObject> terrains;
	public int beginposx;
	public int beginposy;

	private int total;
	private int index;

	private Vector3 startposition;
	private Vector3 currentposition;
	private Vector3 prevposition;
	float startTime;
	float Distance;

	private bool loaded = false;
	private bool first =  true;
	private bool startedcoroutine = false;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		total = 0;
		index = 0;
		heightmaphighres = new List<Texture2D> ();
		heightmaplowres = new List<Texture2D> ();
		terrains = new List<GameObject> ();
		Debug.Log ("This is the array size: " + Mathf.RoundToInt (terrainlength / chunksize));
		Grid = new int[Mathf.RoundToInt (terrainlength / chunksize), Mathf.RoundToInt (terrainwidth / chunksize)];
		Gridlowres = new int[Mathf.RoundToInt (terrainlength / chunksize), Mathf.RoundToInt (terrainwidth / chunksize)];

		DeletePreviousDirectory ();

		for (int i = 0; i < Mathf.RoundToInt (terrainlength / chunksize); i++) {
			for (int n = 0; n < Mathf.RoundToInt (terrainlength / chunksize); n++) {
				Grid [i, n] = -1;
				Gridlowres [i, n] = -1;
			}
		}

		int ind = 0;
		//build the grid
		for(int i = 0; i < terrainwidth/chunksize; i++){
			for (int k = 0; k < terrainlength / chunksize; k++) {
				heightmaplowres.Add(SplitInitialImage (i, k, terrainwidth / chunksize));
				Gridlowres [i, k] = ind;
				ind++;
			}
		}

		//create terrain
		int ind2 = 0;
		for(int i = 0; i < terrainwidth/chunksize; i++){
			for (int k = 0; k < terrainlength / chunksize; k++) {
				CreateTerrain (i, k, heightmaplowres [Gridlowres [i, k]], ind2);
				ind2++;
			}
		}
	
		loaded = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (loaded == true) {
			GetFiles ();
		}
			


	}

	void DeletePreviousDirectory(){
		DirectoryInfo directory = new DirectoryInfo (imagesfolder);
		FileInfo[] imagefiles = directory.GetFiles ();
		if (imagefiles.Length > 0) {
			for (int i = 0; i < imagefiles.Length; i++) {
				imagefiles [i].Delete ();
			}
		}
	}
	void GetFiles(){
		DirectoryInfo directory = new DirectoryInfo (imagesfolder);
		FileInfo[] imagefiles = directory.GetFiles ();
		if (total >= imagefiles.Length || imagefiles.Length == 0) {
			return;
		} else {
			if (imagefiles [total].Extension == ".png") {
					LoadImage (imagefiles [total].FullName);
					total++;

			} else {
				total++;
			}
		}
	}

	void LoadImage(string filename){
		
		FileStream file = new FileStream (filename, FileMode.Open, FileAccess.ReadWrite);
		Texture2D loadedimage = new Texture2D (312, 312);
		byte[] filebytes = new byte[file.Length];
		file.Read (filebytes, 0, (int)file.Length);
		Debug.Log (file.Length);
		loadedimage.LoadImage (filebytes);
		string parsedfilename = filename.Substring (filename.IndexOf("image"), filename.Length-filename.IndexOf("image"));
		int endremove = parsedfilename.IndexOf ('.'); 
		parsedfilename.Remove (endremove);
		string[] indexes = parsedfilename.Split (' ');
		indexes[0] = indexes [0].Substring (5);
		indexes [1] = indexes [1].Substring (0, indexes [1].IndexOf ('.'));
		Debug.Log (indexes [0]);
		Debug.Log (indexes [1]);
		int x = System.Int16.Parse (indexes [0]);
		int y = System.Int16.Parse (indexes [1]);


	
		heightmaphighres.Add (loadedimage);


		//this is to reconvert from left being positive and right being negative
		x = x*-1;
		//y = y*-1;
		Debug.Log ("The x and y locations originaly are: " + x + ' ' + y);
		int xtemp = x + beginposx;
		int ytemp = y + beginposy;
		Debug.Log ("The x and y locations with the offset are: " + xtemp + ' ' + ytemp);
		Grid [x+beginposx, y+beginposy] = index;
		LoadTerrainChunk (loadedimage, x+beginposx, y+beginposy);

	
		SwapHighRes (x+beginposx, y+beginposy, prevposition);
		if (first == true) {
			currentposition = new Vector3 (x + beginposy, y + beginposy);
			first = false;
		} else {
			prevposition = currentposition;
			currentposition = new Vector3 (x + beginposx, player.transform.position.y, y + beginposy);
			startTime = Time.time;
			StartCoroutine (movelerp (false));
			StartCoroutine (movelerp (true));
		}
	}

	void LoadTerrainChunk(Texture2D loadedimage, int xloc, int yloc){
		//parse the image name
		TerrainData data = terrains[Gridlowres[xloc,yloc]].GetComponent<Terrain>().terrainData;
		data.heightmapResolution = 129;

		LoadHeightmap (loadedimage, data);
		data.size = new Vector3 (chunksize, 5, chunksize);
		terrains [Gridlowres [xloc, yloc]].GetComponent<Terrain> ().Flush();
		index++;
	}

	private void LoadHeightmap( Texture2D texture, TerrainData data)
	{
		texture = TextureScaler.scaled (texture, data.heightmapResolution-1, data.heightmapResolution-1, FilterMode.Trilinear);
		//Debug.Log (texture.height);
			Color[] colors = texture.GetPixels ();
			float[,] m_heightValues = new float[texture.width, texture.height];
			//Debug.Log ("The length of the heightmap is: " + colors.Length); 
			//data.heightmapResolution = (int)Mathf.Sqrt (colors.Length) + 1;
			// Run through array and read height values.
			int index = 0;
			for (int z = 0; z < Mathf.Sqrt (colors.Length); z++) {

				for (int x = 0; x < Mathf.Sqrt (colors.Length); x++) {	
					m_heightValues [z, x] = colors [index].r;
					index++;
				}
			}
		//SplatPrototype[] splats = new SplatPrototype[1];
		//SplatPrototype splat = new SplatPrototype ();
		//splats [0] = splat;
		//splat.texture = texture;
		//splat.tileSize = new Vector2 (10, 10);
		//splat.normalMap = testnormalmap;
		//data.splatPrototypes = splats;

			// Now set terrain heights.
		data.size = new Vector3 (texture.width, 5, texture.height);
		data.SetHeights( 0, 0, m_heightValues);


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

	private void CreateTerrain(int x, int y, Texture2D lowresimage, int index){
		TerrainData data = new TerrainData ();
		data.heightmapResolution = 32;
		LoadHeightmap (lowresimage, data);

		data.size = new Vector3 (chunksize, 2, chunksize);
		GameObject newterrain = (GameObject)Terrain.CreateTerrainGameObject (data);

		//we are not going to use collisions because in theory those would be told to us, so we turn them off here
		newterrain.GetComponent<TerrainCollider>().enabled = false;

		newterrain.name = "Terrain" + index;
		newterrain.transform.position = new Vector3 (chunksize * x, 0,chunksize* y);
		newterrain.GetComponent<Terrain> ().castShadows = false;
		newterrain.GetComponent<Terrain> ().materialType = Terrain.MaterialType.Custom;
		newterrain.GetComponent<Terrain> ().materialTemplate = TerrainMaterial;
		newterrain.GetComponent<Terrain> ().Flush ();
		terrains.Add(newterrain);

	}

	private void SwapHighRes(float posx, float posz, Vector2 prevposition){


		//pos divided by 32, due to arraysize will give grid pos when rounded up
		//gonna need to change this to account for original position, using 7 as hard code currently
		int gridx = (int)posx;
		int gridz = (int)posz; 
		Debug.Log ("Center of the player position on grid is: " + gridx + " " + gridz);

		//also change the grid in a square around the player
		for(int i = gridx-traillength; i <= gridx+traillength; i++){
			for (int k = gridz - traillength; k <= gridz + traillength; k++) {
				if (i < 0 || i > (int)terrainwidth / 32 || k < 0 || k > (int)terrainlength / 32) {
					continue;
				}
				if (i <= gridx - traillength|| i >= gridx + traillength || k <= gridz - traillength || k >= gridz + traillength || Grid[i,k] == -1) {
					GameObject changingTerrain = terrains [Gridlowres [i, k]];
					Texture2D heightmapChange = heightmaplowres [Gridlowres [i, k]];
					//Debug.Log ("Index of changing grid is: " + gridx + gridy);
					changingTerrain.GetComponent<Terrain> ().terrainData.heightmapResolution = 32;
					LoadHeightmap (heightmapChange, changingTerrain.GetComponent<Terrain> ().terrainData);
					changingTerrain.GetComponent<Terrain> ().terrainData.size = new Vector3 (chunksize, 2, chunksize);
					changingTerrain.GetComponent<Terrain>().Flush();

				} else {
					//change to maxresolution
					//Debug.Log("The changed location is: " + i + k);
					if (gridx <= i+outlength && gridx >= i-outlength && gridz <= k+outlength && gridz >= k-outlength ) {
						GameObject changingTerrain = terrains [Gridlowres [i, k]];
						TerrainData oldcollider = changingTerrain.GetComponent<TerrainCollider> ().terrainData;

						Texture2D heightmapChange = heightmaphighres [Grid [i, k]];
			
							//Debug.Log ("Index of changing grid is: " + gridx + gridy);
							changingTerrain.GetComponent<Terrain> ().terrainData.heightmapResolution = 129;
							changingTerrain.GetComponent<Terrain> ().castShadows = false;
							LoadHeightmap (heightmapChange, changingTerrain.GetComponent<Terrain> ().terrainData);
							changingTerrain.GetComponent<Terrain> ().terrainData.size = new Vector3 (chunksize, 5, chunksize);
							changingTerrain.GetComponent<Terrain> ().Flush ();
					}
				}

			}
		}
	}

	/*----------------------------------These are made to handle the movement on a seperate thread-------------------------------------*/

	IEnumerator lerpPosition(Vector3 previousp, Vector3 currentp){
		bool notthere = true;
		while (notthere) {
			Debug.Log ("The x and y change positions are: " + currentp.x + ' ' + currentp.z);
			Debug.Log ("The previous x and y position are: " + previousp.x + ' ' + previousp.z);
			float distancecovered = (Time.time - startTime) * speed;
			Vector3 adjustforchunkprev = new Vector3 (previousp.x * chunksize, player.transform.position.y, previousp.z * chunksize);
			Vector3 adjustforchunkcurrent = new Vector3 (currentp.x * chunksize, player.transform.position.y, currentp.z * chunksize);
			float journeylength = Vector3.Distance (adjustforchunkprev, adjustforchunkcurrent);
			float fractionaljourney = distancecovered / journeylength;

			player.transform.position = Vector3.Lerp (adjustforchunkprev, adjustforchunkcurrent, fractionaljourney);
			if (fractionaljourney == 1) {
				notthere = false;

			}
			yield return null;
		}
	}

	IEnumerator movelerp(bool strtorstp){
		if (strtorstp == true) {
			Debug.Log ("Started");
			StartCoroutine (lerpPosition (prevposition, currentposition));
			yield return null;
		} else {
			Debug.Log ("Stopped");
			StopAllCoroutines ();
			yield return null;
		}

	}

}

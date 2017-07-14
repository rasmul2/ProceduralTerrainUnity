using UnityEngine;
using System.Collections;
using System.IO;

public class TerrainLoader : MonoBehaviour
{
	public Terrain terrain;
	public Texture2D heightmap;
	public string url;
	public string file; 
	// Member variables.
	//----------------------------------------------------------------------------------------------
	private Terrain    m_terrain      = null;
	private float[ , ] m_heightValues = null;
	private int        m_resolution   = 0;
	// Private methods.
	//----------------------------------------------------------------------------------------------
	private void Start()
	{
		m_terrain = terrain;
		m_resolution = m_terrain.terrainData.heightmapResolution;

		if (url != "") {
			heightmap = new Texture2D (500, 500);
			LoadFromSite (url);
		} else if (file != "") {
			heightmap = new Texture2D (500, 500);
			LoadTerrain (file, terrain.terrainData);
			m_heightValues = new float[ heightmap.height, heightmap.width ];
			terrain.terrainData.heightmapResolution = heightmap.height + 1;
			LoadHeightmap (heightmap.GetPixels ());
		}else {
			m_heightValues = new float[ m_resolution, m_resolution ];
			LoadHeightmap (heightmap.GetPixels());
		}
	}

	public void Load(){
		
	}

	private void LoadHeightmap( Color[] colors )
	{
		float endsizex = terrain.terrainData.size.x;
		float endsizey = terrain.terrainData.size.z;
		terrain.terrainData.size = new Vector3 (heightmap.height, terrain.terrainData.size.y, heightmap.width);
		//Debug.Log ("Scale size:" + endsizex + endsizey); 

		// Run through array and read height values.
		int index = 0;
		for ( int z = 0; z < heightmap.height; z++ )
		{
			for ( int x = 0; x < heightmap.width; x++ )
			{	
				m_heightValues[ z, x] = colors[ index ].r;
				index++;
			}
		}

		// Now set terrain heights.
		m_terrain.terrainData.SetHeights( 0, 0, m_heightValues );
	
		terrain.terrainData.size = new Vector3 (endsizex, terrain.terrainData.size.y, endsizey);
	}


	void LoadFromSite(string site){
		StartCoroutine (DownloadHeightmap (site));
	}

	IEnumerator DownloadHeightmap(string image){
		WWW www = new WWW (image);
		yield return www;

		www.LoadImageIntoTexture (heightmap);

		m_heightValues = new float[ m_resolution, m_resolution ];
		int hheight = heightmap.height;
		int hwidth = heightmap.width;
		int theight = (int)terrain.terrainData.size.z;
		int twidth = (int)terrain.terrainData.size.x;

		LoadHeightmap( heightmap.GetPixels() );
	}

 
void LoadTerrain(string aFileName, TerrainData aTerrain)
 {
     int h = aTerrain.heightmapHeight;
     int w = aTerrain.heightmapWidth;
	
		if (File.Exists (aFileName)) {
			byte[] fileData = File.ReadAllBytes (aFileName);
			heightmap.LoadImage (fileData);
		} else {
			Debug.Log ("Did not find file");
		}
  
 }

}

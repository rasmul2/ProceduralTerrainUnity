using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class TrackingDownload : MonoBehaviour {

	// Use this for initialization
	public Material cloudmaterial;
	public GameObject plane2;
	public Object clouds;
	Texture2D texture;
	Texture2D texture2;
	private string url = "http://aviationweather.gov/data/obs/sat/intl/ir_ICAO-B1_bw.jpg";
	private string urlclouds = "http://aviationweather.gov/data/obs/sat/intl/ir_ICAO-B1.jpg";

	private XmlDocument document; 
	void Start () {
		texture = new Texture2D (1280, 1280);
		texture2 = new Texture2D (1280, 1280);
		StartCoroutine (Download ());
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator Download(){
		WWW www = new WWW (url);
		WWW www2 = new WWW (urlclouds);
		WWWForm form = new WWWForm ();
		yield return www2;

		Debug.Log (www2.text);
		www2.LoadImageIntoTexture (texture);
		cloudmaterial.mainTexture = texture;
		FindWhite ();
		//plane.GetComponent<Renderer> ().material.mainTexture = texture;
		//yield return www2;
		//www2.LoadImageIntoTexture (texture2);

		//plane2.GetComponent<Renderer> ().material.mainTexture = texture2;
		StopCoroutine ("Download");
		StartCoroutine (Download ());

	}

	void FindWhite(){
		GameObject[] madeclouds = GameObject.FindGameObjectsWithTag ("clouds");
		foreach (GameObject cloud in madeclouds) {
			Destroy (cloud);
		}

		for (int i = 0; i < texture.width; i++) {
			for (int k = 0; k < texture.height; k++) {
				if (texture.GetPixel (i, k).b <= .35 && texture.GetPixel (i, k).g <= .35) {
					Color transparent = new Color (0, 0, 0, 0);
					texture.SetPixel (i, k, transparent);
				} else {
					if (texture.GetPixel (i, k).r >= .6 && texture.GetPixel (i, k).g >= .6 && texture.GetPixel (i, k).b >= .6) {
						Color transparent = new Color (0, 0, 0, 0);
						texture.SetPixel (i, k, transparent);
					}
					Color white = new Color (255, 255, 255, 1);
					//texture.SetPixel (i, k, white);

				}
			}
		}
		texture.Apply ();
		MakeClouds ();
	}

	void MakeClouds(){
		for (int i = 0; i < texture.width; i++) {
			for (int k = 0; k < texture.height; k++) {
				if (texture.GetPixel (i, k).b >= .5 && texture.GetPixel (i - 25, k).b >= .5 && texture.GetPixel (i + 25, k).b >= .5 && texture.GetPixel (i, k - 25).b >= .5 && texture.GetPixel (i, k + 25).b >= .5) {
					//improve with if statements for if their within a larger range, then use a larger version of the object
					float x = i;
					float y = k;
					GameObject currentcloud = (Instantiate (clouds, new Vector3 (x/63, 5, y/63), new Quaternion ()))as GameObject;
					if(texture.GetPixel (i, k).b >= .5 && texture.GetPixel (i - 50, k).b >= .5 && texture.GetPixel (i + 50, k).b >= .5 && texture.GetPixel (i, k - 50).b >= .5 && texture.GetPixel (i, k + 50).b >= .5) {
						currentcloud.transform.localScale = new Vector3 (.03f, .03f, .03f);
						k += 5;
						i += 5;
					}
					k += 5;
					i += 5;
				}
			}
		}
	}
		
}

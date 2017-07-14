using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRenderGapsScript : MonoBehaviour {
	public RenderTexture[] renderTextures;
	// Use this for initialization
	private Texture[] previoustextures;
	private Texture[] currenttextures;
	void Start () {
		for (int i = 0; i < renderTextures.Length; i++) {
			previoustextures[i] = new Texture ();
			currenttextures[i] = new Texture ();
			previoustextures[i] = (Texture)gameObject.GetComponent<Renderer> ().material.mainTexture;
		}
	}
	
	// Update is called once per frame
	void Update () {

		Renderer[] maintextures = gameObject.GetComponents<Renderer> (); 
		for (int i = 0; i < maintextures.Length; i++) {
			currenttextures [i] = (Texture)renderTextures [i];
			maintextures [i].material.mainTexture = previoustextures [i];
			previoustextures [i] = currenttextures [i];
		}
	

	}
}

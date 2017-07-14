using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerController : MonoBehaviour {

	public GameObject MarkerPrefab;
	public GameObject player;
	// Use this for initialization
	private Dictionary<string, Vector2> markers;
	private int markerscount = 0;
	void Start () {
		markers = new Dictionary<string, Vector2> (0); 
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.P) == true) {
			PlaceMarker ();
		}
	}
	
	public void PlaceMarker(){
		markerscount += 1;
		markers.Add(markerscount.ToString(), new Vector2(player.transform.position.x, player.transform.position.z));
		Instantiate ((Object)MarkerPrefab, new Vector3(player.transform.position.x, 10, player.transform.position.z), MarkerPrefab.transform.rotation); 
	}
}

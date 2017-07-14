using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationalGui : MonoBehaviour {

	public GameObject GridBuilder;
	public GameObject player;
	//public GameObject Rover;
	public GameObject UI;
	public GameObject head;
	public Transform location; 
	// Use this for initialization
	private Transform startlocation;
	private Transform previouslocation;

	private float movelength; 
	void Start () {
		GameObject.Find("Map").GetComponent<Renderer> ().material.mainTexture = GridBuilder.GetComponent<GridBuilder> ().BaseImage;
		startlocation = location;
		UI.SetActive (false);
		previouslocation = head.transform;

	}
	// Update is called once per frame
	void Update () {
		UI.transform.position = head.transform.position;
		UI.transform.rotation = head.transform.rotation;
		if (Input.GetKeyDown (KeyCode.M) == true || Input.GetKeyDown(KeyCode.Joystick2Button15)) {
			UI.SetActive (true);
			Debug.Log ("The current position is: " + GridBuilder.GetComponent<GridBuilder> ().positionchange); 
		} else if(Input.GetKeyUp(KeyCode.M) == true || Input.GetKeyUp(KeyCode.Joystick2Button15)){
			UI.SetActive (false); 
		}
		//location.localPosition = new Vector3 (4.25f-player.transform.position.x/30,  .2f, 4.25f-player.transform.position.z/30);
	}

	private void onEnable(){
	}

	private void onDisable(){
	}
	//create a zoom that then shows the higher resolution of the terrain square you're on
}

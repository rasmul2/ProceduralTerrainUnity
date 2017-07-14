using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAlongCollider : MonoBehaviour {

	// Use this for initialization
	public GameObject follow;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = new Vector3(follow.GetComponent<Transform> ().position.x, 0, follow.GetComponent<Transform> ().position.z); 
		GetComponent<Transform> ().position = pos;
	}
}

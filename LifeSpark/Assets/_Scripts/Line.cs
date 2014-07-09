﻿using UnityEngine;
using System.Collections;

public class Line : LSMonoBehaviour {
	public SparkPoint[] edgePoints;	// should just have 2 spark points
	public int state; // may not be used // now -1 = broken, 0 = unoccupied, 1 = team 1, 2 = team 2

	// Use this for initialization
	void Start () {
		// initial position
		this.transform.position = (edgePoints [0].transform.position + edgePoints [1].transform.position) / 2;
		// initial scale
		this.transform.localScale = new Vector3(Mathf.Sqrt((edgePoints [0].transform.position - edgePoints [1].transform.position).sqrMagnitude), 1, 1);
		// initial angle 
		// *now it just support all spark point at same y
		Vector3 direction = edgePoints [0].transform.position - edgePoints [1].transform.position;
		float angle = Vector3.Angle (direction, this.transform.right);
		this.transform.Rotate (new Vector3 (0, angle, 0));
		//
		state = 0;
	}
	
	// Update is called once per frame
	void Update () {
		RPC_setLineMaterial (state);
	}

	//--------------------------------------------------------------------------
	// Set color functions
	//---------------------------
	// Set Line Color By State
	[RPC]
	void RPC_setLineColorByState (int state) {
		this.state = state;
		// just change material's color
		if (state == -1) {
			this.renderer.material.color = Color.black;
		} else if (state == 0) {
			this.renderer.material.color = Color.white;
		} else if (state == 1) {
			this.renderer.material.color = Color.red;
		} else if (state == 2) {
			this.renderer.material.color = Color.blue;
		} else {
			this.renderer.material.color = Color.yellow;
		}
	}

	// Set Line Material
	[RPC]
	void RPC_setLineMaterial (int state) {
		this.state = state;
		GameObject temp = GameObject.Find ("Terrain/Ground"); 
		if (state == -1) {
			this.renderer.material = temp.GetComponent<MasterManager>().BrokenLine;
		} else if (state == 0) {
			this.renderer.material = temp.GetComponent<MasterManager>().OriginalLine;
		} else if (state == 1) {
			this.renderer.material = temp.GetComponent<MasterManager>().Team1Line;
		} else if (state == 2) {
			this.renderer.material = temp.GetComponent<MasterManager>().Team2Line;
		} else {
			this.renderer.material = temp.GetComponent<MasterManager>().OriginalLine;
		}
	}
	//--------------------------------------------------------------------------
}

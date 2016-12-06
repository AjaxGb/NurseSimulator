﻿using UnityEngine;

public class Player : MonoBehaviour {

	public static Player inst { get; private set; }

	// Use this for initialization
	void Start () {
		if (inst != null) Debug.LogWarning("Two players in scene at once!");
		inst = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
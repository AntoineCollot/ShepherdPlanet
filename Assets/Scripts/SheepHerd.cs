using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepHerd : MonoBehaviour {

    [HideInInspector]
    public SheepBoid[] sheeps;

    public static SheepHerd Instance;

	// Use this for initialization
	void Awake () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Start () {
        sheeps = GetComponentsInChildren<SheepBoid>(false);
	}
}

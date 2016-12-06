using UnityEngine;
using System.Collections;

public class CheckRequired : MonoBehaviour {

    float LevelTimeLeft = 120.0f;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        LevelTimeLeft -= Time.deltaTime;
        if (LevelTimeLeft < 0)
        {
            NextLevel();
        }
    }

    void NextLevel()
    {

    }
}

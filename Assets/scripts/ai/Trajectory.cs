using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour {

    public List<Vector3> positions; 
    public bool validLanding = false;
    private int decayValue;
    public float speed = 1f;
    public int score = -1;
	// Use this for initialization
	void Start () {
        decayValue = 300;
	}
	
	// Update is called once per frame
	void Update () {
        decayValue--;
        if(decayValue <= 0)
        {
            Destroy(gameObject);
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCut : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Debug.Log("Loaded");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Exited");
        if (other.transform.parent.CompareTag("Laser"))
        {
            Debug.Log("true");
        }
    }
}

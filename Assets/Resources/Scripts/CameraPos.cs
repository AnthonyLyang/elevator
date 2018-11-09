using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    ObjectStorage storage;
    Vector3 PosRef = Vector3.zero;
    public float OffSetY;
    Transform Player;
	// Use this for initialization
	void Start ()
    {
        storage = ObjectStorage.Instance;
        Player = storage.Player.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        PosRef = transform.position;
        PosRef.y = Player.position.y + OffSetY;
        transform.position = PosRef;
	}
}

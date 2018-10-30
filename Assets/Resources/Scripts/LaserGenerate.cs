using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGenerate : MonoBehaviour {

    LineRenderer Line;
    Transform Node1;
    Transform Node2;
    Transform LineRender;
    Transform ColliderTransform;
    BoxCollider LineCollider;
    Vector3 LineDir;
	// Use this for initialization
	void Awake ()
    {
        Node1 = transform.Find("LaserNode");
        Node2 = transform.Find("LaserNode (1)");
        LineRender = transform.Find("LaserLine");
        Line = LineRender.gameObject.GetComponent<LineRenderer>();
        ColliderTransform = transform.Find("LaserCollider");
        LineCollider = ColliderTransform.gameObject.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void OnEnable()
    {
        LineDir = (Node2.localPosition - Node1.localPosition);
        Line.SetPositions(new Vector3[] { Node1.localPosition, Node2.localPosition });
        ColliderTransform.localPosition = Node1.localPosition + (LineDir / 2);
        ColliderTransform.LookAt(Node2.position);
        var temp = LineCollider.size;
        temp.z = LineDir.magnitude;
        LineCollider.size = temp;
    }
}

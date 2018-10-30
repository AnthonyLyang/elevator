using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMove : MonoBehaviour {
    public bool GoingUp;
    public float Speed;
    Rigidbody rigid;
	// Use this for initialization
	void Start ()
    {
        rigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (GoingUp)
        {
            //rigid.velocity = new Vector3(0, 10f, 0);
            transform.position += new Vector3(0, Speed * Time.fixedDeltaTime, 0);
            //transform.Translate(new Vector3(0, Speed * Time.fixedDeltaTime, 0), Space.World);
        }
        else
        {
            transform.position -= new Vector3(0, Speed * Time.fixedDeltaTime, 0);
            //transform.Translate(new Vector3(0, Speed * Time.fixedDeltaTime, 0), Space.World);
        }
	}
    private void OnCollisionEnter(Collision collision)
    {
        //collision.rigidbody.useGravity = false;
        //if (GoingUp)
        //{
        //    rigid.velocity = new Vector3(0, Speed, 0);
        //}
        //else
        //{
        //    rigid.velocity = new Vector3(0, -Speed, 0);
        //}
    }
    private void OnCollisionStay(Collision collision)
    {
        //rigid.AddForceAtPosition(-1*collision.rigidbody.mass*Physics.gravity, collision.contacts[0].point);
    }

}

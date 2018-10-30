using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {
    Rigidbody rigid;
    bool IsGrounded;
    VerticalMove move;
	// Use this for initialization
	void Start ()
    {
        rigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(rigid.velocity);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (transform.parent == null)
                return;
            if (!IsGrounded)
                return;
            rigid.AddForce(new Vector3(0, rigid.mass * 10f, 0), ForceMode.Impulse);
            IsGrounded = false;
        }
	}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Elevator"))
        {
            IsGrounded = true;
            transform.SetParent(collision.transform.parent);
            Debug.Log("enter");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        rigid.useGravity = true;
        if (collision.collider.CompareTag("Elevator"))
        {
            if (IsGrounded)
            {
                return;
            }
            //move = transform.parent.GetComponent<VerticalMove>();
            //if (move.GoingUp)
            //{
            //    Debug.Log("true");
            //    rigid.velocity = new Vector3(0, move.Speed, 0);
            //}
            //else
            //{
            //    rigid.velocity = new Vector3(0, move.Speed, 0);
            //}
            transform.SetParent(null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformControllTest : MonoBehaviour
{
    [HideInInspector]
    public bool IsGrounded = false;



    public float MoveSpeed;
    public float RotateSpeed;
    public float JumpIniSpeed;
    Vector3 MoveDir = Vector3.zero;
    Vector3 Fall = Vector3.zero;
    float MoveTemp;
    EleSplit ele;
    Transform ObjManager;
	// Use this for initialization
	void Start ()
    {
        ObjManager = transform.parent;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        CRotate(Input.GetAxis("Horizontal"));
        if (IsGrounded)
        {
            MoveTemp = Input.GetAxis("Vertical");
            MoveDir = transform.forward;
        }
        else
        {
            Fall += Physics.gravity * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsGrounded = false;
            transform.SetParent(ObjManager);
            Fall.y = JumpIniSpeed;
        }
        Move();
    }

    //碰撞实现的上下电梯
    //离开电梯可能不用碰撞控制更好
    //因为坍塌而离开时在电梯上通过gamemode控制player和电梯的父子关系
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Land");
        if (collision.transform.parent.CompareTag("ELEVATOR"))
        {
            transform.SetParent(collision.transform.parent);
            IsGrounded = true;
            Fall = Vector3.zero;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.parent.gameObject.CompareTag("ELEVATOR"))
        {
            transform.SetParent(ObjManager);
        }
    }

    void CRotate(float HInput)
    {
        transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime * HInput);
    }
    void Move()
    {
        transform.localPosition += (MoveDir * MoveSpeed * MoveTemp + Fall) * Time.deltaTime;
    }
}

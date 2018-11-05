using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//还是要用射线检测和状态机模式确认角色是不是在地面上
//继续修改

public enum VStat
{
    Grounded,
    Rising,
    Falling
}
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
    [HideInInspector]
    public VStat stat=VStat.Falling;

    RaycastHit hit;
	// Use this for initialization
	void Start ()
    {
        ObjManager = transform.parent;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        switch (stat)
        {
            case VStat.Falling:
                {
                    Fall += Physics.gravity * Time.deltaTime;
                    IsGrounded = GroundCheck();
                    if (IsGrounded)
                    {
                        Fall = Vector3.zero;
                        if (hit.transform.parent.CompareTag("ELEVATOR"))
                        {
                            transform.SetParent(hit.transform.parent);
                        }
                        stat = VStat.Grounded;
                    }
                    break;
                }
            case VStat.Rising:
                {
                    Fall += Physics.gravity * Time.deltaTime;
                    if (Fall.y <= 0)
                    {
                        stat = VStat.Falling;
                    }
                    break;
                }
            case VStat.Grounded:
                {
                    MoveTemp = Input.GetAxis("Vertical");
                    MoveDir = transform.forward;
                    IsGrounded = GroundCheck();
                    if (!IsGrounded)
                    {
                        transform.SetParent(ObjManager);
                        stat = VStat.Falling;
                        break;
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        IsGrounded = false;
                        transform.SetParent(ObjManager);
                        Fall.y = JumpIniSpeed;
                        stat = VStat.Rising;
                        break;
                    }
                    break;
                }
        }
        CRotate(Input.GetAxis("Horizontal"));
        Move();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent.CompareTag("WALL"))
        {
            MoveTemp *= -1;
        }
    }
    //碰撞实现的上下电梯
    //离开电梯可能不用碰撞控制更好
    //因为坍塌而离开时在电梯上通过gamemode控制player和电梯的父子关系
    bool GroundCheck()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out hit, 1.3f))
        {
            if (hit.transform.parent.CompareTag("ELEVATOR")||hit.transform.CompareTag("FALL"))
            {
                return true;
            }
        }
        return false;
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

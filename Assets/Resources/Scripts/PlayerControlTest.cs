using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Grounded
{
    T,
    I,
    F
}
public class PlayerControlTest : MonoBehaviour
{
    //在地面时按横轴输入转向
    //转向时先做瞬间转向
    //改进成短时间内完成转向
    //纵轴输入前进后退速度
    //不在地面时保存在地上最后一刻的速度
    //Y轴加一个向下的加速度
    //落地还是用射线做吧
    //落地之后要把
    RaycastHit hit;
    CharacterController cc;
    Vector3 Fall;
    bool IsGrounded;
    public float MoveSpeed;
    public float RotateSpeed;
    public float JumpSpeed;
    float Movetemp;
    Vector3 DirTemp;
    Grounded g;
    private void Start()
    {
        Fall = Vector3.zero;
        cc = GetComponent<CharacterController>();
        g = Grounded.T;
    }
    private void FixedUpdate()
    {

        Debug.Log(IsGrounded);
        CCRotate(Input.GetAxis("Horizontal"));
        switch (g)
        {
            case Grounded.T:
                {
                    Movetemp = Input.GetAxis("Vertical");
                    DirTemp = transform.forward;
                    cc.Move(transform.forward * MoveSpeed * Movetemp * Time.deltaTime);
                    IsGrounded = IsGroundCheck();
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Debug.Log("GET");
                        Fall = new Vector3(0, JumpSpeed, 0);
                        g = Grounded.I;
                    }
                    else if (!IsGrounded)
                    {
                        g = Grounded.I;
                    }
                    break;
                }
            case Grounded.I:
                {
                    cc.Move((DirTemp * MoveSpeed * Movetemp + Fall) * Time.deltaTime);
                    Fall += Physics.gravity * Time.deltaTime;
                    if (Fall.y <= 0)
                    {
                        g = Grounded.F;
                    }
                    break;
                }
            case Grounded.F:
                {
                    cc.Move((DirTemp * MoveSpeed * Movetemp + Fall) * Time.deltaTime);
                    Fall += Physics.gravity * Time.deltaTime;
                    IsGrounded =IsGroundCheck();
                    if (IsGrounded)
                    {
                        Fall = Vector3.zero;
                        g = Grounded.T;
                    }
                    break;
                }
        }
    }



    void CCRotate(float HInput)//旋转函数 用横轴输入转表现好像挺好的 不想改了
    {
        transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime * HInput);
    }
    void Move()
    {
        if (IsGrounded)
        {
            cc.Move(transform.forward * Movetemp * MoveSpeed * Time.deltaTime);
        }
        else
        {
            cc.Move(((Movetemp * DirTemp * MoveSpeed) + Fall) * Time.deltaTime);
        }
    }
    bool IsGroundCheck()
    {
        if (Physics.Raycast(transform.position, (-1 * transform.up), out hit, 1.1f))
        {
            //if (hit.transform.CompareTag("ELEVATOR"))
            //{
            //    return true;
            //}
            transform.SetParent(hit.transform);
            return true;
        }
        transform.SetParent(null);
        return false;
    }
}
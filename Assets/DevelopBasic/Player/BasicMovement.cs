using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BasicMovement : MonoBehaviour
{
[Header("Player movement")]
    [SerializeField] private float walkSpeed   = 1;
    [SerializeField] private float sprintSpeed = 2;
    [SerializeField] private float turnSmooth  = 1;
    [Range(0,1)] public float SpeedScale  = 1;
    public Vector3 velocity{get{return m_rigid.linearVelocity;}}
    private bool canMove = true;
    private bool canRotate = true;
    private Rigidbody m_rigid;
    void Awake()=>m_rigid = GetComponent<Rigidbody>();
    public void MovePlayer(Vector3 moveVector, bool sprinting){
        if(!canMove) return;
        moveVector.y = 0;
        if(canRotate){
            if(moveVector!=Vector3.zero){
                Quaternion targetRotation = Quaternion.LookRotation(moveVector);
                Quaternion newRotatation  = Quaternion.Slerp(m_rigid.rotation, targetRotation, turnSmooth*Time.fixedDeltaTime);
                m_rigid.MoveRotation(newRotatation);
            }
            moveVector = transform.forward * moveVector.magnitude * SpeedScale * (sprinting?sprintSpeed:walkSpeed);
            moveVector.y = m_rigid.linearVelocity.y;
        }
        else{
            moveVector = moveVector * SpeedScale * (sprinting?sprintSpeed:walkSpeed);
            moveVector.y = m_rigid.linearVelocity.y;
        }
        m_rigid.linearVelocity = moveVector;
    }
    public void MovePlayerToPoint(Vector3 targetPoint, bool sprinting){
        if(!canMove) return;
        Vector3 diff = targetPoint - transform.position;
        diff.y = 0;
        // if(diff.magnitude >= 0.1f){
            Quaternion targetRotation = Quaternion.LookRotation(diff);
            Quaternion newRotation = Quaternion.Slerp(m_rigid.rotation, targetRotation, turnSmooth*Time.fixedDeltaTime);
            m_rigid.MoveRotation(newRotation);
        // }
        // m_rigid.velocity = transform.forward * Mathf.Min(1,diff.magnitude) * (sprinting?sprintSpeed:walkSpeed);
        m_rigid.MovePosition(m_rigid.position + transform.forward * Mathf.Min(1,diff.magnitude) * (sprinting?sprintSpeed:walkSpeed) * Time.fixedDeltaTime);
    }
    public void JumpPlayer(float jumpForce){
        m_rigid.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }
    public bool CloseToTarget(Vector3 targetPoint){
        Vector3 diff = targetPoint - transform.position;
        diff.y = 0;
        return diff.magnitude<0.1f;
    }
    public void FaceDirection(Vector3 lookDir){m_rigid.MoveRotation(Quaternion.LookRotation(lookDir, Vector3.up));}
    public void FaceDirection(Quaternion direction){m_rigid.MoveRotation(direction);}
    public void SwitchCanMove(bool value)=>canMove = value;
    public void SwitchCanRotate(bool value)=>canRotate = value;
    public void ResetVelocity(){m_rigid.linearVelocity = Vector3.zero;}
    public void SwitchPhysics(bool isOn){m_rigid.isKinematic = !isOn;}
}
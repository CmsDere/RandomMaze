using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveComponent : PlayerComponent
{
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float gravity = 20f;

    CharacterController con;
    
    Animator anim;
    Vector3 move;
    [SerializeField] Transform cameraTransform;

    void Start()
    {
        con = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        move = Vector3.zero;
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (con.isGrounded)
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            move = cameraTransform.TransformDirection(move);
            transform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y) ;
            move *= moveSpeed;
        }
        move.y -= gravity * Time.deltaTime;
        con.Move(move * Time.deltaTime);
    }

    
}

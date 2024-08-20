using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    [SerializeField] GameObject cameraController;

    public float jumpForce, distToGround = .5f;
    public float gravity = 4;

    [NonSerialized] public float xMomentum, yMomentum, yAngle, yAngleCamera, speed = 1;
    int state = 1;
    bool isGrounded;
    RaycastHit groundRaycast;
    Vector3 deltaVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        xMomentum = Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));

        yAngleCamera = cameraController.transform.rotation.eulerAngles.y;
        yAngle = (Mathf.Rad2Deg * Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        transform.rotation = Quaternion.Euler(0f, yAngle + yAngleCamera, 0f);

        /* Uses a State Machine like code to switch between walking, jumping, idle etc.
         * 
         * States:
         * 0:Falling, 1:Idle, 2:Walking, 3:Running
        */

        if (!isGrounded)
        {
            state = 0;
        }

        //Falling
        if (state == 0)
        {
            if (isGrounded)
            {
                state = 1;
                animator.ResetTrigger("Jump");
            }
        }

        //Idle
        if (state == 1)
        {
            if (xMomentum >= .2f)
            {
                state = 2;
            }

            checkJump();
        }

        //Walking
        if (state == 2)
        {
            if (Input.GetButton("Run"))
            {
                state = 3;
            }

            else if (xMomentum < .2f)
            {
                state = 1;
            }

            checkJump();
        }

        //Running
        if (state == 3)
        {
            if (!Input.GetButton("Run"))
            {
                state = 2;
            }

            checkJump();
        }


        animator.SetInteger("State", state);
    }

    void FixedUpdate()
    {
        isGrounded = Physics.SphereCast(col.bounds.center, .25f, Vector3.down, out groundRaycast, distToGround, LayerMask.GetMask("Default"));

        if (state == 0)
        {
            yMomentum -= gravity * Time.deltaTime;
        }
        else {
            yMomentum = 0;
        }

        deltaVelocity = new Vector3(0, yMomentum, 0);
        rb.velocity += deltaVelocity;
    }

    void checkJump()
    {
        if (Input.GetButton("Jump"))
        {
            state = 0;
            animator.SetTrigger("Jump");
            yMomentum = jumpForce;
        }
    }
}

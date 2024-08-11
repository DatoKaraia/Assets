using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    [SerializeField] GameObject cameraController;

    public float walkSpeed = 1, runSpeed = 3, distToGround = .5f;
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
            }
        }

        //Idle
        if (state == 1)
        {
            if (xMomentum >= .2f)
            {
                state = 2;
            }
        }

        //Walking
        if (state == 2)
        {
            animator.SetFloat("SpeedMult", xMomentum);
            if (xMomentum < .2f)
            {
                state = 1;
                animator.SetFloat("SpeedMult", 1);
            }
            
            else if (Input.GetButton("Run"))
            {
                state = 3;
            }

            else
            {
                speed = walkSpeed;
            }
        }

        //Running
        if (state == 3)
        {
            if (!Input.GetButton("Run"))
            {
                state = 2;
            }

            else
            {
                speed = runSpeed;
            }
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

        rb.velocity = new Vector3(Mathf.Sin((yAngle + yAngleCamera) * Mathf.Deg2Rad) * xMomentum * speed, yMomentum, Mathf.Cos((yAngle + yAngleCamera) * Mathf.Deg2Rad) * xMomentum * speed);


        deltaVelocity = new Vector3(0, yMomentum, 0);
        rb.velocity += deltaVelocity;

        Debug.Log(speed);
    }
}

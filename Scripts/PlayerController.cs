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

    public float speed = 1;
    public float gravity = 4;

    [NonSerialized] public float xMomentum, yMomentum, yAngle, yAngleCamera;
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
        isGrounded = Physics.SphereCast(transform.position, .5f, Vector3.down, out groundRaycast, 1.05f, LayerMask.GetMask("Default"));

        xMomentum = Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical"))) * speed;

        yAngleCamera = cameraController.transform.rotation.eulerAngles.y;
        yAngle = (Mathf.Rad2Deg * Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        transform.rotation = Quaternion.Euler(0f, yAngle + yAngleCamera, 0f);

        /* Uses a State Machine like code to switch between walking, jumping, idle etc.
         * 
         * States:
         * 0:Falling, 1:Idle, 2:Walking
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
        }



        animator.SetInteger("State", state);
    }

    void FixedUpdate()
    {
        if (state == 0)
        {
            yMomentum -= gravity * Time.deltaTime;
        }
        else {
            yMomentum = 0;
        }

        deltaVelocity = new Vector3(0,yMomentum,0);
        rb.velocity += deltaVelocity;
    }
}

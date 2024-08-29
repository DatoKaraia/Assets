using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMove : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float jumpForce = 5.0f;
    public float lookSpeed = 10.0f;
    public Camera playerCamera;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // We handle rotation manually
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        LookAtMouse();
        Jump();

        // Update animator parameters
        float speed = rb.velocity.magnitude;
        float horizontalInput = Input.GetAxis("Horizontal");

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("Turn", horizontalInput);

        HandleTurning(horizontalInput);
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Check if the player is running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Set the speed based on whether the player is running or walking
        float moveSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= moveSpeed;

        Vector3 velocity = rb.velocity;
        velocity.x = move.x;
        velocity.z = move.z;
        rb.velocity = velocity;

        // Update the animator with the running state
        animator.SetBool("IsRunning", isRunning);
    }

    void LookAtMouse()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);
            Vector3 lookDirection = pointToLook - transform.position;
            lookDirection.y = 0; // Keep the player level with the ground

            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleTurning(float horizontalInput)
    {
        // Example of handling turn animations
        if (horizontalInput < -0.1f)
        {
            // Trigger the LeftTurn animation
            animator.SetTrigger("LeftTurn");
        }
        else if (horizontalInput > 0.1f)
        {
            // Trigger the RightTurn animation
            animator.SetTrigger("RightTurn");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}

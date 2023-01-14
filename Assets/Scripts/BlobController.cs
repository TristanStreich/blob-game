using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobController : MonoBehaviour
{
    public static event Action JumpingEvent;
    public float jumpHoldDuration = 10_000f;
    public float moveSpeed;
    public float jumpForce = 4f;


    private bool isGrounded;
    private bool isHoldingJump;
    private float jumpStartTime;


    private Rigidbody rb;
    
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per render frame
    void Update()
    {
        horizontalMovementSystem();
    }

    // called once per physics frame
    void FixedUpdate() {
        HandleJumpingInputs();
    }

    void horizontalMovementSystem() {
        // Get input from the horizontal and vertical axes
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate the movement vector based on the input and move speed
        Vector3 moveDirection = new Vector3(horizontalInput, 0, 0) * moveSpeed;

        // Set the horizontal velocity based on the movement vector
        transform.position += moveDirection * Time.deltaTime;
    }

    void setVertVelocity(float speed) {
        rb.velocity = speed * Vector3.up;
    }

    public void HandleJumpingInputs() {
        // Do not accept jumping inputs if not in the grounded state
        // if (!isGrounded) return;
        // If the jump button was just pressed start jump timer
        if (Input.GetButtonDown("Jump")) {
            isHoldingJump = true;
            jumpStartTime = Time.time;
            setVertVelocity(jumpForce);
        }
        // if not holding jump the rest of this function is useless
        if (!isHoldingJump) return;

        setVertVelocity(jumpForce);

        // calculate how long it was held
        float elapsedTime = Time.time - jumpStartTime;

        // if jump is too long or we have released the button, then we stop applying the jump force
        if (elapsedTime > jumpHoldDuration || Input.GetButtonUp("Jump")) {
            isHoldingJump = false;
        }
    }
}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobController : MonoBehaviour
{
    public static event Action JumpingEvent;
    public float minJumpForce = 10f;
    public float maxJumpForce = 20f;
    public float jumpHoldDuration = 10_000f;
    public float moveSpeed;


    private bool isGrounded;
    private bool isHoldingJump;
    private float jumpStartTime;


    private Rigidbody rb;
    
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleJumpingInputs();
        horizontalMovementSystem();
    }

    void horizontalMovementSystem() {
        // Get input from the horizontal and vertical axes
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate the movement vector based on the input and move speed
        Vector3 moveDirection = new Vector3(horizontalInput, 0, 0) * moveSpeed;

        // Set the horizontal velocity based on the movement vector
        transform.position += moveDirection * Time.deltaTime;
    }

    void jump(float jumpForce) {
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        JumpingEvent?.Invoke();
    }

    public void HandleJumpingInputs() {
        // Do not accept jumping inputs if not in the grounded state
        // if (!isGrounded) return;
        // If the jump button was just pressed start jump timer
        if (Input.GetButtonDown("Jump")) {
            isHoldingJump = true;
            jumpStartTime = Time.time;
        }
        // If the jump button is released
        if (Input.GetButtonUp("Jump") && isHoldingJump) {
            // calculate how long it was held
            float elapsedTime = Time.time - jumpStartTime;
            Debug.Log("Held Button For " + elapsedTime + " Seconds");
            // apply jump force scaled by hold time
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, elapsedTime / jumpHoldDuration);
            jump(jumpForce);
            
            // no longer on ground
            // SetGrounded(false);
        }
    }
}

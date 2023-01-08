using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VerticalScrollController : MonoBehaviour
{
    public static event Action JumpingEvent;
    public static float jumpForce;

    public float blobJumpForce;
    public float moveSpeed;

    private Rigidbody rb;
    
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        jumpForce = blobJumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        jumpingSystem();
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

    void jumpingSystem() {
        if (Input.GetButtonDown("Jump")) {
            jump();
        }
    }

    void jump() {
        JumpingEvent?.Invoke();
    }
}

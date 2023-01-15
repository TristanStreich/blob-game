using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlobController : MonoBehaviour
{
    public static event Action JumpingEvent; //TODO: probably remove this
    public float jumpHoldDuration = 10_000f;
    public float moveSpeed;
    public float jumpForce = 4f;


    private bool isGrounded;
    private bool isHoldingJump;
    private float jumpStartTime;
    private int groundLayer;


    private Rigidbody rb;

     private static List<GameObject> GetObjectsInLayer(int layer)
 {
     List<GameObject> ret = new List<GameObject>();
     foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
     {
        Transform t = g.transform;
         if (t.gameObject.layer == layer)
         {
             ret.Add (t.gameObject);
         }
     }
     return ret;        
 }
    
    void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        int groundLayerNum = LayerMask.NameToLayer("Ground");
        // layer mask is bit mask of length 32.
        // to select just layer 3. You need a 32bit int
        // with a one on only position 3. Hence the bit shift below
        groundLayer = 1<<groundLayerNum;
    }

    // Update is called once per render frame
    void Update()
    {
        HorizontalMovementSystem();
        CheckGroundedSystem();
    }

    // called once per physics frame
    void FixedUpdate() {
        JumpingSystem();
        FastFallGravitySystem();
    }

    void HorizontalMovementSystem() {
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

    public void JumpingSystem() {
        // If the jump button was just pressed start jump timer
        // only works on ground
        if (isGrounded && Input.GetButtonDown("Jump")) {
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

    public float gravityScale = 9.81f;
    public float fallingGravityScale = 40f;
    // Use Our own custom gravity system with increased gravity on fall down
    // This system automatically turns off if the rigid body has `Use Gravity` turned on
    void FastFallGravitySystem() {
        // exit if regular unity gravity is turned on
        if (rb.useGravity) return;

        // set gravity for this frame
        float thisFrameGravityScale;
        if (rb.velocity.y >= 0) {
            // if going up, use small gravity
            thisFrameGravityScale = gravityScale;
        } else {
            // if going down use high gravity
            thisFrameGravityScale = fallingGravityScale;
        }

        // apply gravity which we just set
        rb.velocity -= thisFrameGravityScale * Time.deltaTime * Vector3.up;
    }

    public float groundingRadius = 1f;
    void CheckGroundedSystem() {
        bool groundedNow = Physics.OverlapSphere(transform.position, groundingRadius, groundLayer).Length > 0;
        // if (groundedNow != isGrounded) Debug.Log("Set Grounded To: " + groundedNow);
        isGrounded = groundedNow;
    }
}

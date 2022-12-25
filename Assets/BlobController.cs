using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float bounceDampening = 0.8f;
    public float minJumpForce = 10f;
    public float maxJumpForce = 20f;
    public float jumpHoldDuration = 10_000f;

    public float minBounceVel = 1f;

    private Rigidbody rb;
    private float maxReachedHeight;
    private float mass;
    private bool isJumping;
    private bool isGrounded;
    private float jumpStartTime;
    private GameObject Standing;
    private GameObject Jumping;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;

        if (transform.Find("Standing") == null || transform.Find("Jumping") == null)
        {  
        Debug.LogError("Game object is missing required children with specific names.");
        }

        Transform childTransform = transform.Find("Standing");
        Standing = childTransform.gameObject;
        childTransform = transform.Find("Jumping");
        Jumping = childTransform.gameObject;

        unGround();

    }

    void Update()
    {
        // Get input from the horizontal and vertical axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement vector based on the input and move speed
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed;

        // Set the horizontal velocity based on the movement vector
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Update the maximum reached height
        maxReachedHeight = Mathf.Max(maxReachedHeight, transform.position.y);

        if (!isGrounded) {return;}

      // Check if the jump button is being held down
        if (Input.GetButton("Jump"))
        {
            // If the jump button was just pressed, set the jump start time
            if (!isJumping)
            {
                isJumping = true;
                jumpStartTime = Time.time;
            }
        }
        // If the jump button is released, reset the jump start time and isJumping flag
        else
        {
            if (isJumping) {
                float elapsedTime = Time.time - jumpStartTime;
                Debug.Log(elapsedTime);
                float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, elapsedTime / jumpHoldDuration);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isJumping = false;
                unGround();
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        // Calculate the bounce force based on the maximum reached height
        float potentialEnergy = - mass * Physics.gravity.y * maxReachedHeight;
        float bounceVel = Mathf.Sqrt(2 * (potentialEnergy) / mass) * bounceDampening;
        if (bounceVel < minBounceVel) {
            bounceVel = 0f;
            ground();
        }
        float scaledBounceForce = bounceVel * mass;

        // If the blob collides with something, add a bounce force to its velocity
        rb.velocity += Vector3.up * scaledBounceForce;

        // Reset the maximum reached height
        maxReachedHeight = 0f;
    }


    public void ground() {
        isGrounded = true;
        Jumping.GetComponent<Renderer>().enabled = false;
        Standing.GetComponent<Renderer>().enabled = true;
    }

    public void unGround() {
        isGrounded = false;
        Jumping.GetComponent<Renderer>().enabled = true;
        Standing.GetComponent<Renderer>().enabled = false;
    }
}


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

    public float minBounceSpeed = 1f;

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

        float bouncePosition = GetAverageYPosition(collision);
        // Calculate the bounce force based on the maximum reached height
        float potentialEnergy = - mass * Physics.gravity.y * (maxReachedHeight - bouncePosition);
        float kineticEnergy = potentialEnergy * bounceDampening;

        float bounceSpeed = Mathf.Sqrt(2 * kineticEnergy / mass);
        if (bounceSpeed < minBounceSpeed) {
            bounceSpeed = 0f;
            ground();
        }

        // If the blob collides with something, add a bounce velocity to its velocity
        rb.velocity = new Vector3(rb.velocity.x, bounceSpeed, rb.velocity.z);

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

    float GetAverageYPosition(Collision collision) {
        float sum = 0f;
        foreach (ContactPoint contact in collision.contacts)
        {
            sum += contact.point.y;
        }
        return sum / collision.contacts.Length;
    }

}


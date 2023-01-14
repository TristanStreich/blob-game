using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ArchiveBlobController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float bounceDampening = 0.8f;
    public float minJumpForce = 10f;
    public float maxJumpForce = 20f;
    public float jumpHoldDuration = 10_000f;
    public float minBounceSpeed = 1f;
    public float GroundedHeightThreshold = 1f;
    public float CayoteTime = 0.1f;
    public float jumpSquashSize = 0.5f;

    private Rigidbody rb;
    private float maxReachedHeight;
    private float mass;
    private bool isGrounded;
    private bool isHoldingJump;
    private float jumpStartTime;
    private GameObject Standing;
    private Renderer StandingRenderer;
    private GameObject Jumping;
    private Renderer JumpingRenderer;
    private float CayoteDeadline;
    private Vector3 MaxSquashScale;
    private Vector3 OriginalScale;
    private SphereCollider collider;
    private Vector3 OriginalColliderPostion;
    private Vector3 MaxSquashColliderPosition;
    private ParticleSystem particleSystem;
    private bool playedParticles;

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
        StandingRenderer = Standing.GetComponent<Renderer>();
        childTransform = transform.Find("Jumping");
        Jumping = childTransform.gameObject;
        JumpingRenderer = Jumping.GetComponent<Renderer>();

        particleSystem = GetComponent<ParticleSystem>();

        // calculate max squash scale so we can lerp to this while holding jump
        MaxSquashScale = transform.localScale;
        OriginalScale = transform.localScale;
        MaxSquashScale.y *= jumpSquashSize;

        collider = GetComponent<SphereCollider>();
        OriginalColliderPostion = collider.center;
        MaxSquashColliderPosition = new Vector3(OriginalColliderPostion.x,
                                                 OriginalColliderPostion.y + collider.radius*2 * jumpSquashSize,
                                                 OriginalColliderPostion.z);

        SetGrounded(false);
    }

    void Update()
    {
        // move blob
        HandleDirectionalInputs();

        // Update the maximum reached height
        maxReachedHeight = Mathf.Max(maxReachedHeight, transform.position.y);

        // accept space bar inputs if grounded
        HandleJumpingInputs();
        SquashWhileJumping();

        // Sets state to ungrounded if off the ground for too long
        AboveGroundHandler();
    }

    void OnCollisionEnter(Collision collision)
    {

        float bouncePosition = collision.AveragePosition().y;
        float heightDifference = maxReachedHeight - bouncePosition;
        // Calculate the bounce speed based on the heightDifference
        float potentialEnergy = - mass * Physics.gravity.y * (heightDifference);
        float kineticEnergy = potentialEnergy * bounceDampening;
        float bounceSpeed = Mathf.Sqrt(2 * kineticEnergy / mass);
        // stick to ground if bounce will be small
        if (bounceSpeed < minBounceSpeed) {
            bounceSpeed = 0f;
            SetGrounded(true);
        }

        // remove NaNs
        if (float.IsNaN(bounceSpeed)) bounceSpeed = 0f;

        // If the blob collides with something, add a bounce velocity to its velocity
        rb.velocity = new Vector3(rb.velocity.x, bounceSpeed, rb.velocity.z);

        // Reset the maximum reached height
        maxReachedHeight = 0f;
    }


    public void SetGrounded(bool grounded) {
        isGrounded = grounded;
        JumpingRenderer.enabled = !grounded;
        StandingRenderer.enabled = grounded;
        // If we are no longer on the ground. Then reset the isHoldingJump flag
        if (!grounded) isHoldingJump = false;
        // Unsquash the scale
        transform.localScale = OriginalScale;
        collider.center = OriginalColliderPostion;
    }

    // moves the blob based on wasd or arrow keys
    public void HandleDirectionalInputs() {
        // Get input from the horizontal and vertical axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement vector based on the input and move speed
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed;

        // Set the horizontal velocity based on the movement vector
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    public void HandleJumpingInputs() {
        // Do not accept jumping inputs if not in the grounded state
        if (!isGrounded) return;
        // If the jump button was just pressed start jump timer
        if (Input.GetButtonDown("Jump")) {
            isHoldingJump = true;
            playedParticles = false;
            jumpStartTime = Time.time;
        }
        // If the jump button is released
        if (Input.GetButtonUp("Jump") && isHoldingJump) {
            // calculate how long it was held
            float elapsedTime = Time.time - jumpStartTime;
            Debug.Log("Held Button For " + elapsedTime + " Seconds");
            // apply jump force scaled by hold time
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, elapsedTime / jumpHoldDuration);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            // no longer on ground
            SetGrounded(false);
        }
    }

    public void SquashWhileJumping() {
        if (!isHoldingJump) return;

        float elapsedTime = Time.time - jumpStartTime;

        // Clamp the lerp value to the range [0, 1]
        float t = Mathf.Clamp01(elapsedTime / jumpHoldDuration);
        // squash the transform and offset collider
        transform.localScale = Vector3.Lerp(OriginalScale, MaxSquashScale, t);
        collider.center = Vector3.Lerp(OriginalColliderPostion, MaxSquashColliderPosition, t);

        //play particle effect if this is the first frame reaching max squash.
        if (t == 1 && !playedParticles) {
            particleSystem.Play();
            playedParticles = true;
        }
    }

    // handles the case that we are in the air but are still in the grounded state.
    // start a timer and if off the ground for too long, then set state to ungrounded
    public void AboveGroundHandler() {
        // if already in the non grounded state, we do not need to do any checks
        if (!isGrounded) return;

        // Check if on ground
        if (transform.HeightAboveGround() <= GroundedHeightThreshold) {
            // reset deadline
            CayoteDeadline = float.NaN;
            return;
        }
        // after this if, we are off the ground

        // If this is first frame off ground then cayote deadline will be NaN
        if (float.IsNaN(CayoteDeadline)) {
            // set deadline
            CayoteDeadline = Time.time + CayoteTime;
            return;
        }
        // if deadline is not reached then do nothing
        if (Time.time < CayoteDeadline) return;

        // here we have been off the ground for too long. Unground
        CayoteDeadline = float.NaN;
        SetGrounded(false);
    }
}


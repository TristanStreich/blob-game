using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveDownOnBlobJump : MonoBehaviour
{
    float jumpForce;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpForce = VerticalScrollController.jumpForce;
        VerticalScrollController.JumpingEvent += MoveDownOnJump;
    }

    void MoveDownOnJump() {
        rb.velocity += new Vector3(0f,-jumpForce,0f);
    }
}

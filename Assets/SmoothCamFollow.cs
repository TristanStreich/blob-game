using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public Vector3 offsetLookAt;

    public float CamMoveSpeed = 1.0f;
    public float CamRotSpeed = 1.0f;

    private Vector3 camMoveVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Smoothly interpolate the position of the camera towards the target position
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref camMoveVelocity, CamMoveSpeed);

        // Smoothly interpolate the rotation of the camera towards the desired rotation
        Quaternion desiredRotation = Quaternion.LookRotation(target.position + offsetLookAt - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, CamRotSpeed);
    }
}

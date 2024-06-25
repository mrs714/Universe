using System;
using UnityEngine;

public class ShipController : PhysicalObject
{

    [Header("Settings")]
    [Range(1, 100)]
    public float moveSpeed = 10f;
    [Range(10, 200)]
    public float rotationSpeed = 100f;
    [Range(1, 10)]
    public float mouseSensitivity = 2f;

    [Header("Camera settings")]
    public Vector3 cameraOffset = new Vector3(0, 2, -5);
    [NonSerialized]
    public Transform cameraTransform;

    new void Start()
    {
        base.Start();
        cameraTransform = Camera.main.transform;
    }

    new void Update()
    {
        base.Update();
        HandleMovement();
        HandleRotation();
    }

    void LateUpdate()
    {
        FollowCamera();
    }

    void HandleMovement()
    {
        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float moveZ = 0;

        if (Input.GetKey(KeyCode.Q)) moveZ = -1;
        if (Input.GetKey(KeyCode.E)) moveZ = 1;

        // Create movement vector
        Vector3 move = transform.right * moveX + transform.up * moveZ + transform.forward * moveY;
        move *= moveSpeed * Time.deltaTime;

        // Apply movement
        rb.MovePosition(transform.position + move);
    }

    void HandleRotation()
    {
        /*
        // Get input for rotation
        float rotateX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Apply rotation
        transform.Rotate(Vector3.up, rotateX * rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, rotateY * rotationSpeed * Time.deltaTime);
        */
        // Get the normal of the strongest force applied to the object
        Vector3 normal = physicsSimulation.GetStrongestNormal(this);
        // Make the ship upright
        transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        
    }

    void FollowCamera()
    {
        cameraTransform.position = transform.position + cameraOffset;
        cameraTransform.LookAt(transform);
    }
}



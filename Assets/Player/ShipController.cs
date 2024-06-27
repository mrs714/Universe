using System;
using UnityEditor.EditorTools;
using UnityEngine;

public class ShipController : PhysicalObject
{

    [Header("Settings")]
    [Range(1, 1000)]
    public float moveSpeed = 10f;
    [Range(10, 200)]
    public float rotationSpeed = 100f;
    [Range(1, 10)]
    public float mouseSensitivity = 2f;

    [Header("Camera settings")]
    public Vector3 cameraOffset = new Vector3(0, 2, -5);
    [NonSerialized]
    public Transform cameraTransform;
    [Tooltip("How many frames until the scene is centered around the player")]
    public int centeringInterval = 100;

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
        HandleCentering();
        FollowCamera();
    }

    void HandleMovement()
    {
        // Get input for movement
        float moveX = 0;
        float moveY = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W)) moveY = 1;
        if (Input.GetKey(KeyCode.S)) moveY = -1;
        if (Input.GetKey(KeyCode.A)) moveX = -1;
        if (Input.GetKey(KeyCode.D)) moveX = 1;
        if (Input.GetKey(KeyCode.LeftShift)) moveZ = 1;
        if (Input.GetKey(KeyCode.LeftControl)) moveZ = -1;

        // Create movement vector
        Vector3 move = transform.right * moveX + transform.up * moveZ + transform.forward * moveY;
        move *= moveSpeed * Time.deltaTime;
        
        // Calculate the force applied to the object
        force = move * mass;
        rb.AddForce(force);
    }

    void HandleRotation()
    {
        
        // Get input for rotation
        float rotateX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float rotateZ = 0;
        if (Input.GetKey(KeyCode.Q)) rotateZ = 1;
        if (Input.GetKey(KeyCode.E)) rotateZ = -1;

        // Apply rotation
        transform.Rotate(Vector3.up, rotateX * rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, rotateY * rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward, rotateZ * rotationSpeed * Time.deltaTime);
        
        // Get the normal of the strongest force applied to the object
        PhysicsSimulation.StrongestNormalResult strongestNormal = physicsSimulation.GetStrongestNormal(this);

        // Make the ship upright
        transform.rotation = Quaternion.FromToRotation(transform.up, strongestNormal.Normal) * transform.rotation;
        
    }

    void FollowCamera()
    {
         // Calculate the new camera position based on the player's position and rotation
        Vector3 desiredPosition = transform.position + transform.TransformDirection(cameraOffset);
        cameraTransform.position = desiredPosition;

        // Make the camera look at the player
        cameraTransform.LookAt(transform.position + transform.TransformDirection(Vector3.forward * 2));
    
    }

    // Move all the objects in the scene so that the player is in the center of the scene
    void HandleCentering()
    {
        // every 100 frames
        if (Time.frameCount % centeringInterval != 0) return;
        // Calculate the offset between the player and the center of the scene
        PhysicalObject[] physicalObjects = physicsSimulation.GetPhysicalObjects();
        Vector3 offset = this.position;
        
        // Move all the objects in the scene so that the player is in the center of the scene
        for (int i = 0; i < physicalObjects.Length; i++)
        {
            physicalObjects[i].MoveTo(physicalObjects[i].position - offset);
        }
    }
}



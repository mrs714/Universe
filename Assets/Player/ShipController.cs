using System;
using Unity.Mathematics;
using UnityEditor.EditorTools;
using UnityEngine;

public class ShipController : SmallPhysicalObject
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

    public int centeringInterval = 100; // remove

    // Get the children object named Door
    [SerializeField]
    private float doorSpeed = 0.25f;
    private GameObject door;
    float closedDoorX = 0;
    float openDoorX = 0;
    Boolean doorOpen = false;
    Boolean doorObjective = false;

    new void Start()
    {
        base.Start();
        cameraTransform = Camera.main.transform;
        door = transform.Find("ShipDoor").gameObject;
        closedDoorX = door.transform.localRotation.x * Mathf.Rad2Deg * 2;
        openDoorX = closedDoorX -100f;
    }

    new void Update()
    {
        base.Update();
        //HandleMovement();
        //HandleRotation();
        HandleDoor();

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
                
    }

    void OpenDoor()
    {
        doorObjective = true;
    }

    void CloseDoor()
    {
        doorObjective = false;
    }

    void HandleDoor()
    {
        if (doorObjective && !doorOpen)
        {
            if (door.transform.localRotation.x * Mathf.Rad2Deg * 2 > openDoorX)
            {
                // Rotate half degree each time until the door is open
                door.transform.Rotate(Vector3.right, -doorSpeed);

            }
            else
            {
                doorOpen = true;
            }
        }
        if (!doorObjective && doorOpen)
        {
            if (door.transform.localRotation.x * Mathf.Rad2Deg * 2 < closedDoorX)
            {
                // Rotate half degree each time until the door is closed
                door.transform.Rotate(Vector3.right, doorSpeed);
            }
            else
            {
                doorOpen = false;
            }
        }
    }
}
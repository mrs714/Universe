using System;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    [Header("Physical Object")]
    public string objectName;
    public float mass = 10f;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;

    [Header("Visuals")]
    public Boolean showPath = false;

    Rigidbody rb;
    PhysicsSimulation physicsSimulation;

    public void Start()
    {
        physicsSimulation = FindObjectsByType<PhysicsSimulation>(FindObjectsSortMode.None)[0]; // Find the physics simulation object

        position = transform.position;        
        rb = GetComponent<Rigidbody>();

        // Rigidbody setup
        rb.mass = mass;
        rb.useGravity = false;
        rb.isKinematic = false;

    }

    /*
    Updates the velocity of the rigidbody, letting the unity engine handle the movement of the object
    */
    public void MoveObject()
    {
        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = velocity;
    }

    /*
    Updates the position of the object, letting our physics engine handle the movement of the object to plot the path
    */
    public void UpdateObject()
    {
        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime;
        position += velocity * Time.fixedDeltaTime;
    }

    public void Update()
    {
        position = transform.position;
    }




    public void OnDrawGizmos()
    {
        if (showPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(position, 0.1f);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3[] objectPath = physicsSimulation.GetObjectTrajectory(this, 10000);
        
        for (int i = 0; i < objectPath.Length - 1; i++)
        {
            Gizmos.DrawLine(objectPath[i], objectPath[i + 1]);
        }
        
    }
    

}
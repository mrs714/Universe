using System;
using Unity.Mathematics;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    [Header("Physical Object")]
    public string objectName;
    public float mass = 10f;
    [NonSerialized]
    public float radius; // Used for collision prediction only, not meant to be a proper mesh collider


    public Vector3 position;
    public Vector3 velocity;
    public Vector3 force;

    [Header("Visuals")]
    public Boolean showPath = false;
    // Slider for step count
    [Range(1, 100000)]
    public int pathSteps = 10000;
    [Tooltip("The bigger the step size, the less accurate the path will be, but the faster it will be calculated")]
    [Range(1, 100)]
    public int simulationStepSize = 1;
    

    protected Rigidbody rb;
    protected PhysicsSimulation physicsSimulation;

    public void Start()
    {
        physicsSimulation = FindObjectsByType<PhysicsSimulation>(FindObjectsSortMode.None)[0]; // Find the physics simulation object

        position = transform.position;        
        radius = transform.localScale.x / 2;
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
    public void UpdateObject(int stepSize = 1)
    {
        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime * stepSize;
        position += velocity * Time.fixedDeltaTime * stepSize;
    }

    public void Update()
    {
        position = transform.position;
    }
    
    private Boolean settingChanged = true;
    private Vector3[] objectPath = null;
    private int collision = -1;
    PhysicsSimulation.StrongestNormalResult strongestNormal;
    private Vector3 distanceToStrongestNormal;
    void OnValidate()
    {
        settingChanged = true;
    }

    [ExecuteInEditMode]
    public void OnDrawGizmosSelected()
    {
        if (position != transform.position)
        {
            position = transform.position;
            settingChanged = true;
        }


        if (showPath && physicsSimulation && settingChanged)
        {
            PhysicsSimulation.TrajectoryResult trajectoryResult = physicsSimulation.GetObjectTrajectory(this, pathSteps, simulationStepSize);
            objectPath = trajectoryResult.Path;
            collision = trajectoryResult.Collision;
            
            strongestNormal = physicsSimulation.GetStrongestNormal(this);
            distanceToStrongestNormal = strongestNormal.PhysicalObject.position - position;

            settingChanged = false;
        }

        if (showPath && physicsSimulation != null && objectPath != null)
        {              
            // Draw the path of the object
            Gizmos.color = Color.blue;
        
            int predictionLength = collision != -1 ? collision : objectPath.Length - 1;

            for (int i = 0; i < predictionLength; i++)
            {
                Gizmos.DrawLine(objectPath[i], objectPath[i + 1]);
            }

            if (collision != -1)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(objectPath[collision], radius);
            }

            // Draw an arrow towards the strongest force applied to the object
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, distanceToStrongestNormal);
        }    
    }
}
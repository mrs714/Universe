using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsSimulation : MonoBehaviour
{
    /* 
    This script is used to calculate the physics of every object in the scene: 
    It does so by calculating the force of gravity between every object in the scene.
    This force is then used to apply to the rigidbody of the object.

    It also provides the following tools:
    - GetForce(PhysicalObject physicalObject): Returns the force applied to a physical object by all other objects in the scene.
    - GetObjectTrajectory(PhysicalObject physicalObject, int steps = 10): This function is used to plot the path of an object in the scene.
    - CheckCollision(PhysicalObject physicalObject, PhysicalObject otherPhysicalObject): This function checks if two objects are colliding.
    - GetStrongestNormal(PhysicalObject physicalObject): Returns the normal of the strongest force applied to the object (to get upright orientation when on the surface of a planet)
    */

    public float gravitationalConstant = 10f;
    PhysicalObject[] physicalObjects;

    void Start()
    {
        physicalObjects = FindObjectsByType<PhysicalObject>(FindObjectsSortMode.None);
    }
    void FixedUpdate()
    {
        UpdateObjects();
    }

    void UpdateObjects(Boolean move = true)
    {
        // Calculate the force of gravity between every object in the scene
        foreach (PhysicalObject physicalObject in physicalObjects)
        {
            physicalObject.force = GetForce(physicalObject);
        }
        // Move or update every object in the scene by applying the force
        foreach (PhysicalObject physicalObject in physicalObjects)
        {
            if (move) // If done for the actual simulation, move the object (using the rigidbody);
                physicalObject.MoveObject();
            else // If done to plot the path, update the object (without using the rigidbody)
                physicalObject.UpdateObject();
        }
    }

    /*
    This function calculates the force applied to a physical object by all other objects in the scene.
    */
    public Vector3 GetForce(PhysicalObject physicalObject)
    {
        Vector3 totalForce = Vector3.zero;

        foreach (PhysicalObject otherPhysicalObject in physicalObjects)
        {
            if (physicalObject != otherPhysicalObject)
            {
                Vector3 direction = otherPhysicalObject.position - physicalObject.position;
                float distance = direction.magnitude;
                Vector3 force = gravitationalConstant * physicalObject.mass * otherPhysicalObject.mass / Mathf.Pow(distance, 2) * direction.normalized;

                totalForce += force;
            }
        }
        return totalForce;
    }

    /* 
    This function is used to plot the path of an object in the scene.
    To do so, it saves the actual state of every object in the scene, and then, given a number of steps, it calculates the future state for all objects in the scene.
    Then, it reverses the state of the objects to the actual state, and returns a list of points that represent the path of the object.
    */
    public struct TrajectoryResult
    {
        public Vector3[] Path;
        public int Collision;

        public TrajectoryResult(Vector3[] path, int collision)
        {
            Path = path;
            Collision = collision;
        }
    }
    public TrajectoryResult GetObjectTrajectory(PhysicalObject physicalObject, int steps = 10)
    {
        Vector3[] path = new Vector3[steps];
        Vector3[] initialPositions = new Vector3[physicalObjects.Length];
        Vector3[] initialVelocities = new Vector3[physicalObjects.Length];

        int collision = -1;

        // Find the index for the object we're plotting the path for
        int physicalObjectIndex = 0;
        foreach (PhysicalObject obj in physicalObjects)
        {
            if (obj == physicalObject)
            {
                break;
            }
            physicalObjectIndex++;
        }

        // Save the initial state of the objects
        for (int i = 0; i < physicalObjects.Length; i++)
        {
            initialPositions[i] = physicalObjects[i].position;
            initialVelocities[i] = physicalObjects[i].velocity;
        }

        // Calculate the future state of the objects
        for (int i = 0; i < steps; i++)
        {
            UpdateObjects(move: false); // Update the objects without using the rigidbody
            path[i] = physicalObjects[physicalObjectIndex].position; // Save the position of the object we're plotting the path for

            // Check for collisions
            for (int j = 0; j < physicalObjects.Length; j++)
            {
                if (j != physicalObjectIndex)
                {
                    if (CheckCollision(physicalObjects[physicalObjectIndex], physicalObjects[j]))
                    {
                        collision = i;
                        break;
                    }
                }
            }
            if (collision != -1)
            {
                break;
            }
        }

        // Reset the state of the objects
        for (int i = 0; i < physicalObjects.Length; i++)
        {
            physicalObjects[i].position = initialPositions[i];
            physicalObjects[i].velocity = initialVelocities[i];
        }



        return new TrajectoryResult(path, collision);
    }

    public Boolean CheckCollision(PhysicalObject physicalObject, PhysicalObject otherPhysicalObject)
    {
        float distance = (physicalObject.position - otherPhysicalObject.position).magnitude;
        return distance < physicalObject.radius + otherPhysicalObject.radius;
    }

    public Vector3 GetStrongestNormal(PhysicalObject physicalObject)
    {
        Vector3 strongestNormal = Vector3.zero;
        float strongestForce = 0;

        foreach (PhysicalObject otherPhysicalObject in physicalObjects)
        {
            if (physicalObject != otherPhysicalObject)
            {
                Vector3 direction = otherPhysicalObject.position - physicalObject.position;
                float distance = direction.magnitude;
                Vector3 force = gravitationalConstant * physicalObject.mass * otherPhysicalObject.mass / Mathf.Pow(distance, 2) * direction.normalized;
                if (force.magnitude > strongestForce)
                {
                    strongestForce = force.magnitude;
                    strongestNormal = force.normalized;
                }
            }
        }

        return strongestNormal;
    }
}

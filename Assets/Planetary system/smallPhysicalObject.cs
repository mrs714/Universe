using System;
using UnityEngine;

public class SmallPhysicalObject : PhysicalObject
{

    /*
    This class is used to handle the collision of a small object with a big object which is causing a strong pull towards it.
    */

    UnityEngine.Object[] CollidingObjects;

    public override void MoveObject()
    {        
        print("MoveObjectModified");
        velocity = rb.linearVelocity; 
        // Apply the force to the object
        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime;
        // If the object is in contact with a surface, the velocity in the direction of the normal is set to 0, 
        // Otherwise it causes bouncing as close to a big object, it gets a high velocity in the direction of the normal every frame
        // This causes the object to clip, and the unity engine makes the object bounce back

        // We will assume that the object causing the collision is the one with a strongest gravitational pull
        print("velocity: " + velocity);
        velocity = HandleVelocityOnCollision(velocity);
        print("velocity after: " + velocity);
        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision) // Get the objects that the object is colliding with
    {
        UnityEngine.ContactPoint[] CollidingPoints = collision.contacts;
        for (int i = 0; i < CollidingPoints.Length; i++)
        {
            UnityEngine.Object obj = CollidingPoints[i].otherCollider.gameObject.GetComponent<PhysicalObject>();
            if (obj != null)
            {
                CollidingObjects = new UnityEngine.Object[] { obj };
            }
        }
    }

    private void OnCollisionExit(Collision collision) // Clear the colliding objects when the object is no longer in contact with the surface
    {
        CollidingObjects = null;
    }

    private Vector3 HandleVelocityOnCollision(Vector3 velocity)
    {
        if (CollidingObjects != null)
        {
            PhysicsSimulation.StrongestNormalResult strongestNormal = physicsSimulation.GetStrongestNormal(this);

            foreach (UnityEngine.Object obj in CollidingObjects)
            {
                if (obj != null)
                {
                    PhysicalObject physicalObject = (PhysicalObject)obj;

                    if (physicalObject == strongestNormal.PhysicalObject)
                    {
                        Vector3 distanceToStrongestNormal = strongestNormal.PhysicalObject.position - position;
                        Vector3 force = strongestNormal.Normal * strongestNormal.Force;
                        return velocity - Vector3.Project(velocity, force);
                    }
                }
            }
        }
        return velocity;
    }

}
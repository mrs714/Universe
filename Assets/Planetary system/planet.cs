using System;
using UnityEngine;

public class Planet : PhysicalObject
{
    [Range(1, 100)]
    [Tooltip("This overrides the mass of the object, calculating it based on the radius and the expected gravity on the surface")]
    public float gravityStrength = 10f;

    new void Start()
    {
        base.Start();
          
        radius = transform.localScale.x / 2;

        mass = gravityStrength * Mathf.Pow(radius, 2) / physicsSimulation.gravitationalConstant;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}

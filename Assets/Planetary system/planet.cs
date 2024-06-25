using System;
using UnityEngine;

public class Planet : PhysicalObject
{
    new void Start()
    {
        base.Start();
          
        radius = transform.localScale.x / 2;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}

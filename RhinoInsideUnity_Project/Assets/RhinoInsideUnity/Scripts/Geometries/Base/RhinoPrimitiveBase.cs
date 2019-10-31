using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhinoPrimitiveBase : MonoBehaviour
{
    public Vector3 center;
    
    public virtual void OnEnable()
    { }
    
    public virtual void Update()
    {
        center = transform.position;
    }
}

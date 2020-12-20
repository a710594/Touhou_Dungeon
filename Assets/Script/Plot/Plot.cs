using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot
{
    public enum CheckState
    {
        NotSatisfy,
        Satisfy,
        Completed,
    }


    public virtual void Start() { }
    public virtual void Start(Action callback) { }
}

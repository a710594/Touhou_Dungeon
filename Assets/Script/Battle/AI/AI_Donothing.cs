using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Donothing : AI
{
    public override void StartAI(Action callback)
    {
        CanHitTarget = false;
        callback();
    }
}

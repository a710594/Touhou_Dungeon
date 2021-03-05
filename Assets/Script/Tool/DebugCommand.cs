using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommand
{
    public static void Start()
    {
        TeamManager.Instance.AddMember(5, true, new Vector2Int(0, -1), 41005, 42003);
        TeamManager.Instance.SetPower(50);
    }
}

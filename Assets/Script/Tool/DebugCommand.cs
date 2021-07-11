using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommand
{
    public static void Start()
    {
        //TeamManager.Instance.AddMember(4, true, new Vector2Int(0, 1), 41005, 42003);
        //TeamManager.Instance.AddMember(5, true, new Vector2Int(0, -1), 41005, 42003);
        TeamManager.Instance.SetPower(0);
        TeamManager.Instance.SetLv(13, 0);
        //TeamManager.Instance.SetEquip(0, 41005, 42003);
        //TeamManager.Instance.SetEquip(1, 41006, 42003);
        //TeamManager.Instance.SetEquip(2, 41006, 42003);
        //TeamManager.Instance.SetEquip(3, 41005, 42003);
        //TeamManager.Instance.SetEquip(4, 41007, 42004);
        ExploreController.Instance.ArriveFloor = 13;
        TeamManager.Instance.RemoveMember(4);
    }
}

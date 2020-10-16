using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_7 : Plot
{
    public override void Start()
    {
        ExploreController.Instance.ArriveFloor = 7;
        TeamManager.Instance.AddMember(4, new Vector2Int(0, 1), 41003, 42002);
        ProgressManager.Instance.Memo.Youmu_Flag = true;
        GameSystem.Instance.SaveGame();
        Debug.Log("妖夢入隊");
    }
}

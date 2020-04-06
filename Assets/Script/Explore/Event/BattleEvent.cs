using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvent : EventResult
{
    public BattleEvent(EventOptionData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        ExploreController.Instance.EnterBattle(_result.Value);
        Time.timeScale = 1;
    }
}

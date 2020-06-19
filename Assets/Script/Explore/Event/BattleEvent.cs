using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvent : EventResult
{
    private Timer _timer = new Timer();

    public BattleEvent(EventData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        _timer.Start(0.5f, ()=> 
        {
            ExploreController.Instance.ForceEnterBattle();
            Time.timeScale = 1;
        });
    }
}

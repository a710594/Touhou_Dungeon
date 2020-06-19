using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyEvent : EventResult
{
    public MoneyEvent(EventData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        ItemManager.Instance.AddMoney(ExploreController.Instance.ArriveFloor * 100);
    }
}

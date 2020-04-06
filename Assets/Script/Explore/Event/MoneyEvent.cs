using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyEvent : EventResult
{
    public MoneyEvent(EventOptionData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        ItemManager.Instance.AddMoney(_result.Value);
    }
}

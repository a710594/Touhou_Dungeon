using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverEvent : EventResult
{
    public RecoverEvent(EventData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            TeamManager.Instance.MemberList[i].AddHP((int)(TeamManager.Instance.MemberList[i].MaxHP));
            TeamManager.Instance.MemberList[i].AddMP((int)(TeamManager.Instance.MemberList[i].MaxMP));
        }
        ItemManager.Instance.MinusMoney((int)(ItemManager.Instance.Money * 0.1f));
    }
}

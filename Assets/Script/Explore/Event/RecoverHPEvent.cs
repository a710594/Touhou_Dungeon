using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverHPEvent : EventResult
{
    public RecoverHPEvent(EventOptionData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        for (int i=0; i<TeamManager.Instance.MemberList.Count; i++)
        {
            TeamManager.Instance.MemberList[i].AddHP((int)(TeamManager.Instance.MemberList[i].MaxHP * ((float)_result.Value / 100.0f)));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverMPEvent : EventResult
{
    public RecoverMPEvent(EventOptionData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            TeamManager.Instance.MemberList[i].CurrentMP += (int)(TeamManager.Instance.MemberList[i].MaxMP * ((float)_result.Value / 100.0f));
            Debug.Log(TeamManager.Instance.MemberList[i].Data.GetName() + " " + (int)(TeamManager.Instance.MemberList[i].MaxMP * ((float)_result.Value / 100.0f)));
        }
    }
}

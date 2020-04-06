﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverBothEvent : EventResult
{
    public RecoverBothEvent(EventOptionData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            TeamManager.Instance.MemberList[i].CurrentHP += (int)(TeamManager.Instance.MemberList[i].MaxHP * ((float)_result.Value / 100.0f));
            TeamManager.Instance.MemberList[i].CurrentMP += (int)(TeamManager.Instance.MemberList[i].MaxMP * ((float)_result.Value / 100.0f));
        }
    }
}
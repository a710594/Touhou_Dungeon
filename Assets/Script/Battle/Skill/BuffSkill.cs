using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkill : Skill
{
    public BuffSkill(SkillData.RootObject data)
    {
        Data = data;
    }

    public override void Use(BattleCharacter executor, Action callback)
    {
        base.Use(executor, callback);

        for (int i = 0; i < _targetList.Count; i++)
        {
            SetEffect(_targetList[i]);
        }

        if (_targetList.Count == 0)
        {
            OnSkillEnd();
        }
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        target.SetBuff(Data.StatusID, CheckSkillCallback);
    }
}

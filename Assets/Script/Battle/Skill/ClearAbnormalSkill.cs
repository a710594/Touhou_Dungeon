﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAbnormalSkill : Skill
{
    public ClearAbnormalSkill(SkillData.RootObject data)
    {
        Data = data;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData);
        }
    }

    public override void Use(BattleCharacter executor, Action callback)
    {
        base.UseCallback();

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

        target.ClearAbnormal(CheckSkillCallback);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSkill : Skill
{
    public FieldSkill(SkillData.RootObject data)
    {
        Data = data;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData);
        }
    }

    protected override void UseCallback()
    {
        base.UseCallback();

        SetEffect(null);
        BattleUI.Instance.SetFloatingNumber(_executor, Data.GetComment(), FloatingNumber.Type.Other, _skillCallback);
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            BattleFieldManager.Instance.MapDic[_skillRangeList[i]].SetBuff(Data.StatusID);
        }
    }
}

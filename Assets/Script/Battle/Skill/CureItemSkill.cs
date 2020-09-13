using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureItemSkill : Skill
{
    public CureItemSkill(SkillData.RootObject data, int lv)
    {
        Data = data;
        Lv = lv;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, null, lv);
        }
    }

    protected override void UseCallback()
    {
        base.UseCallback();

        for (int i = 0; i < _targetList.Count; i++)
        {
            SetEffect(_targetList[i]);
        }

        if (_targetList.Count == 0)
        {
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        }
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        target.SetRecoverHP(_value, CheckSkillCallback); //與 CureSkill 不同的地方之一是回復量的計算
    }
}

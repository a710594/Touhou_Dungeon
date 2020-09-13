using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureSkill : Skill
{
    public CureSkill() { }

    public CureSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
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

        target.SetRecoverHP(CalculateRecover(_user), CheckSkillCallback);
    }

    public int CalculateRecover(BattleCharacterInfo executor)
    {
        return (int)((float)executor.MEF * (float)_value / 100f);
    }
}

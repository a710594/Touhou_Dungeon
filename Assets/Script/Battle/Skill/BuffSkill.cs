using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkill : Skill
{
    public BuffSkill(SkillData.RootObject data, BattleCharacterInfo user)
    {
        _user = user;
        Data = data;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user);
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

        if (Data.Target == SkillData.TargetType.Us) //目標為我方則必中
        {
            hitType = HitType.Hit;
        }

        target.SetBuff(Data.StatusID, hitType, CheckSkillCallback);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSkill : Skill
{
    public FieldSkill(SkillData.RootObject data, BattleCharacterInfo user)
    {
        Data = data;
        _user = user;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user);
        }
    }

    protected override void UseCallback()
    {
        base.UseCallback();

        SetEffect(null);
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            BattleFieldManager.Instance.MapDic[_skillRangeList[i]].SetBuff(Data.StatusID);
        }

        //BattleUI.Instance.SetFloatingNumber(_user, Data.GetComment(), FloatingNumber.Type.Other, () =>
        //{
        //    BattleUI.Instance.SetSkillLabel(false);
        //    _skillCallback();
        //});
        BattleUI.Instance.SetSkillLabel(false);
        CheckSkillCallback(target);
    }
}

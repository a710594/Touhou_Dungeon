using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonothingSkill : Skill
{
    public DonothingSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffects()
    {
        SetEffect(null);
    }

    public override void SetEffect(BattleCharacter target)
    {
        Timer timer = new Timer(Data.ShowTime / 2f, () =>
        {
            //BattleUI.Instance.SetFloatingNumber(target, Data.Name, FloatingNumber.Type.Other);
            CheckSubSkill(target, HitType.Hit);
        });
    }
}

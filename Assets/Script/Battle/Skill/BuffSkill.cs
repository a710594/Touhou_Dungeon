using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkill : Skill
{
    public BuffSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        _user = user;
        _hasNoTarget = false;
        Data = data;
        Lv = lv;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
        HitType hitType = CheckHit(_user, target.Info, target.LiveState);

        if (Data.Target == SkillData.TargetType.Us) //目標為我方則必中
        {
            hitType = HitType.Hit;
        }

        if (hitType != HitType.Miss)
        {
            target.SetBuff(Data.StatusID, Lv);
        }

        string text = "";
        FloatingNumber.Type floatingNumberType = FloatingNumber.Type.Other;

        if (hitType == HitType.Hit)
        {
            floatingNumberType = FloatingNumber.Type.Other;
            text = BattleStatusData.GetData(Data.StatusID).Message;
        }
        else if (hitType == HitType.Miss)
        {
            floatingNumberType = FloatingNumber.Type.Miss;
            text = "Miss";
        }

        _floatingNumberDic = floatingNumberDic;
        SetFloatingNumberDic(target, floatingNumberType, text);

        CheckSubSkill(target, hitType);
    }
}

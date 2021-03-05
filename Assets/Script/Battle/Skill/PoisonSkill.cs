using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSkill : Skill
{
    private Poison _poison;

    public PoisonSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _hasNoTarget = false;
        _poison = new Poison(Data.StatusID, lv);
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

        if (hitType != HitType.Miss)
        {
            target.Info.SetPoison(_poison, CalculateDamage(target.Info));
        }

        string text = "";
        FloatingNumber.Type floatingNumberType = FloatingNumber.Type.Other;

        if (hitType == HitType.Hit)
        {
            floatingNumberType = FloatingNumber.Type.Poison;
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

    private int CalculateDamage(BattleCharacterInfo target)
    {
        return (int)(_poison.Damage / 100f * (float)target.MaxHP);
    }

    protected override HitType CheckHit(BattleCharacterInfo executor, BattleCharacterInfo target, BattleCharacter.LiveStateEnum targetLiveState)
    {
        HitType hitType = base.CheckHit(executor, target, targetLiveState);

        if (hitType == HitType.Hit)
        {
            if (target.AbnormalRecorder.Contains(BattleCharacterInfo.AbnormalEnum.Poison))
            {
                return HitType.Miss;
            }
            else
            {
                return HitType.Hit;
            }
        }
        else
        {
            return hitType;
        }
    }
}

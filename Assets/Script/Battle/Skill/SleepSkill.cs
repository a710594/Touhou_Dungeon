using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSkill : Skill
{
    public SleepSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _hasNoTarget = false;
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
            target.SetSleep(Data.StatusID);
        }

        string text = "";
        FloatingNumber.Type floatingNumberType = FloatingNumber.Type.Other;

        if (hitType == HitType.Hit)
        {
            floatingNumberType = FloatingNumber.Type.Sleeping;
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

    protected override HitType CheckHit(BattleCharacterInfo executor, BattleCharacterInfo target, BattleCharacter.LiveStateEnum targetLiveState)
    {
        HitType hitType = base.CheckHit(executor, target, targetLiveState);

        if (hitType == HitType.Hit)
        {
            if (target.AbnormalRecorder.Contains(BattleCharacterInfo.AbnormalEnum.Sleep))
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

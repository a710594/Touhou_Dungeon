using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikingSkill : Skill
{
    public StrikingSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
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

        if (Data.Target == SkillData.TargetType.Us) //目標為我方則必中
        {
            hitType = HitType.Hit;
        }

        if (hitType != HitType.Miss)
        {
            target.SetStriking(Data.StatusID);
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

        //Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        //{
        //    if (hitType != Skill.HitType.Miss)
        //    {
        //        target.SetStriking(Data.StatusID);

        //        BattleUI.Instance.SetFloatingNumber(target, BattleStatusData.GetData(Data.StatusID).Message, FloatingNumber.Type.Other);
        //    }
        //    else
        //    {
        //        BattleUI.Instance.SetFloatingNumber(target, "Miss", FloatingNumber.Type.Miss);
        //    }
        //});

        //Timer timer2 = new Timer(_floatingNumberTime + Data.ShowTime / 2f, () =>
        //{
        //    CheckSubSkill(target, hitType);
        //});
    }
}

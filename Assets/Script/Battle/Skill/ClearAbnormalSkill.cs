using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAbnormalSkill : Skill
{
    public ClearAbnormalSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
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
        target.ClearAbnormal();

        _floatingNumberDic = floatingNumberDic;
        SetFloatingNumberDic(target, FloatingNumber.Type.Other, "解除異常狀態");

        CheckSubSkill(target, HitType.Hit);

        //Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        //{
        //    target.ClearAbnormal();
        //    BattleUI.Instance.SetFloatingNumber(target, "解除異常狀態", FloatingNumber.Type.Other);
        //});

        //Timer timer2 = new Timer(Data.ShowTime / 2f + _floatingNumberTime, () =>
        //{
        //    CheckSubSkill(target, HitType.Hit);
        //});
    }
}

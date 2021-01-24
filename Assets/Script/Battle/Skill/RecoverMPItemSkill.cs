using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverMPItemSkill : Skill
{
    public RecoverMPItemSkill(SkillData.RootObject data, int lv)
    {
        _hasNoTarget = false;
        Data = data;
        Lv = lv;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, null, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
        target.SetRecoverMP(_value);

        _floatingNumberDic = floatingNumberDic;
        SetFloatingNumberDic(target, FloatingNumber.Type.Other, "解除異常狀態");

        CheckSubSkill(target, HitType.Hit);

        //Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        //{
        //    target.SetRecoverMP(_value);
        //    BattleUI.Instance.SetFloatingNumber(target, _value.ToString(), FloatingNumber.Type.Recover);
        //});

        //Timer timer2 = new Timer(_floatingNumberTime + Data.ShowTime / 2f, () =>
        //{
        //    CheckSubSkill(target, HitType.Hit);
        //});
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureItemSkill : Skill
{
    public CureItemSkill(SkillData.RootObject data, int lv)
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
        target.SetRecoverHP(_value); //與 CureSkill 不同的地方之一是回復量的計算

        _floatingNumberDic = floatingNumberDic;
        SetFloatingNumberDic(target, FloatingNumber.Type.Other, "解除異常狀態");

        CheckSubSkill(target, HitType.Hit);

        //Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        //{
        //    target.SetRecoverHP(_value); //與 CureSkill 不同的地方之一是回復量的計算

        //    BattleUI.Instance.SetFloatingNumber(target, _value.ToString(), FloatingNumber.Type.Recover);
        //    BattleUI.Instance.SetLittleHPBar(target, true);
        //});

        //Timer timer2 = new Timer(Data.ShowTime / 2f + _floatingNumberTime, () =>
        //{
        //    CheckSubSkill(target, HitType.Hit);
        //});
    }
}

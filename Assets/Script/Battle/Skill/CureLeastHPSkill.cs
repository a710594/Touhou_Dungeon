﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureLeastHPSkill : Skill
{
    public CureLeastHPSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _hasNoTarget = false;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
        int minHP = int.MaxValue;
        BattleCharacter character;
        List<BattleCharacter> chaarcterList = new List<BattleCharacter>();
        for (int i = 0; i < BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            //治療我方 HP 最少的角色
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead && character.Info.Camp == _user.Camp && character.Info.CurrentHP < minHP)
            {
                target = character;
                minHP = target.Info.CurrentHP;
            }
        }

        int recover = CalculateRecover(_user);

        target.SetRecoverHP(recover);

        _floatingNumberDic = floatingNumberDic;
        SetFloatingNumberDic(target, FloatingNumber.Type.Recover, recover.ToString());

        CheckSubSkill(target, HitType.Hit);

        //Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        //{
        //    target.SetRecoverHP(recover);
        //    BattleUI.Instance.SetFloatingNumber(target, recover.ToString(), FloatingNumber.Type.Recover);
        //    BattleUI.Instance.SetLittleHPBar(target, true);
        //});

        //Timer timer2 = new Timer(Data.ShowTime / 2f + _floatingNumberTime, () =>
        //{
        //    CheckSubSkill(target, HitType.Hit);
        //});
    }

    public int CalculateRecover(BattleCharacterInfo executor)
    {
        return (int)((float)executor.MEF / 100f * _value);
    }
}

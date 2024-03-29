﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSkill : Skill
{
    public FieldSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _hasNoTarget = true;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            BattleFieldManager.Instance.MapDic[_skillRangeList[i]].SetBuff(Data.StatusID, Lv);
        }

        BattleUI.Instance.SetSkillLabel(false);

        //Timer timer = new Timer(Data.ShowTime / 2f + _floatingNumberTime, ()=> 
        //{
        //    CheckSubSkill(target, HitType.Hit);
        //});

        CheckSubSkill(target, HitType.Hit);
    }

    public void SetSkillRange(List<Vector2Int> list) //for subskill
    {
        _skillRangeList = list;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSkill : Skill
{
    public FieldSkill(SkillData.RootObject data)
    {
        Data = data;
    }

    public override void Use(BattleCharacter executor, Action callback)
    {
        base.Use(executor, callback);

        SetEffect(null);
        OnSkillEnd();
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            BattleFieldManager.Instance.MapDic[_skillRangeList[i]].SetBuff(Data.StatusID);
        }
    }
}

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
        _poison = new Poison(Data.StatusID, lv);
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
        }
    }

    protected override void UseCallback()
    {
        base.UseCallback();

        for (int i = 0; i < _targetList.Count; i++)
        {
            SetEffect(_targetList[i]);
        }

        if (_targetList.Count == 0)
        {
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        }
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        target.SetPoison(_poison, CalculateDamage(target.Info), hitType, CheckSkillCallback);
    }

    private int CalculateDamage(BattleCharacterInfo target)
    {
        return (int)(_poison.Damage / 100f * (float)target.MaxHP);
    }
}

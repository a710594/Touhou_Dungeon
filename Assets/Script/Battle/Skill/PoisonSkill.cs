using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSkill : Skill
{
    public PoisonSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
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

        target.SetPoison(Data.StatusID, CalculateDamage(target.Info), Lv, hitType, CheckSkillCallback);
    }

    private int CalculateDamage(BattleCharacterInfo target)
    {
        float poisonDamage = BattleStatusData.GetData(Data.StatusID).ValueList[Lv];
        return (int)(poisonDamage / 100f * (float)target.MaxHP);
    }
}

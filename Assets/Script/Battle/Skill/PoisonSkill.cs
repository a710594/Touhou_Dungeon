using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSkill : Skill
{
    public PoisonSkill(SkillData.RootObject data)
    {
        Data = data;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData);
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

        target.SetPoison(Data.StatusID, CalculateDamage(target), CheckSkillCallback);
    }

    private int CalculateDamage(BattleCharacter target)
    {
        float poisonDamage = BattleStatusData.GetData(Data.StatusID).Damage;
        return (int)(poisonDamage / 100f * (float)target.MaxHP);
    }
}

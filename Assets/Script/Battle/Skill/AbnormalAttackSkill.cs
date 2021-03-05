using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbnormalAttackSkill : AttackSkill
{
    public AbnormalAttackSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _hasNoTarget = false;
        _value = data.ValueList[lv - 1];
        _isMagic = data.IsMagic;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override int CalculateDamage(BattleCharacterInfo executor, BattleCharacterInfo target, bool isCritical, bool isRandom)
    {
        float damage = base.CalculateDamage(executor, target, isCritical, isRandom);
        bool hasAbnormal = false;
        foreach (KeyValuePair<int, BattleStatus> item in target.StatusDic)
        {
            if (item.Value is Poison || item.Value is Paralysis || item.Value is Sleeping)
            {
                hasAbnormal = true;
                break;
            }
        }
        if (hasAbnormal)
        {
            damage *= 2;
        }

        return Mathf.RoundToInt(damage);
    }
}

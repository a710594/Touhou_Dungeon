using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageAttackSkill : AttackSkill
{
    public RageAttackSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
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
        damage *= 1 + ((float)_user.LastTurnGetDamage / (float)_user.MaxHP);

        return Mathf.RoundToInt(damage);
    }
}

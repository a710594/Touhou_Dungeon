using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : Skill
{
    public AttackSkill(SkillData.RootObject data, BattleCharacterInfo user)
    {
        Data = data;
        _user = user;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user);
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

        int damage = 0;
        if (hitType == HitType.Critical)
        {
            damage = CalculateDamage(_user, target.Info, true);
        }
        else if (hitType == HitType.Hit)
        {
            damage = CalculateDamage(_user, target.Info, false);
        }

        target.SetDamage(damage, hitType, CheckSkillCallback);
    }

    public int CalculateDamage(BattleCharacterInfo executor, BattleCharacterInfo target, bool isCritical)
    {
        float damage;
        if (Data.IsMagic)
        {
            damage = (float)executor.MTK / (float)target.MEF;
            damage = damage * Data.Value * (1 + (executor.Lv - 1) * 0.1f) + executor.EquipMTK - target.EquipMEF;
        }
        else
        {
            damage = (float)executor.ATK / (float)target.DEF;
            damage = damage * Data.Value * (1 + (executor.Lv - 1) * 0.1f) + executor.EquipATK - target.EquipDEF;
        }


        if (damage < 1)
        {
            damage = 1;
        }

        if (isCritical)
        {
            damage = (int)(damage * 1.5f);
            Debug.Log("爆擊");
        }
        if (target.IsSleeping)
        {
            damage = (int)(damage * 2f);
        }
        damage = (int)(damage * (UnityEngine.Random.Range(100f, 110f) / 100f)); //加上10%的隨機傷害

        return Mathf.RoundToInt(damage);
    }
}

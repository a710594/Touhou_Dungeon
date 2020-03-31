using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : Skill
{
    public enum HitType
    {
        Miss,
        Hit,
        Critical
    }

    public AttackSkill(SkillData.RootObject data)
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
            OnSkillEnd();
        }
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        int damage = 0;
        HitType hitType = CheckHit(_executor, target);
        if (hitType == HitType.Critical)
        {
            damage = CalculateDamage(_executor, target, true);
        }
        else if (hitType == HitType.Hit)
        {
            damage = CalculateDamage(_executor, target, false);
        }

        target.SetDamage(damage, hitType, CheckSkillCallback);
    }

    private int CalculateDamage(BattleCharacter executor, BattleCharacter target, bool isCritical)
    {
        int damage;
        if (Data.IsMagic)
        {
            damage = Mathf.RoundToInt(((float)executor.MTK / (float)target.MEF) * Data.Damage);
        }
        else
        {
            damage = Mathf.RoundToInt(((float)executor.ATK / (float)target.DEF) * Data.Damage);
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

        return damage;
    }

    protected HitType CheckHit(BattleCharacter executor, BattleCharacter target)
    {
        float misssRate;
        misssRate = (float)(target.AGI - executor.SEN) / (float)target.AGI; //迴避率

        if (misssRate >= 0) //迴避率為正,骰迴避
        {
            if (misssRate < UnityEngine.Random.Range(0f, 1f))
            {
                return HitType.Hit;
            }
            else
            {
                return HitType.Miss;
            }
        }
        else //迴避率為負,骰爆擊
        {
            if (misssRate < UnityEngine.Random.Range(0f, 1f) * -1f)
            {
                return HitType.Critical;
            }
            else
            {
                return HitType.Hit;
            }
        }
    }
}

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
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        }
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        int damage = 0;
        HitType hitType = CheckHit(_executor.Info, target.Info, target.LiveState);
        if (hitType == HitType.Critical)
        {
            damage = CalculateDamage(_executor.Info, target.Info, true);
        }
        else if (hitType == HitType.Hit)
        {
            damage = CalculateDamage(_executor.Info, target.Info, false);
        }

        target.SetDamage(damage, hitType, CheckSkillCallback);
    }

    public int CalculateDamage(BattleCharacterInfo executor, BattleCharacterInfo target, bool isCritical)
    {
        float damage;
        if (Data.IsMagic)
        {
            damage = (float)executor.MTK / (float)target.MEF;
            damage = damage * Data.Damage * (1 + (executor.Lv - 1) * 0.1f) + executor.EquipMTK - target.EquipMEF;
        }
        else
        {
            damage = (float)executor.ATK / (float)target.DEF;
            damage = damage * Data.Damage * (1 + (executor.Lv - 1) * 0.1f) + executor.EquipATK - target.EquipDEF;
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

    protected HitType CheckHit(BattleCharacterInfo executor, BattleCharacterInfo target, BattleCharacter.LiveStateEnum targetLiveState)
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
                if (targetLiveState == BattleCharacter.LiveStateEnum.Dying)
                {
                    return HitType.Hit;
                }
                else
                {
                    return HitType.Miss;
                }
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : Skill
{
    protected bool _isMagic;

    public AttackSkill() { }

    public AttackSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
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

    public AttackSkill(bool isMagic, int damage) //計算機用的
    {
        _value = damage;
        _isMagic = isMagic;
    }

    public override void SetEffects()
    {
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
        HitType hitType = CheckHit(_user, target.Info, target.LiveState);

        int damage = 0;
        if (hitType == HitType.Critical)
        {
            damage = CalculateDamage(_user, target.Info, true, true);
        }
        else if (hitType == HitType.Hit)
        {
            damage = CalculateDamage(_user, target.Info, false, true);
        }

        string text = "";
        FloatingNumber.Type floatingNumberType = FloatingNumber.Type.Other;
        if (hitType == Skill.HitType.Critical)
        {
            floatingNumberType = FloatingNumber.Type.Critical;
            text = damage.ToString();
        }
        else if (hitType == Skill.HitType.Hit)
        {
            floatingNumberType = FloatingNumber.Type.Damage;
            text = damage.ToString();
        }
        else if (hitType == Skill.HitType.Miss)
        {
            floatingNumberType = FloatingNumber.Type.Miss;
            text = "Miss";
        }
        else if (hitType == Skill.HitType.NoDamage)
        {
            floatingNumberType = FloatingNumber.Type.Miss;
            text = "NoDamage";
        }

        Timer timer1 = new Timer(Data.ShowTime / 2f, ()=> 
        {
            target.SetDamage(damage);
            BattleUI.Instance.SetFloatingNumber(target, text, floatingNumberType);
            BattleUI.Instance.SetLittleHPBar(target, true);
        });

        Timer timer2 = new Timer(Data.ShowTime / 2f + _floatingNumberTime, () =>
        {
            target.CheckLiveState();
            CheckSubSkill(target, hitType);
        });
    }

    public virtual int CalculateDamage(BattleCharacterInfo executor, BattleCharacterInfo target, bool isCritical, bool isRandom)
    {
        float damage;
        if (_isMagic)
        {
            damage = (float)executor.MTK / (float)target.MEF / 10f;
            damage = damage * _value * (1 + (executor.Lv - 1) * 0.1f) + executor.EquipMTK - target.EquipMEF;
        }
        else
        {
            damage = (float)executor.ATK / (float)target.DEF / 10f;
            damage = damage * _value * (1 + (executor.Lv - 1) * 0.1f) + executor.EquipATK - target.EquipDEF;
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

        if (isRandom)
        {
            damage = (int)(damage * (UnityEngine.Random.Range(100f, 110f) / 100f)); //加上10%的隨機傷害
        }

        return Mathf.RoundToInt(damage);
    }

    protected override HitType CheckHit(BattleCharacterInfo executor, BattleCharacterInfo target, BattleCharacter.LiveStateEnum targetLiveState)
    {
        HitType hitType = base.CheckHit(executor, target, targetLiveState);
        if (BattleFieldManager.Instance.IsNoDamageField(target.Position))
        {
            return HitType.NoDamage;
        }

        if (hitType == HitType.Hit)
        {
            if (target.EnemyData != null && target.EnemyData.ID == 7) //新手教學的那隻怪不會被爆擊
            {
                return HitType.Hit;
            }
            else
            {
                float originalSEN = 0; //與等級無關的原始數值A
                if (_user.JobData != null)
                {
                    originalSEN = _user.JobData.SEN;
                }
                else if (_user.EnemyData != null)
                {
                    originalSEN = _user.EnemyData.SEN;
                }
                float criticalRate = (originalSEN - 10) * (Data.HitRate / 100f) * 0.02f;
                if (criticalRate > UnityEngine.Random.Range(0f, 1f))
                {
                    return HitType.Critical;
                }
                else
                {
                    return HitType.Hit;
                }
            }
        }
        else 
        {
            return hitType;
        }

        //float misssRate;
        //misssRate = (float)(target.AGI - executor.SEN * (Data.HitRate / 100f)) / (float)target.AGI; //迴避率

        //if (misssRate >= 0) //迴避率為正,骰迴避
        //{
        //    if (misssRate < UnityEngine.Random.Range(0f, 1f))
        //    {
        //        return HitType.Hit;
        //    }
        //    else
        //    {
        //        if (targetLiveState == BattleCharacter.LiveStateEnum.Dying)
        //        {
        //            return HitType.Hit;
        //        }
        //        else
        //        {
        //            return HitType.Miss;
        //        }
        //    }
        //}
        //else //迴避率為負,骰爆擊
        //{
        //    if (target.EnemyData != null && target.EnemyData.ID == 7) //新手教學的那隻怪不會被爆擊
        //    {
        //        return HitType.Hit;
        //    }
        //    else
        //    {
        //        float criticalRate = (_user.SEN - 10) * (Data.HitRate / 100f) *0.02f;
        //        if (criticalRate > UnityEngine.Random.Range(0f, 1f))
        //        {
        //            return HitType.Critical;
        //        }
        //        else
        //        {
        //            return HitType.Hit;
        //        }
        //    }
        //    //else if (misssRate < UnityEngine.Random.Range(0f, 1f) * -1f)
        //    //{
        //    //    return HitType.Critical;
        //    //}
        //    //else
        //    //{
        //    //    return HitType.Hit;
        //    //}
        //}
    }
}

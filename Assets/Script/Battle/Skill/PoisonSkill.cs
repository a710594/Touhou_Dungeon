using System;
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
            _subSkill.SetPartnerSkill(this);
        }
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

        Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        {
            if (hitType != Skill.HitType.Miss)
            {
                target.Info.SetPoison(_poison, CalculateDamage(target.Info));

                BattleUI.Instance.SetFloatingNumber(target, _poison.Data.Message, FloatingNumber.Type.Other);
            }
            else
            {
                BattleUI.Instance.SetFloatingNumber(target, "Miss", FloatingNumber.Type.Miss);
            }
        });

        Timer timer2 = new Timer(_floatingNumberTime + Data.ShowTime / 2f, () =>
        {
            CheckSubSkill(target, hitType);
        });
    }

    private int CalculateDamage(BattleCharacterInfo target)
    {
        return (int)(_poison.Damage / 100f * (float)target.MaxHP);
    }
}

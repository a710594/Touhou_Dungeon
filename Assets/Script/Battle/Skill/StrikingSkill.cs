using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikingSkill : Skill
{
    public StrikingSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
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
        base.SetEffect(target);

        Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        {
            if (hitType != Skill.HitType.Miss)
            {
                target.SetStriking(Data.StatusID);

                BattleUI.Instance.SetFloatingNumber(target, BattleStatusData.GetData(Data.StatusID).Message, FloatingNumber.Type.Other);
            }
            else
            {
                BattleUI.Instance.SetFloatingNumber(target, "Miss", FloatingNumber.Type.Miss);
            }
        });

        Timer timer2 = new Timer(_floatingNumberTime + Data.ShowTime / 2f, () =>
        {
            CheckSubSkill(target);
        });
    }
}

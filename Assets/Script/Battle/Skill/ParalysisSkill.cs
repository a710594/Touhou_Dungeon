using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalysisSkill : Skill
{
    public ParalysisSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
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
        HitType hitType = CheckHit(_user, target.Info, target.LiveState);

        Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        {
            if (hitType != HitType.Miss)
            {
                target.SetParalysis(Data.StatusID, Lv);

                BattleUI.Instance.SetFloatingNumber(target, BattleStatusData.GetData(Data.StatusID).Message, FloatingNumber.Type.Other);
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
}

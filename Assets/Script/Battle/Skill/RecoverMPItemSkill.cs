using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverMPItemSkill : Skill
{
    public RecoverMPItemSkill(SkillData.RootObject data, int lv)
    {
        Data = data;
        Lv = lv;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, null, lv);
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
        Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        {
            target.SetRecoverMP(_value);
            BattleUI.Instance.SetFloatingNumber(target, _value.ToString(), FloatingNumber.Type.Recover);
        });

        Timer timer2 = new Timer(_floatingNumberTime + Data.ShowTime / 2f, () =>
        {
            CheckSubSkill(target, HitType.Hit);
        });
    }
}

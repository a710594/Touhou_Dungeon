using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonothingSkill : Skill
{
    private Timer _timer = new Timer();

    public DonothingSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
        }
    }

    protected override void UseCallback()
    {
        base.UseCallback();

        SetEffect(null);
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        _timer.Start(0.5f, () =>
        {
            CheckSkillCallback(target);
        });
    }
}

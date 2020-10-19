using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkill : Skill
{
    public SummonSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
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
        SetEffect(null);
    }

    public override void SetEffect(BattleCharacter target)
    {
        Timer timer1 = new Timer(Data.ShowTime / 2f, () =>
        {
            BattleCharacter character;
            for (int i = 0; i < _skillRangeList.Count; i++)
            {
                character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
                character.Init(Data.Summon, _user.Lv);
                character.SetPosition(_skillRangeList[i]);
                BattleController.Instance.AddCharacer(character);
            }

            BattleUI.Instance.SetSkillLabel(false);
        });

        Timer timer2 = new Timer(_floatingNumberTime + Data.ShowTime / 2f, () =>
        {
            CheckSubSkill(target, HitType.Hit);
        });
    }
}

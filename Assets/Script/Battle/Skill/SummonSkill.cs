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

        BattleCharacter character;
        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.Init(Data.Summon, _user.Lv);
            character.SetPosition(_skillRangeList[i]);
            BattleController.Instance.AddCharacer(character);
        }

        BattleUI.Instance.SetSkillLabel(false);
        CheckSkillCallback(target);
    }
}

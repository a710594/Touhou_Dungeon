using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureLeastHPSkill : Skill
{
    public CureLeastHPSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
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

        int minHP = int.MaxValue;
        BattleCharacter character;
        List<BattleCharacter> chaarcterList = new List<BattleCharacter>();
        for (int i = 0; i < BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            //治療我方 HP 最少的角色
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead && character.Info.Camp == _user.Camp && character.Info.CurrentHP < minHP)
            {
                target = character;
                minHP = target.Info.CurrentHP;
            }
        }
        target.SetRecover(CalculateRecover(_user), CheckSkillCallback);
    }

    public int CalculateRecover(BattleCharacterInfo executor)
    {
        return (int)((float)executor.MEF / 100f * _value);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonothingSkill : Skill
{
    public DonothingSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _hasNoTarget = true;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
        _floatingNumberDic = floatingNumberDic;
        BattleCharacter myself = BattleController.Instance.GetCharacterByPosition(_user.Position);
        SetFloatingNumberDic(myself, FloatingNumber.Type.Other, Data.Name);

        CheckSubSkill(target, HitType.Hit);

        //Timer timer = new Timer(Data.ShowTime / 2f, () =>
        //{
        //    BattleCharacter myself = BattleController.Instance.GetCharacterByPosition(_user.Position);
        //    BattleUI.Instance.SetFloatingNumber(myself, Data.Name, FloatingNumber.Type.Other);
        //    CheckSubSkill(target, HitType.Hit);
        //});
    }
}

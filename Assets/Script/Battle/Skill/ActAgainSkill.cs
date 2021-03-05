using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActAgainSkill : Skill
{
    public ActAgainSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        _user = user;
        _hasNoTarget = false;
        Data = data;
        Lv = lv;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
            _subSkill.SetPartnerSkill(this);
        }
    }

    public override void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
        HitType hitType = HitType.Hit;

        BattleController.Instance.ActAgain(target);

        string text = "";
        FloatingNumber.Type floatingNumberType = FloatingNumber.Type.Other;

        floatingNumberType = FloatingNumber.Type.Other;
        text = BattleStatusData.GetData(Data.StatusID).Message;
        _floatingNumberDic = floatingNumberDic;
        SetFloatingNumberDic(target, floatingNumberType, text);

        CheckSubSkill(target, hitType);
    }
}

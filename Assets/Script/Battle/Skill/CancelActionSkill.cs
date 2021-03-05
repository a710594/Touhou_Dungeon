using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelActionSkill : Skill
{
    public CancelActionSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
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
        HitType hitType = CheckHit(_user, target.Info, target.LiveState);

        if (hitType != HitType.Miss)
        {
            target.Info.ActionDoneCompletely();
        }

        CheckSubSkill(target, hitType);
    }
}

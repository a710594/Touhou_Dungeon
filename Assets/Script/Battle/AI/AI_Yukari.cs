using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Yukari : AI //八雲紫專用
{
    public override void StartAI(Action callback)
    {
        if (!_character.Info.HasUseSkill) //還沒用過技能
        {
            SelectSkill();

            List<Vector2Int> detectRangeList = _character.GetDetectRange();
            BattleCharacter target = GetTarget();

            if (target != null)
            {
                _character.SetTarget(Vector2Int.RoundToInt(target.transform.position));
                CanHitTarget = true;
                _character.GetSkillRange();
                callback();
            }
            else
            {
                CanHitTarget = false;
                callback();
            }
        }
        else
        {
            CanHitTarget = false;
            callback();
        }
    }

    protected override void SelectSkill()
    {
        _selectedSkill = _skillList[0];
        _character.SelectedSkill = _selectedSkill;
    }

    public BattleCharacter GetTarget()
    {
        BattleCharacter character;
        List<BattleCharacter> candidateList = new List<BattleCharacter>(); //只要是目標陣營活著的角色都算

        for (int i = 0; i < BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if (character.Info.Camp == BattleCharacterInfo.CampEnum.Partner)
                {
                    candidateList.Add(character);
                }
            }
        }

        //列車的攻擊範圍是橫的一排,挑打誰可以一次打到最多人
        int y;
        int count;
        int maxCount = 0;
        BattleCharacter target = null;
        for (int i=0; i<candidateList.Count; i++)
        {
            y = (int)candidateList[i].transform.position.y;
            count = 0;
            for (int j=0; j<candidateList.Count; j++)
            {
                if (candidateList[j].transform.position.y == y)
                {
                    count++;
                }
            }
            if (count > maxCount)
            {
                maxCount = count;
                target = candidateList[i];
            }
        }

        return target;
    }
}
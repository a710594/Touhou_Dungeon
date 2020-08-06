using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//縫隙專用的AI
public class AI_Gap : AI
{
    public override void StartAI(Action callback)
    {
        if (!_character.Info.HasUseSkill && BattleController.Instance.Turn % 2 == 1)
        {
            SelectSkill();
            List<Vector2Int> positionList = _selectedSkill.GetDistance(_character);
            for (int i=0; i<positionList.Count; i++)
            {
                if (BattleController.Instance.GetCharacterByPosition(positionList[i]) != null)
                {
                    positionList.RemoveAt(i);
                    i--;
                }
            }

            if (positionList.Count > 0)
            {
                _character.SetTarget(positionList[UnityEngine.Random.Range(0, positionList.Count)]);
                _character.GetSkillRange();
                CanHitTarget = true;
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//縫隙專用的AI
public class AI_Gap : AI
{
    public override void StartAI(Action callback)
    {
        if (!_myself.Info.HasUseSkill && BattleController.Instance.Turn % 3 == 1)
        {
            _selectedSkill = _skillList[0];
            _myself.SelectedSkill = _selectedSkill; 

            List<Vector2Int> positionList = _selectedSkill.GetDistance(_myself);
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
                _myself.SetTarget(positionList[UnityEngine.Random.Range(0, positionList.Count)]);
                Vector2Int target;
                List<Vector2Int> rangeList;
                _myself.GetSkillRange(out target, out rangeList);
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
}

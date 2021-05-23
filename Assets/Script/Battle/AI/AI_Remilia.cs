using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Remilia : AI
{
    private int _startTurn_1 = -1; //戰鬥開始的第一回合
    private int _startTurn_2 = -1; //剩下最後一條血的回合

    public override void StartAI(Action callback)
    {
        if (_startTurn_1 == -1)
        {
            _startTurn_1 = BattleController.Instance.Turn;
        }

        if (!_myself.Info.HasUseSkill) //還沒用過技能
        {
            BattleCharacter target;
            if (_myself.Info.HPQueue.Count > 0)
            {
                _selectedSkill = _skillList[0];
                target = GetTarget();
            }
            else
            {
                if (_startTurn_2 == -1)
                {
                    _startTurn_2 = BattleController.Instance.Turn;
                }

                if (BattleController.Instance.Turn - _startTurn_2 > 0)
                {
                    _selectedSkill = _skillList[1]; //符卡
                }
                else
                {
                    _selectedSkill = _skillList[2]; //宣告
                }

                target = _myself;
            }
            _myself.SelectedSkill = _selectedSkill;

            if (target != null)
            {
                _myself.SetTarget(Vector2Int.RoundToInt(target.transform.position));
                CanHitTarget = true;
                Vector2Int targetPosition;
                List<Vector2Int> rangeList;

                if (_myself.Info.HPQueue.Count > 0 && (BattleController.Instance.Turn - _startTurn_1) % 2 == 1)
                {
                    Queue<Vector2Int> shortestPath = _myself.GetPath(BattleFieldManager.Instance.GetValidPosition());
                    _myself.StartMove(shortestPath, () =>
                    {
                        _myself.MoveDone();
                        _myself.GetSkillRange(out targetPosition, out rangeList);
                        callback();
                    });
                }
                else
                {
                    _myself.GetSkillRange(out targetPosition, out rangeList);
                    callback();
                }
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

    protected BattleCharacter GetTarget()
    {
        BattleCharacter character;
        List<BattleCharacter> candidateList = new List<BattleCharacter>(); //只要是目標陣營活著的角色都算
        List<BattleCharacter> strikingList = new List<BattleCharacter>(); //符合上述條件且有注目狀態的角色

        for (int i = 0; i < BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if (character.Info.Camp == BattleCharacterInfo.CampEnum.Partner)
                {
                    candidateList.Add(character);
                    if (character.Info.IsStriking)
                    {
                        strikingList.Add(character);
                    }
                }
            }
        }

        if (strikingList.Count > 0)
        {
            candidateList = strikingList;
        }

        BattleCharacter target = null;
        if (candidateList.Count > 0)
        {
            target = candidateList[0];
            for (int i = 1; i < candidateList.Count; i++)
            {
                if ((_selectedSkill.Data.IsMagic && candidateList[i].Info.MEF < target.Info.MEF) ||
                    !_selectedSkill.Data.IsMagic && candidateList[i].Info.DEF < target.Info.DEF) //挑皮薄的
                {
                    target = candidateList[i];
                }
            }
        }

        return target;
    }
}

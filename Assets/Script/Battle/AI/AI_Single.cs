using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Single : AI
{
    public override void StartAI(Action callback)
    {
        if (!_myself.Info.HasUseSkill) //還沒用過技能
        {
            _selectedSkill = _skillList[0];
            _myself.SelectedSkill = _selectedSkill;

            Vector2Int targetPosition;
            List<Vector2Int> rangeList;
            BattleCharacter target = GetTarget();

            if (target != null)
            {
                _myself.SetTarget(Vector2Int.RoundToInt(target.transform.position));
                if (Utility.GetDistance(transform.position, target.transform.position) <= _selectedSkill.Data.Distance) //目標已在射程內,無需移動
                {
                    CanHitTarget = true;
                    _myself.GetSkillRange(out targetPosition, out rangeList);
                    callback();
                }
                else if (_myself.Info.MOV > 0)//需要移動
                {
                    Queue<Vector2Int> path;
                    List<Vector2Int> positionList = Utility.GetRhombusPositionList(_myself.SelectedSkill.Data.Distance, _myself.TargetPosition, false); //技能可打中目標的位置
                    for (int i = 0; i < positionList.Count; i++)
                    {
                        if (!_myself.InMoveRange(positionList[i])) //移除無法走到的位置
                        {
                            positionList.RemoveAt(i);
                            i--;
                        }
                    }

                    if (positionList.Count > 0) //可以打到目標
                    {
                        CanHitTarget = true;
                        //尋找最短路徑
                        //path = _myself.GetPath(positionList[0]);
                        //Queue<Vector2Int> shortestPath = path;
                        Queue<Vector2Int> shortestPath = _myself.GetPath(positionList[0]);
                        for (int i = 1; i < positionList.Count; i++)
                        {
                            path = _myself.GetPath(positionList[i]);
                            if (path.Count < shortestPath.Count)
                            {
                                shortestPath = path;
                            }
                        }

                        _myself.StartMove(shortestPath, () =>
                        {
                            _myself.MoveDone();
                            _myself.GetSkillRange(out targetPosition, out rangeList);
                            callback();
                        });
                    }
                    else //不能打到目標,但還是要盡可能接近目標
                    {
                        CanHitTarget = false;
                        List<Vector2Int> tempList = BattleFieldManager.Instance.GetPath(transform.position, _myself.TargetPosition, BattleCharacterInfo.CampEnum.None);
                        path = new Queue<Vector2Int>();
                        int pathLength = 0;
                        for (int i = 1; i < tempList.Count; i++) //擷取小於等於移動距離的路徑
                        {
                            pathLength += BattleFieldManager.Instance.GetField(tempList[i]).MoveCost;
                            if (pathLength <= _myself.Info.MOV)
                            {
                                path.Enqueue(tempList[i]);
                            }
                            else
                            {
                                break;
                            }
                        }

                        _myself.StartMove(path, () =>
                        {
                            _myself.MoveDone();
                            _myself.ActionDoneCompletely();
                            callback();
                        });
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
        else
        {
            CanHitTarget = false;
            callback();
        }
    }

    protected BattleCharacter GetTarget()
    {
        BattleCharacter character;
        List<Vector2Int> detectRangeList = _myself.GetDetectRange();
        List<BattleCharacter> candidateList = new List<BattleCharacter>(); //只要是目標陣營活著的角色都算
        List<BattleCharacter> inRangeList = new List<BattleCharacter>(); //符合上述條件且打得到的角色才算
        List<BattleCharacter> strikingList = new List<BattleCharacter>(); //符合上述條件且有注目狀態的角色

        for (int i = 0; i < BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if (character.Info.Camp == BattleCharacterInfo.CampEnum.Partner)
                {
                    candidateList.Add(character);
                    if (detectRangeList.Contains(Vector2Int.RoundToInt(character.transform.position)))
                    {
                        inRangeList.Add(character);
                        if (character.Info.IsStriking)
                        {
                            strikingList.Add(character);
                        }
                    }
                }
            }
        }

        if (strikingList.Count > 0)
        {
            candidateList = strikingList;
        }
        else if (inRangeList.Count > 0)
        {
            candidateList = inRangeList;
        }
        else //排除走不到的目標
        {
            List<Vector2Int> path;
            for (int i = 0; i < candidateList.Count; i++)
            {
                path = BattleFieldManager.Instance.GetPath(transform.position, candidateList[i].transform.position, BattleCharacterInfo.CampEnum.None);
                if (path == null)
                {
                    candidateList.RemoveAt(i);
                    i--;
                }
            }
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

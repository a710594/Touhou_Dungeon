using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [HideInInspector]
    public bool CanHitTarget = false;

    protected Skill _selectedSkill;
    protected BattleCharacter _character;
    protected List<Skill> _skillList = new List<Skill>();

    public void Init(BattleCharacter character, List<int> list)
    {
        _character = character;

        for (int i = 0; i < list.Count; i++)
        {
            _skillList.Add(SkillFactory.GetNewSkill(list[i], _character.Info, 1)); //等級填1是暫時的
        }
        _character.SelectedSkill = _skillList[0];
    }

    public virtual void StartAI(Action callback)
    {
        if (!_character.Info.HasUseSkill) //還沒用過技能
        {
            SelectSkill();

            List<Vector2Int> detectRangeList = _character.GetDetectRange();
            BattleCharacter target = GetTarget(BattleCharacterInfo.CampEnum.Partner, detectRangeList);

            if (target != null)
            {
                Debug.Log(target.Info.Name);

                _character.SetTarget(Vector2Int.RoundToInt(target.transform.position));
                if (Utility.GetDistance(transform.position, target.transform.position) <= _selectedSkill.Data.Distance) //目標已在射程內,無需移動
                {
                    CanHitTarget = true;
                    _character.GetSkillRange();
                    callback();
                }
                else if (_character.Info.MOV > 0)//需要移動
                {
                    List<Vector2Int> positionList = Utility.GetRhombusPositionList(_character.SelectedSkill.Data.Distance, _character.TargetPosition, false); //技能可打中目標的位置
                    for (int i = 0; i < positionList.Count; i++)
                    {
                        if (!_character.InMoveRange(positionList[i])) //移除無法走到的位置
                        {
                            positionList.RemoveAt(i);
                            i--;
                        }
                        else if (BattleController.Instance.GetCharacterByPosition(positionList[i]) != null) //移除有角色的位置
                        {
                            positionList.RemoveAt(i);
                            i--;
                        }
                    }

                    if (positionList.Count > 0) //可以打到目標
                    {
                        CanHitTarget = true;
                        //尋找最短路徑
                        Queue<Vector2Int> path = _character.GetPath(positionList[0]);
                        Queue<Vector2Int> shortestPath = path;
                        for (int i = 1; i < positionList.Count; i++)
                        {
                            path = _character.GetPath(positionList[i]);
                            if (path.Count < shortestPath.Count)
                            {
                                shortestPath = path;
                            }
                        }

                        _character.StartMove(shortestPath, () =>
                        {
                            _character.MoveDone();
                            _character.GetSkillRange();
                            callback();
                        });
                    }
                    else //不能打到目標,但還是要盡可能接近目標
                    {
                        CanHitTarget = false;
                        positionList = new List<Vector2Int>(_character.GetPath(_character.TargetPosition));
                        Queue<Vector2Int> path = new Queue<Vector2Int>();
                        int pathLength = 0;
                        for (int i=1; i< positionList.Count; i++) //擷取小於等於移動距離的路徑
                        {
                            pathLength += BattleFieldManager.Instance.GetField(positionList[i]).MoveCost;
                            if (pathLength <= _character.Info.MOV)
                            {
                                path.Enqueue(positionList[i]);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (path.Count > 0)
                        {
                            _character.StartMove(path, () =>
                            {
                                _character.MoveDone();
                                _character.ActionDoneCompletely();
                                callback();
                            });
                        }
                        else
                        {
                            _character.MoveDone();
                            _character.ActionDoneCompletely();
                            callback();
                        }

                        //positionList = _character.GetMoveRange(); //可能移動的位置
                        //for (int i = 0; i < positionList.Count; i++) //移除有角色的位置
                        //{
                        //    if (BattleController.Instance.GetCharacterByPosition(positionList[i]) != null)
                        //    {
                        //        positionList.RemoveAt(i);
                        //        i--;
                        //    }
                        //}

                        ////尋找離目標最近的點
                        //if (positionList.Count > 0)
                        //{
                        //    Vector2Int closestPosition = positionList[0];
                        //    for (int i = 1; i < positionList.Count; i++)
                        //    {
                        //        if (Utility.GetDistance(positionList[i], _character.TargetPosition) < Utility.GetDistance(closestPosition, _character.TargetPosition))
                        //        {
                        //            closestPosition = positionList[i];
                        //        }
                        //    }
                        //    Queue<Vector2Int> path = _character.GetPath(closestPosition);

                        //    _character.StartMove(path, () =>
                        //    {
                        //        _character.MoveDone();
                        //        _character.ActionDoneCompletely();
                        //        callback();
                        //    });
                        //}
                        //else
                        //{
                        //    _character.MoveDone();
                        //    _character.ActionDoneCompletely();
                        //    callback();
                        //}
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

    protected virtual void SelectSkill()
    {
    }

    protected virtual BattleCharacter GetTarget(BattleCharacterInfo.CampEnum targetCamp, List<Vector2Int> detectRangeList)
    {
        BattleCharacter character;
        List<BattleCharacter> candidateList = new List<BattleCharacter>(); //只要是目標陣營活著的角色都算
        List<BattleCharacter> inRangeList = new List<BattleCharacter>(); //符合上述條件且打得到的角色才算
        List<BattleCharacter> strikingList = new List<BattleCharacter>(); //符合上述條件且有注目狀態的角色

        for (int i=0; i<BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if (character.Info.Camp == targetCamp)
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

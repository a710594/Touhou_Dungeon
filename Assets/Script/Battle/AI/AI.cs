using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public bool CanHitTarget = false;

    protected Skill _selectedSkill;
    protected BattleCharacter _character;
    protected List<Skill> _skillList = new List<Skill>();

    public void Init(BattleCharacter character, List<int> list)
    {
        _character = character;

        for (int i = 0; i < list.Count; i++)
        {
            _skillList.Add(SkillFactory.GetNewSkill(list[i]));
        }
        _character.SelectedSkill = _skillList[0];
    }

    public void StartAI(Action callback)
    {
        if (!_character.Info.HasUseSkill) //還沒用過技能
        {
            SelectSkill();

            List<Vector2Int> detectRangeList = _character.GetDetectRange();
            BattleCharacter target = GetTarget(BattleCharacter.CampEnum.Partner, detectRangeList);

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
                else
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

                        //_character.StartMoveAnimation();
                        //while (shortestPath.Count > 0)
                        //{
                        //    position = shortestPath.Dequeue();
                        //    _character.Move(position);
                        //    yield return new WaitForSeconds(0.2f);
                        //}
                        //_character.StopMoveAnimation();
                        _character.StartMove(shortestPath, ()=> 
                        {
                            _character.MoveDone();
                            _character.GetSkillRange();
                            callback();
                        });
                    }
                    else //不能打到目標,但還是要盡可能接近目標
                    {
                        CanHitTarget = false;
                        positionList = _character.GetMoveRange(); //可能移動的位置
                        for (int i = 0; i < positionList.Count; i++) //移除有角色的位置
                        {
                            if (BattleController.Instance.GetCharacterByPosition(positionList[i]) != null)
                            {
                                positionList.RemoveAt(i);
                                i--;
                            }
                        }

                        //尋找離目標最近的點
                        Vector2Int closestPosition = positionList[0];
                        for (int i = 1; i < positionList.Count; i++)
                        {
                            if (Utility.GetDistance(positionList[i], _character.TargetPosition) < Utility.GetDistance(closestPosition, _character.TargetPosition))
                            {
                                closestPosition = positionList[i];
                            }
                        }
                        Queue<Vector2Int> path = _character.GetPath(closestPosition);

                        //_character.StartMoveAnimation();
                        //while (path.Count > 0)
                        //{
                        //    position = path.Dequeue();
                        //    _character.Move(position);
                        //    yield return new WaitForSeconds(0.2f);
                        //}
                        //_character.StopMoveAnimation();
                        _character.StartMove(path, ()=> 
                        {
                            _character.MoveDone();
                            _character.ActionDoneCompletely();
                            callback();
                        });
                    }
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

    protected BattleCharacter GetTarget(BattleCharacter.CampEnum targetCamp, List<Vector2Int> detectRangeList)
    {
        BattleCharacter character;
        List<BattleCharacter> candidateList = new List<BattleCharacter>(); //只要是目標陣營活著的角色都算
        List<BattleCharacter> inRangeList = new List<BattleCharacter>(); //符合上述條件且打得到的角色才算

        for (int i=0; i<BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if (character.Camp == targetCamp)
                {
                    candidateList.Add(character);
                    if (detectRangeList.Contains(Vector2Int.RoundToInt(character.transform.position)))
                    {
                        inRangeList.Add(character);
                    }
                }
            }
        }

        if (inRangeList.Count > 0)
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

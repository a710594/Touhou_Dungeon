using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int[] SkillID;

    protected BattleCharacterAI _character;
    protected List<Skill> _skillList = new List<Skill>();

    //public void Init()
    //{
    //    for (int i = 0; i < SkillID.Length; i++)
    //    {
    //        _skillList.Add(SkillFactory.GetNewSkill(SkillID[i]));
    //    }
    //}

    public void Init(BattleCharacterAI character, List<int> list)
    {
        _character = character;

        for (int i = 0; i < list.Count; i++)
        {
            _skillList.Add(SkillFactory.GetNewSkill(list[i]));
        }
        _character.SelectedSkill = _skillList[0];
    }

    public virtual void StartAI()
    {
        StartCoroutine(Run());
    }

    protected IEnumerator Run()
    {
        if (!_character.Info.HasUseSkill) //還沒用過技能
        {
            SelectSkill();

            Vector2Int position = new Vector2Int();
            List<Vector2Int> detectRangeList = _character.GetDetectRange();
            _character.Target = GetTarget(BattleCharacter.CampEnum.Partner/*, detectRangeList*/);

            if (_character.Target != null)
            {
                _character.SetTarget(Vector2Int.RoundToInt(_character.Target.transform.position));

                if (detectRangeList.Contains(Vector2Int.RoundToInt(_character.Target.transform.position))) //可以打到目標
                {
                    if (!_character.InSkillDistance())//目標不再在程內就需要移動
                    {
                        Queue<Vector2Int> path = _character.GetPath(_character.TargetPosition);

                        while (path.Count > 0 && !_character.InSkillDistance())
                        {
                            position = path.Dequeue();
                            if (!_character.InMoveRange(position))
                            {
                                break;
                            }
                            _character.Move(position);
                            yield return new WaitForSeconds(0.2f);
                        }
                        _character.MoveDone();
                    }
                    _character.GetSkillRange();
                }
                else
                {
                    Queue<Vector2Int> path = _character.GetPath(_character.TargetPosition);
                    while (path.Count > 0)
                    {
                        position = path.Dequeue();
                        if (!_character.InMoveRange(position))
                        {
                            break;
                        }
                        _character.Move(position);
                        yield return new WaitForSeconds(0.2f);
                    }
                    _character.ActionDoneCompletely();
                    _character.Target = null;
                }
            }
        }
        else
        {
            _character.Target = null;
        }

        _character.EndAI();
    }

    protected virtual void SelectSkill()
    {
    }

    protected BattleCharacter GetTarget(BattleCharacter.CampEnum targetCamp/*, List<Vector2Int> detectRangeList*/)
    {
        BattleCharacter character;
        List<BattleCharacter> candidateList = new List<BattleCharacter>();

        for (int i=0; i<BattleController.Instance.CharacterList.Count; i++)
        {
            character = BattleController.Instance.CharacterList[i];
            if (character.LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if (character.Camp == targetCamp)
                {
                    //if (detectRangeList.Contains(Vector2Int.RoundToInt(character.transform.position)))
                    //{
                        candidateList.Add(character);
                    //}
                }
            }
        }

        BattleCharacter target = null;
        if (candidateList.Count > 0)
        {
            int distance;
            int minDistance;
            target = candidateList[0];
            minDistance = Utility.GetDistance(_character.transform.position, target.transform.position);
            for (int i = 1; i < candidateList.Count; i++)
            {
                distance = Utility.GetDistance(_character.transform.position, candidateList[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = candidateList[i];
                }
            }
        }

        return target;
    }
}

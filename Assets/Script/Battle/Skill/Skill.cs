﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SkillData.RootObject Data;

    protected int _currentCD = 0;
    protected int _skillCallBackCount = 0;
    protected Action _skillCallback;
    protected BattleCharacter _executor;
    protected List<Vector2Int> _skillDistanceList = new List<Vector2Int>();
    protected List<Vector2Int> _skillRangeList = new List<Vector2Int>();
    protected List<BattleCharacter> _targetList = new List<BattleCharacter>();

    public void SetCD()
    {
        if (_currentCD > 0)
        {
            _currentCD--;
        }
    }

    public virtual bool CanUse(int currentMP, out string notUseReason)
    {
        notUseReason = "";

        if (currentMP < Data.MP)
        {
            notUseReason = "MP 不足";
            return false;
        }
        else if (_currentCD > 0)
        {
            notUseReason = "還要 " + _currentCD + " 回合才能使用";
            return false;
        }

        return true;
    }

    //有可能出現沒有任何格子可選的情況,有空要修
    public void GetSkillDistance(Vector2Int orign, BattleCharacter executor, List<BattleCharacter> characterList)
    {
        TilePainter.Instance.Clear(2);
        _skillDistanceList.Clear();

        List<Vector2Int> positionList = Utility.GetRhombusPositionList(Data.Distance, orign, false);
        positionList = RemovePosition(executor, characterList, positionList);
        positionList = BattleFieldManager.Instance.RemoveBound(positionList);

        for (int i = 0; i < positionList.Count; i++)
        {
            TilePainter.Instance.Painting("RedGrid", 2, positionList[i]);
            _skillDistanceList.Add(positionList[i]);
        }
    }

    public void GetSkillRange(Vector2Int target, Vector2Int orign, BattleCharacter executor, List<BattleCharacter> characterList)
    {
        List<Vector2Int> positionList = new List<Vector2Int>();
        TilePainter.Instance.Clear(2);
        _skillRangeList.Clear();

        TilePainter.Instance.Painting("FrontSight", 4, target);
        if (Data.RangeType == SkillData.RangeTypeEnum.Point)
        {
            positionList.Add(target);
            positionList = RemovePosition(executor, characterList, positionList);
            if (positionList.Count > 0)
            {
                _skillRangeList.Add(positionList[0]);
            }
        }
        else if (Data.RangeType == SkillData.RangeTypeEnum.Circle)
        {
            positionList = Utility.GetRhombusPositionList(Data.Range - 1, target, false);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
            for (int i = 0; i < positionList.Count; i++)
            {
                if (positionList[i] != target)
                {
                    TilePainter.Instance.Painting("RedGrid", 2, positionList[i]);
                }
                _skillRangeList.Add(positionList[i]);
            }
        }
        else if (Data.RangeType == SkillData.RangeTypeEnum.Line)
        {
            positionList = Utility.GetLinePositionList(Data.Range, orign, target - orign);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
            for (int i = 0; i < positionList.Count; i++)
            {
                if (positionList[i] != target)
                {
                    TilePainter.Instance.Painting("RedGrid", 2, positionList[i]);
                }
                _skillRangeList.Add(positionList[i]);
            }
        }
    }

    public bool IsInSkillDistance(Vector2Int position)
    {
        return _skillDistanceList.Contains(position);
    }

    public List<BattleCharacter> GetSkillTargetList()
    {
        BattleCharacter target;
        _targetList.Clear();
        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            target = BattleController.Instance.GetCharacterByPosition(_skillRangeList[i]);
            if (target != null)
            {
                _targetList.Add(target);
            }
        }
        return _targetList;
    }

    public virtual void Use(BattleCharacter executor, Action callback)
    {
        _executor = executor;
        if (Data.CD > 0)
        {
            _currentCD = Data.CD + 1; //要加一是因為本回合不減CD
        }
        _skillCallBackCount = 0;
        _skillCallback = callback;
        GetSkillTargetList();
    }

    public void SetCallback(Action callback) //給 subSkill 用的
    {
        _skillCallback = callback;
    }

    public virtual void SetEffect(BattleCharacter target)
    {

    }

    //移除非技能目標
    private List<Vector2Int> RemovePosition(BattleCharacter executor, List<BattleCharacter> characterList, List<Vector2Int> positionList)
    {
        if (Data.Target == SkillData.TargetEnum.None)
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
            }
        }
        else if (executor.Camp == BattleCharacter.CampEnum.Partner)
        {
            if (Data.Target == SkillData.TargetEnum.Us)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Enemy)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
            else if (Data.Target == SkillData.TargetEnum.Them)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Partner)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
        }
        else if (executor.Camp == BattleCharacter.CampEnum.Enemy)
        {
            if (Data.Target == SkillData.TargetEnum.Us)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Partner)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
            else if (Data.Target == SkillData.TargetEnum.Them)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Enemy)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
        }

        return positionList;
    }

    protected void CheckSkillCallback(BattleCharacter target)
    {
        if (Data.SubID != 0 && target != null)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            Skill skill = SkillFactory.GetNewSkill(skillData);

            skill.SetCallback(_skillCallback);
            skill.SetEffect(target);
        }

        _skillCallBackCount++;
        if (_skillCallBackCount == _targetList.Count)
        {
            OnSkillEnd();
        }
    }

    protected void OnSkillEnd()
    {
        if (Data.SubID == 0)
        {
            if (_skillCallback != null)
            {
                _skillCallback();
            }
        }
    }
}

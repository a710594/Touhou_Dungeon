﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SkillData.RootObject Data;
    public int CurrentCD = 0;

    protected int _skillCallBackCount = 0;
    private int _targetCount = 0;
    protected Action _skillCallback;
    protected Skill _subSkill;
    protected BattleCharacter _executor;
    protected List<Vector2Int> _skillDistanceList = new List<Vector2Int>();
    protected List<Vector2Int> _skillRangeList = new List<Vector2Int>();
    protected List<BattleCharacter> _targetList = new List<BattleCharacter>();

    public bool IsSpellCard
    {
        get
        {
            return Data.NeedPower > 0;
        }
    }
    public void SetCD()
    {
        if (CurrentCD > 0)
        {
            CurrentCD--;
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
        else if (CurrentCD > 0)
        {
            notUseReason = "還要 " + CurrentCD + " 回合才能使用";
            return false;
        }
        else if (Data.NeedPower > BattleController.Instance.Power)
        {
            notUseReason = "Power 不足";
            return false;
        }

        return true;
    }

    //有可能出現沒有任何格子可選的情況,有空要修
    public void GetDistance(BattleCharacter executor, List<BattleCharacter> characterList)
    {
        TilePainter.Instance.Clear(2);
        _skillDistanceList.Clear();

        Vector2Int orign = Vector2Int.FloorToInt(executor.transform.position);
        List<Vector2Int> positionList = Utility.GetRhombusPositionList(Data.Distance, orign, false);
        positionList = RemovePosition(executor, characterList, positionList);
        positionList = BattleFieldManager.Instance.RemoveBound(positionList);

        for (int i = 0; i < positionList.Count; i++)
        {
            TilePainter.Instance.Painting("RedGrid", 2, positionList[i]);
            _skillDistanceList.Add(positionList[i]);
        }
    }

    public void GetRange(Vector2Int target, Vector2Int orign, BattleCharacter executor, List<BattleCharacter> characterList)
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
            positionList = Utility.GetRhombusPositionList(Data.Range_1 - 1, target, false);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
            for (int i = 0; i < positionList.Count; i++)
            {
                if (positionList[i] != target)
                {
                    TilePainter.Instance.Painting("YellowGrid", 2, positionList[i]);
                }
                _skillRangeList.Add(positionList[i]);
            }
        }
        else if (Data.RangeType == SkillData.RangeTypeEnum.Rectangle)
        {
            positionList = Utility.GetRectanglePositionList(Data.Range_1, Data.Range_2, orign, target - orign);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
            for (int i = 0; i < positionList.Count; i++)
            {
                if (positionList[i] != target)
                {
                    TilePainter.Instance.Painting("YellowGrid", 2, positionList[i]);
                }
                _skillRangeList.Add(positionList[i]);
            }
        }
    }

    public bool IsInDistance(Vector2Int position)
    {
        return _skillDistanceList.Contains(position);
    }

    public List<BattleCharacter> GetTargetList()
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
        _targetCount = _targetList.Count;
        return _targetList;
    }

    public virtual void Use(BattleCharacter executor, Action callback)
    {
        _executor = executor;
        if (Data.CD > 0)
        {
            CurrentCD = Data.CD + 1; //要加一是因為本回合不減CD
        }
        InitSkillCallbackCount();
        if (_subSkill != null)
        {
            _subSkill.InitSkillCallbackCount();
            _subSkill.SetExecutor(_executor);
        }
        _skillCallback = callback;
        GetTargetList();

        if (_targetList.Count > 0 && executor.Camp == BattleCharacter.CampEnum.Partner && !IsSpellCard)
        {
            BattleController.Instance.AddPower(_targetList.Count * executor.SelectedSkill.Data.AddPower);
            BattleUI.Instance.DropPowerPoint(_targetList);
        }

        if (IsSpellCard)
        {
            BattleUI.Instance.ShowSpellCard(Data.GetName(), executor.LargeImage, UseCallback);
        }
        else
        {
            BattleUI.Instance.SetSkillLabel(true, Data.GetName());
            UseCallback();
        }
    }

    public void InitSkillCallbackCount()
    {
        _skillCallBackCount = 0;
    }

    public virtual void SetEffect(BattleCharacter target)
    {

    }

    protected virtual void UseCallback() //Use 之後會呼叫的方法
    {
        if (Data.ParticleName != "x")
        {
            GameObject particle;
            for (int i = 0; i < _skillRangeList.Count; i++)
            {
                particle = ResourceManager.Instance.Spawn("Particle/" + Data.ParticleName, ResourceManager.Type.Other);
                particle.transform.position = _skillRangeList[i] + Vector2.up; // + Vector2.up 是為了調整特效生成的位置
            }
        }
    }

    protected void CheckSkillCallback(BattleCharacter target)
    {
        if (_subSkill != null)
        {
            _subSkill.SetCallback(_targetCount, _skillCallback);
            _subSkill.SetEffect(target);
        }
        else
        {
            OnSkillEnd();
        }
    }

    protected void OnSkillEnd()
    {
        _skillCallBackCount++;
        if (_skillCallBackCount == _targetCount && _skillCallback != null)
        {
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        }
    }

    //移除非技能目標
    private List<Vector2Int> RemovePosition(BattleCharacter executor, List<BattleCharacter> characterList, List<Vector2Int> positionList)
    {
        if (Data.Target == SkillData.TargetType.None)
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
            }
        }
        else if (Data.Target == SkillData.TargetType.Us)
        {
            if (executor.Camp == BattleCharacter.CampEnum.Enemy)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Partner && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
            else if (executor.Camp == BattleCharacter.CampEnum.Partner)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Enemy && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
        }
        else if (Data.Target == SkillData.TargetType.Them)
        {
            if (executor.Camp == BattleCharacter.CampEnum.Enemy)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Enemy && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
            else if (executor.Camp == BattleCharacter.CampEnum.Partner)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Camp == BattleCharacter.CampEnum.Partner && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
        }

        return positionList;
    }

    private void SetExecutor(BattleCharacter executor) //給 subSkill 用的
    {
        _executor = executor;
    }

    private void SetCallback(int targetCount, Action callback) //給 subSkill 用的
    {
        _targetCount = targetCount;
        _skillCallback = callback;
    }
}

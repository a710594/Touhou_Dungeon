﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSkill : AttackSkill
{
    public TrainSkill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        Data = data;
        Lv = lv;
        _user = user;
        _value = data.ValueList[lv - 1];
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
        }
    }

    public override List<Vector2Int> GetRange(Vector2Int target, BattleCharacter executor, List<BattleCharacter> characterList)
    {
        _targetPosition = new Vector3(target.x, target.y, Camera.main.transform.position.z);
        List<Vector2Int> positionList = new List<Vector2Int>();
        TilePainter.Instance.Painting("FrontSight", 4, target);
        TilePainter.Instance.Clear(2);
        _skillRangeList.Clear();

        BoundsInt mapBound = BattleFieldManager.Instance.MapBound;
        for (int i= mapBound.xMin; i<=mapBound.xMax; i++) 
        {
            positionList.Add(new Vector2Int(i, target.y));
        }
        positionList = RemovePosition(executor, characterList, positionList);
        _skillRangeList = positionList;

        return _skillRangeList;
    }

    protected override void UseCallback()
    {

        GameObject particle = ResourceManager.Instance.Spawn("Particle/" + Data.ParticleName, ResourceManager.Type.Other);
        particle.transform.position = _skillRangeList[_skillRangeList.Count - 1] + Vector2.up; // + Vector2.up 是為了調整特效生成的位置

        for (int i = 0; i < _targetList.Count; i++)
        {
            SetEffect(_targetList[i]);
        }

        if (_targetList.Count == 0)
        {
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        }
    }
}
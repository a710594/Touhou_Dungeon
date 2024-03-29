﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldManager
{
    private static BattleFieldManager _instance;
    public static BattleFieldManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattleFieldManager();
            }
            return _instance;
        }
    }

    public Vector2Int Center;
    public BoundsInt MapBound;
    public List<Vector2Int> EnemyPositionList = new List<Vector2Int>();
    public Dictionary<Vector2Int, BattleField> MapDic = new Dictionary<Vector2Int, BattleField>();

    public void Init(Vector2Int center, List<Vector2Int> enemyPositionList, Dictionary<Vector2Int, BattleField> mapDic)
    {
        Center = center;
        EnemyPositionList = enemyPositionList;
        MapDic = mapDic;
        MapBound = Utility.GetMapBounds(new List<Vector2Int>(MapDic.Keys));
    }

    public List<Vector2Int> GetPath(Vector2 from, Vector2 to, BattleCharacterInfo.CampEnum camp) //camp:尋路的人是我方還是敵方
    {
        //Refresh(from, to, true);

        Dictionary<Vector2Int, int> pathFindDic = new Dictionary<Vector2Int, int>();
        foreach (KeyValuePair<Vector2Int, BattleField> item in MapDic)
        {
            if (item.Value.MoveCost != -1)
            {
                pathFindDic.Add(item.Key, item.Value.MoveCost);
            }
        }

        Vector2Int position = new Vector2Int();
        for (int i=0; i<BattleController.Instance.CharacterList.Count; i++)
        {
            if (BattleController.Instance.CharacterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                if ((camp == BattleCharacterInfo.CampEnum.Partner && BattleController.Instance.CharacterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Enemy) ||
                    (camp == BattleCharacterInfo.CampEnum.Enemy && BattleController.Instance.CharacterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Partner) ||
                    (camp == BattleCharacterInfo.CampEnum.None)) //與自己不同陣營的角色會被視為障礙物, Nono 給想要排除所有角色的人使用
                {
                    position = Vector2Int.RoundToInt(BattleController.Instance.CharacterList[i].transform.position);
                    if (position != from && position != to)
                    {
                        pathFindDic.Remove(position);
                    }
                }
            }
        }


        return AStarAlgor.Instance.GetPath(Vector2Int.RoundToInt(from), Vector2Int.RoundToInt(to), pathFindDic, false);
    }

    public int GetPathLength(Vector2 from, Vector2 to, BattleCharacterInfo.CampEnum camp)
    {
        int pathLength = 0;
        List<Vector2Int> pathList = GetPath(from, to, camp);
        if (pathList != null)
        {
            for (int i=0; i<pathList.Count; i++) 
            {
                pathLength += GetField(pathList[i]).MoveCost;
            }
        }
        else
        {
            pathLength = -1;
        }
        return pathLength;
    }

    public List<Vector2Int> RemoveBound(List<Vector2Int> positionList)
    {
        List<Vector2Int> newList = new List<Vector2Int>(positionList);
        for (int i=0; i<positionList.Count; i++)
        {
            if (positionList[i].x < MapBound.xMin || positionList[i].x > MapBound.xMax || positionList[i].y < MapBound.yMin || positionList[i].y > MapBound.yMax)
            {
                newList.Remove(positionList[i]);
            }
        }
        return newList;
    }

    public BattleField GetField(Vector2 position) 
    {
        BattleField battleField = null;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        return battleField;
    }

    public void SetField(Vector2 position, int id)
    {
        BattleField battleField = null;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        if (battleField != null)
        {
            battleField.SetData(BattleTileData.GetData(id));
        }
    }

    public float GetFieldBuff(Vector2 position, BattleStatusData.TypeEnum valueType)
    {
        BattleField battleField;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        if (battleField != null && battleField.Status is Buff)
        {
            Buff buff = (Buff)battleField.Status;
            if (buff != null && buff.Type == valueType)
            {
                return buff.Value;
            }
            else
            {
                if (valueType == BattleStatusData.TypeEnum.MOV)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
        else
        {
            if (valueType == BattleStatusData.TypeEnum.MOV)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }


    public bool IsNoDamageField(Vector2 position)
    {
        BattleField battleField;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        if (battleField != null && battleField.Status is NoDamage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector2Int GetValidPosition()
    {
        //Refresh(from, to, true);

        List<Vector2Int> positionList = new List<Vector2Int>(); 
        foreach (KeyValuePair<Vector2Int, BattleField> item in MapDic)
        {
            if (item.Value.MoveCost != -1)
            {
                positionList.Add(item.Key);
            }
        }

        Vector2Int position = new Vector2Int();
        for (int i = 0; i < BattleController.Instance.CharacterList.Count; i++)
        {
            if (BattleController.Instance.CharacterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead)
            {
                position = Vector2Int.RoundToInt(BattleController.Instance.CharacterList[i].transform.position);
                positionList.Remove(position);
            }
        }

        return positionList[Random.Range(0, positionList.Count)];
    }
}

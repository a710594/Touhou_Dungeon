using System.Collections;
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
                    (camp == BattleCharacterInfo.CampEnum.Enemy && BattleController.Instance.CharacterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Partner)) //與自己不同陣營的角色會被視為障礙物
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

    /*public Vector2 GetRandomPosition() //隨機取得空格
    {
        Vector2Int position = new Vector2Int();
        List<Vector2Int> positionList = new List<Vector2Int>(MapDic.Keys);

        while (positionList.Count > 0)
        {
            position = positionList[Random.Range(0, positionList.Count)];

            if (BattleController.Instance.GetCharacterByPosition(position) == null && MapDic[Vector2Int.RoundToInt(position)].MoveCost != -1)
            {
                return position;
            }
            else
            {
                positionList.Remove(position);
            }
        }
        return Vector2.zero;
    }*/

    public BattleField GetField(Vector2 position) 
    {
        BattleField battleField = null;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        return battleField;
    }

    public Buff GetFieldBuff(Vector2 position)
    {
        BattleField battleField;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        if (battleField != null)
        {
            Buff buff = battleField.Buff;
            if (buff != null)
            {
                return buff;
            }
            else
            {
                return new Buff(); //如果該地形上沒有 buff,就回傳一個空的 buff
            }
        }
        else
        {
            return new Buff(); //如果沒有該座標的地形,就回傳一個空的 buff
        }
    }
}

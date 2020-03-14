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
    public Dictionary<Vector2Int, BattleField> MapDic = new Dictionary<Vector2Int, BattleField>();

    public void Init(Vector2Int center, Dictionary<Vector2Int, BattleField> mapDic)
    {
        Center = center;
        MapDic = mapDic;
        MapBound = Utility.GetMapBounds(new List<Vector2Int>(MapDic.Keys));
    }

    public List<Vector2Int> GetPath(Vector2 from, Vector2 to, BattleCharacter.CampEnum camp) //camp:尋路的人是我方還是敵方
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
                if ((camp == BattleCharacter.CampEnum.Partner && BattleController.Instance.CharacterList[i].Camp == BattleCharacter.CampEnum.Enemy) ||
                    (camp == BattleCharacter.CampEnum.Enemy && BattleController.Instance.CharacterList[i].Camp == BattleCharacter.CampEnum.Partner)) //與自己不同陣營的角色會被視為障礙物
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

    public int GetPathLength(Vector2 from, Vector2 to, BattleCharacter.CampEnum camp)
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

    public Vector2 GetRandomPosition(bool isPlayer) //隨機取得空格
    {
        Vector3Int position = MapBound.position;
        Vector3Int size = MapBound.size;
        Vector2 randomPosition = new Vector2();

        if (isPlayer)
        {
            randomPosition.x = Utility.NormalDistributionRandom(position.x, position.x + size.x);
            randomPosition.y = Utility.NormalDistributionRandom(position.y, position.y + size.y);
        }
        else
        {
            randomPosition.x = UnityEngine.Random.Range(position.x, position.x + size.x);
            randomPosition.y = UnityEngine.Random.Range(position.y, position.y + size.y);
        }

        if (BattleController.Instance.GetCharacterByPosition(randomPosition) == null && MapDic[Vector2Int.RoundToInt(randomPosition)].MoveCost != -1)
        {
            return randomPosition;
        }
        else
        {
            return GetRandomPosition(isPlayer);
        }
    }

    public BattleField GetField(Vector2 position) 
    {
        BattleField battleField = null;
        MapDic.TryGetValue(Vector2Int.RoundToInt(position), out battleField);
        return battleField;
    }

    public Buff GetFieldBuff(Vector2 position)
    {
        Buff buff = MapDic[Vector2Int.RoundToInt(position)].Buff;
        if (buff != null)
        {
            return buff;
        }
        else
        {
            return new Buff(); //如果該地形上沒有 buff,就回傳一個空的 buff
        }
    }
}

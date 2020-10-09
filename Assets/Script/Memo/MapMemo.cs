using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMemo
{
    public int ArriveFloor; //最多到達第幾樓
    public int ID;
    public BoundsInt MapBound;
    public Vector2Int Start;
    public Vector2Int Goal;
    public Vector2 PlayerPosition; //玩家的位置
    public List<string> MapList;
    public List<string> GrassList;
    public List<string> KeyList;
    public List<string> DoorList;
    public List<string> ExploredList = new List<string>(); //走過的地圖範圍
    public List<string> ExploredWallList = new List<string>(); //已被發現的牆壁的範圍
    public List<string> GuardList = new List<string>(); //守衛型敵人的位置,遇到該敵人後會 remove
    public List<List<string>> RoomPositionList = new List<List<string>>();
    public Dictionary<string, int> MoneyDic = new Dictionary<string, int>();
    public Dictionary<string, Event.TypeEnum> ExploreEventDic = new Dictionary<string, Event.TypeEnum>();
    public Dictionary<string, Treasure> TreasureDic = new Dictionary<string, Treasure>();

    public MapMemo() { }

    public MapMemo(int arriveFloor, MapInfo info, Vector2 playerPosition)
    {
        ArriveFloor = arriveFloor;
        ID = info.ID;
        MapBound = info.MapBound;
        Start = info.Start;
        Goal = info.Goal;
        PlayerPosition = playerPosition;
        MapList = Utility.Vector2IntToString(info.MapList);
        GrassList = Utility.Vector2IntToString(info.GrassList);
        KeyList = Utility.Vector2IntToString(info.KeyList);
        DoorList = Utility.Vector2IntToString(info.DoorList);
        ExploredList = Utility.Vector2IntToString(info.ExploredList);
        ExploredWallList = Utility.Vector2IntToString(info.ExploredWallList);
        GuardList = Utility.Vector2IntToString(info.GuardList);

        for (int i=0; i<info.RoomPositionList.Count; i++)
        {
            RoomPositionList.Add(Utility.Vector2IntToString(info.RoomPositionList[i]));
        }

        foreach (KeyValuePair<Vector2Int, int> item in info.MoneyDic)
        {
            MoneyDic.Add(Utility.Vector2IntToString(item.Key), item.Value);
        }

        foreach (KeyValuePair<Vector2Int, Event> item in info.ExploreEventDic)
        {
            ExploreEventDic.Add(Utility.Vector2IntToString(item.Key), item.Value.Type);
        }

        foreach (KeyValuePair<Vector2Int, Treasure> item in info.TreasureDic)
        {
            TreasureDic.Add(Utility.Vector2IntToString(item.Key), item.Value);
        }
    }
}

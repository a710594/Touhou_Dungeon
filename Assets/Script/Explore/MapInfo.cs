using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DungeonBuilder 到 ExploreController 的中間資料
public class MapInfo
{
    public int Floor;
    public int LastFloor;
    public int NextFloor;
    public BoundsInt MapBound;
    public Vector2Int Start;
    public Vector2Int Goal;
    //public DungeonData.RootObject DungeonData;
    public string GroundTile;
    public string DoorTile;
    public string GrassTile;
    public string WallTile;
    public List<Vector2Int> MapList = new List<Vector2Int>();
    public List<Vector2Int> GrassList = new List<Vector2Int>();
    public List<Vector2Int> KeyList = new List<Vector2Int>();
    public List<Vector2Int> DoorList = new List<Vector2Int>();
    public List<Vector2Int> ExploredList = new List<Vector2Int>(); //走過的地圖範圍
    public List<Vector2Int> ExploredWallList = new List<Vector2Int>(); //已被發現的牆壁的範圍
    public List<Vector2Int> GuardList = new List<Vector2Int>(); //守衛型敵人的位置,遇到該敵人後會 remove
    public List<List<Vector2Int>> RoomPositionList = new List<List<Vector2Int>>(); //各個房間的空間座標,生怪的時候用的
    public Dictionary<Vector2Int, int> MoneyDic = new Dictionary<Vector2Int, int>();
    public Dictionary<Vector2Int, int> ExploreEventDic = new Dictionary<Vector2Int, int>();
    public Dictionary<Vector2Int, Treasure> TreasureDic = new Dictionary<Vector2Int, Treasure>();


    public MapInfo() { }

    public MapInfo(MapMemo memo)
    {
        Floor = memo.ID;
        LastFloor = memo.LastFloor;
        NextFloor = memo.NextFloor;
        MapBound = memo.MapBound;
        Start = memo.Start;
        Goal = memo.Goal;
        GroundTile = memo.GroundTile;
        DoorTile = memo.DoorTile;
        GrassTile = memo.GrassTile;
        WallTile = memo.WallTile;
        MapList = Utility.StringToVector2Int(memo.MapList);
        GrassList = Utility.StringToVector2Int(memo.GrassList);
        KeyList = Utility.StringToVector2Int(memo.KeyList);
        DoorList = Utility.StringToVector2Int(memo.DoorList);
        ExploredList = Utility.StringToVector2Int(memo.ExploredList);
        ExploredWallList = Utility.StringToVector2Int(memo.ExploredWallList);
        GuardList = Utility.StringToVector2Int(memo.GuardList);

        for (int i=0; i<memo.RoomPositionList.Count; i++)
        {
            RoomPositionList.Add(Utility.StringToVector2Int(memo.RoomPositionList[i]));
        }

        foreach (KeyValuePair<string, int> item in memo.MoneyDic)
        {
            MoneyDic.Add(Utility.StringToVector2Int(item.Key), item.Value);
        }

        foreach (KeyValuePair<string, int> item in memo.ExploreEventDic)
        {
            ExploreEventDic.Add(Utility.StringToVector2Int(item.Key), item.Value);
        }

        foreach (KeyValuePair<string, Treasure> item in memo.TreasureDic)
        {
            TreasureDic.Add(Utility.StringToVector2Int(item.Key), item.Value);
        }
    }
}
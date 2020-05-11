using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo
{
    public int DungeonId;
    public BoundsInt MapBound;
    public Vector2Int Start;
    public Vector2Int Goal;
    public List<Vector2Int> MapList;
    public List<Vector2Int> GrassList;
    public List<Vector2Int> KeyList;
    public List<Vector2Int> WallList = new List<Vector2Int>();
    public List<Vector2Int> MistList = new List<Vector2Int>();
    public List<Room> RoomList = new List<Room>();
    public Dictionary<Vector2Int, int> MoneyDic;
    public Dictionary<Vector2Int, int> ExploreEventDic;
    public Dictionary<Vector2Int, Treasure> TreasureDic;
    public Dictionary<Vector2Int, Room> DoorDic;
}

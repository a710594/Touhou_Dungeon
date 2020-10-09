using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonFromScene : MonoBehaviour
{
    public int Floor;
    public int LastFloor;
    public int NextFloor;
    public TilemapToPositionList TilemapToPositionList;

    public MapInfo GetMapInfo()
    {
        MapInfo mapInfo = new MapInfo();

        mapInfo.ID = Floor;
        mapInfo.LastFloor = LastFloor;
        mapInfo.NextFloor = NextFloor;
        mapInfo.MapList = TilemapToPositionList.GetPositionList(0);
        mapInfo.Start = TilemapToPositionList.GetPositionList(1)[0];
        mapInfo.Goal = TilemapToPositionList.GetPositionList(2)[0];
        mapInfo.MapBound = Utility.GetMapBounds(mapInfo.MapList);

        return mapInfo;
    }
}

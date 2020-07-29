using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonFromScene : MonoBehaviour
{
    public TilemapToPositionList TilemapToPositionList;

    public MapInfo GetMapInfo()
    {
        MapInfo mapInfo = new MapInfo();

        mapInfo.MapList = TilemapToPositionList.GetPositionList(0);
        mapInfo.Start = TilemapToPositionList.GetPositionList(1)[0];
        mapInfo.Goal = TilemapToPositionList.GetPositionList(2)[0];
        mapInfo.MapBound = Utility.GetMapBounds(mapInfo.MapList);

        return mapInfo;
    }
}

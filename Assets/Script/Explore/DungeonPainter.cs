using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPainter
{
    private static DungeonPainter _instance;
    public static DungeonPainter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DungeonPainter();
            }
            return _instance;
        }
    }

    public void Paint(MapInfo info)
    {
        TilePainter.Instance.ClearAll();

        //ground
        for (int i=0; i<info.MapList.Count; i++)
        {
            TilePainter.Instance.Painting(info.GroundTile, 0, info.MapList[i]);
        }

        //door
        for(int i=0; i<info.DoorList.Count; i++)
        {
            TilePainter.Instance.Painting(info.DoorTile, 2, info.DoorList[i]);
        }

        //grass
        for (int i = 0; i < info.GrassList.Count; i++)
        {
            TilePainter.Instance.Painting(info.GrassTile, 0, info.GrassList[i]);
        }

        //treasure
        foreach (KeyValuePair<Vector2Int, Treasure> item in info.TreasureDic)
        {
            TilePainter.Instance.Painting(item.Value.TileName, 2, item.Key);
        }

        //key
        for (int i=0; i<info.KeyList.Count; i++)
        {
            TilePainter.Instance.Painting("Key", 2, info.KeyList[i]);
        }

        //explore point
        foreach (KeyValuePair<Vector2Int, int> item in info.ExploreEventDic)
        {
            TilePainter.Instance.Painting(EventData.GetData(item.Value).Tile, 2, item.Key);
        }

        //起點
        TilePainter.Instance.Painting("UpStairs", 0, info.Start);

        //終點
        TilePainter.Instance.Painting("DownStairs", 0, info.Goal);

        //Mist
        Vector2Int position;
        for (int i = 0; i <= info.MapBound.size.x + 30; i++)
        {
            for (int j = 0; j <= info.MapBound.size.y + 30; j++)
            {
                position = new Vector2Int(info.MapBound.xMin + i - 15, info.MapBound.yMin + j - 15);
                if (!info.MapList.Contains(position))
                {
                    TilePainter.Instance.Painting(info.WallTile, 0, position);
                }
                TilePainter.Instance.Painting("Mist", 3, position);
            }
        }

        for (int i=0; i<info.ExploredList.Count; i++)
        {
            TilePainter.Instance.Clear(3, info.ExploredList[i]);
        }
    }
}

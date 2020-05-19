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
            TilePainter.Instance.Painting(info.DungeonData.GroundTile, 0, info.MapList[i]);
        }

        //door
        List<Vector2Int> doorList = new List<Vector2Int>(info.DoorDic.Keys);
        for(int i=0; i<doorList.Count; i++)
        {
            TilePainter.Instance.Painting(info.DungeonData.DoorTile, 2, doorList[i]);
        }

        //grass
        for (int i = 0; i < info.GrassList.Count; i++)
        {
            TilePainter.Instance.Painting(info.DungeonData.GrassTile, 0, info.GrassList[i]);
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

        //money
        foreach (KeyValuePair<Vector2Int, int> item in info.MoneyDic)
        {
            TilePainter.Instance.Painting("Money", 2, item.Key);
        }

        //explore point
        foreach (KeyValuePair<Vector2Int, int> item in info.ExploreEventDic)
        {
            TilePainter.Instance.Painting("ExplorePoint", 2, item.Key);
        }

        //起點
        TilePainter.Instance.Painting("UpStairs", 0, info.Start);

        //終點
        TilePainter.Instance.Painting("DownStairs", 0, info.Goal);

        //Wall
        for (int i = 0; i < info.WallList.Count; i++)
        {
            TilePainter.Instance.Painting(info.DungeonData.WallTile, 0, info.WallList[i]);
        }

        //Mist
        for (int i = 0; i < info.MistList.Count; i++)
        {
            TilePainter.Instance.Painting("Mist", 3, info.MistList[i]);
        }

        //Vector2Int position;
        //for (int i = 0; i <= info.MapBound.size.x + 20; i++)
        //{
        //    for (int j = 0; j <= info.MapBound.size.y + 20; j++)
        //    {
        //        position = new Vector2Int(info.MapBound.xMin + i - 10, info.MapBound.yMin + j - 10);
        //        if (!info.MapList.Contains(position))
        //        {
        //            TilePainter.Instance.Painting(dungeonData.WallTile, 0, position);
        //        }

        //        //mist
        //        TilePainter.Instance.Painting("Black", 2, position);
        //    }
        //}
    }
}

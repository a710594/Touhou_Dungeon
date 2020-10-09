using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPainter : MonoBehaviour
{
    public static DungeonPainter Instance;

    private int _loadingCount = 0;

    public void Paint(MapInfo info)
    {
        StartCoroutine(paint(info));
    }

    private IEnumerator paint(MapInfo info)
    {
        DungeonGroupData.RootObject data = DungeonGroupData.GetData(info.Group);
        TilePainter.Instance.ClearAll();

        //ground
        for (int i=0; i<info.MapList.Count; i++)
        {
            TilePainter.Instance.Painting(data.GroundTile, 0, info.MapList[i]);
            if (CheckLoadingCount()) yield return null;
        }

        //door
        for (int i=0; i<info.DoorList.Count; i++)
        {
            TilePainter.Instance.Painting(data.DoorTile, 1, info.DoorList[i]);
        }

        //grass
        for (int i = 0; i < info.GrassList.Count; i++)
        {
            TilePainter.Instance.Painting(data.GrassTile, 0, info.GrassList[i]);
            if (CheckLoadingCount()) yield return null;
        }

        //treasure
        foreach (KeyValuePair<Vector2Int, Treasure> item in info.TreasureDic)
        {
            TilePainter.Instance.Painting(item.Value.TileName, 1, item.Key);
        }

        //key
        for (int i=0; i<info.KeyList.Count; i++)
        {
            TilePainter.Instance.Painting("Key", 1, info.KeyList[i]);
        }

        //explore point
        foreach (KeyValuePair<Vector2Int, Event> item in info.ExploreEventDic)
        {
            TilePainter.Instance.Painting(item.Value.Tile, 1, item.Key);
        }

        //起點
        TilePainter.Instance.Painting("UpStairs", 1, info.Start);

        //終點
        TilePainter.Instance.Painting("DownStairs", 1, info.Goal);

        //Layer 2 是保留給角色的,Mist 蓋在角色上
        TilePainter.Instance.Fill("Mist", 3, info.MapBound.xMin - 15, info.MapBound.yMin - 15, info.MapBound.xMin + info.MapBound.size.x + 15, info.MapBound.yMin + info.MapBound.size.y + 15);
        TilePainter.Instance.Fill(data.WallTile, 4, info.MapBound.xMin - 15, info.MapBound.yMin - 15, info.MapBound.xMin + info.MapBound.size.x + 15, info.MapBound.yMin + info.MapBound.size.y + 15);

        for (int i=0; i<info.ExploredList.Count; i++)
        {
            TilePainter.Instance.Clear(3, info.ExploredList[i]);
            //if(CheckLoadingCount()) yield return null;
        }

        ExploreController.Instance.SetFloor();
        LoadingUI.Instance.Close();
    }

    private bool CheckLoadingCount()
    {
        _loadingCount++;
        if (_loadingCount == 1000)
        {
            _loadingCount = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
}

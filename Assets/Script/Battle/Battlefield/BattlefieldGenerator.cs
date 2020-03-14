using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldGenerator
{
    private static BattlefieldGenerator _instance;
    public static BattlefieldGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattlefieldGenerator();
            }
            return _instance;
        }
    }

    public void Generate(int id)
    {
        Vector2Int center;
        List<Vector2Int> reservedLits = new List<Vector2Int>(); //地圖中間的九宮格是保留區,不生成特殊地形
        Dictionary<Vector2Int, BattleField> mapDic = new Dictionary<Vector2Int, BattleField>();

        BattlefieldData.RootObject battlefieldData = BattlefieldData.GetData(id);
        int width = Random.Range(battlefieldData.MinWidth, battlefieldData.MaxWidth + 1);
        int height = Random.Range(battlefieldData.MinHeight, battlefieldData.MaxHeight + 1);
        center = new Vector2Int(width / 2, height / 2);

        reservedLits.Add(center);
        reservedLits.Add(center + Vector2Int.left + Vector2Int.up);
        reservedLits.Add(center + Vector2Int.up);
        reservedLits.Add(center + Vector2Int.right + Vector2Int.up);
        reservedLits.Add(center + Vector2Int.left);
        reservedLits.Add(center + Vector2Int.right);
        reservedLits.Add(center + Vector2Int.left + Vector2Int.down);
        reservedLits.Add(center + Vector2Int.down);
        reservedLits.Add(center + Vector2Int.right + Vector2Int.down);

        Vector2Int pos = new Vector2Int();
        BattleTileData.RootObject tileData;

        //地板
        tileData = BattleTileData.GetData(battlefieldData.GroundID);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pos.x = i;
                pos.y = j;
                mapDic.Add(pos, new BattleField(pos, tileData));
                TilePainter.Instance.Painting(tileData.TileName, 0, pos);
            }
        }

        //牆壁
        tileData = BattleTileData.GetData(battlefieldData.BlockID);
        List<Vector2Int> tempPositionList = new List<Vector2Int>(mapDic.Keys);

        for (int i=0; i<reservedLits.Count; i++)
        {
            tempPositionList.Remove(reservedLits[i]);
        }

        int blockCount = Random.Range(battlefieldData.MinBlockCount, battlefieldData.MaxBlockCount);
        for (int i = 0; i < blockCount; i++)
        {
            pos = tempPositionList[Random.Range(0, tempPositionList.Count)];
            mapDic[pos].SetData(tileData);
            TilePainter.Instance.Painting(tileData.TileName, 0, pos);
            tempPositionList.Remove(pos);
        }

        //草
        tileData = BattleTileData.GetData(battlefieldData.GrassID);
        int grassCount = Random.Range(battlefieldData.MinGrassCount, battlefieldData.MaxGrassCount);
        for (int i = 0; i < grassCount; i++)
        {
            pos = tempPositionList[Random.Range(0, tempPositionList.Count)];
            mapDic[pos].SetData(tileData);
            TilePainter.Instance.Painting(tileData.TileName, 0, pos);
            tempPositionList.Remove(pos);
        }

        BattleFieldManager.Instance.Init(center, mapDic);
    }
}

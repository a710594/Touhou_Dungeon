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

    public void Generate(int id, int enemyAmount)
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

        //敵人的位置
        List<Vector2Int> tempPositionList = new List<Vector2Int>(mapDic.Keys);
        List<Vector2Int> enemyPositionList = new List<Vector2Int>();
        for (int i = 0; i < reservedLits.Count; i++)
        {
            tempPositionList.Remove(reservedLits[i]);
        }

        Vector2Int firstPos = new Vector2Int();
        firstPos = tempPositionList[Random.Range(0, tempPositionList.Count)];
        enemyPositionList.Add(firstPos); //第一個敵人的位置
        Debug.Log("1:" + firstPos);
        tempPositionList.Remove(firstPos);
        for (int i=1; i<enemyAmount; i++) //其他敵人的位置與第一個敵人的位置相近
        {
            //pos = new Vector2Int(enemyPositionList[0].x + Random.Range(-enemyAmount, enemyAmount + 1), enemyPositionList[0].y + Random.Range(-enemyAmount, enemyAmount + 1));
            //if (tempPositionList.Contains(pos))
            //{
            //    enemyPositionList.Add(pos);
            //    Debug.Log(i + ":" + pos);
            //}
            //else
            //{
            //    i--;
            //}
            //tempPositionList.Remove(pos);
            while (tempPositionList.Count > 0) 
            {
                pos = tempPositionList[Random.Range(0, tempPositionList.Count)];
                tempPositionList.Remove(pos);
                if (Utility.GetDistance(pos, firstPos) <= enemyAmount) 
                {
                    enemyPositionList.Add(pos);
                    Debug.Log(i + ":" + pos);
                    break;
                }
            }
        }

        //牆壁
        tileData = BattleTileData.GetData(battlefieldData.BlockID);

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

        //Camera Wall
        GameObject wall;

        //up
        wall = ResourceManager.Instance.Spawn("CameraWall", ResourceManager.Type.Other);
        wall.transform.localScale = new Vector3(width + 2, 1, 1);
        wall.transform.position = new Vector3(width / 2f - 0.5f, height + 1, 0);

        //down
        wall = ResourceManager.Instance.Spawn("CameraWall", ResourceManager.Type.Other);
        wall.transform.localScale = new Vector3(width + 2, 1, 1);
        wall.transform.position = new Vector3(width / 2f - 0.5f, 0, 0);

        //left
        wall = ResourceManager.Instance.Spawn("CameraWall", ResourceManager.Type.Other);
        wall.transform.localScale = new Vector3(1, height + 2, 1);
        wall.transform.position = new Vector3(-1, height / 2f + 0.5f, 0);

        //right
        wall = ResourceManager.Instance.Spawn("CameraWall", ResourceManager.Type.Other);
        wall.transform.localScale = new Vector3(1, height + 2, 1);
        wall.transform.position = new Vector3(width, height / 2f + 0.5f, 0);


        BattleFieldManager.Instance.Init(center, enemyPositionList, mapDic);
    }

    public void Generate(Dictionary<string, BattleField> mapDic) 
    {
        foreach (KeyValuePair<string, BattleField> item in mapDic)
        {
            TilePainter.Instance.Painting(item.Value.TileName, 0, Utility.StringToVector2Int(item.Key));
            if (item.Value.Status != null)
            {
                BattleController.Instance.TurnEndHandler += item.Value.CheckRemainTurn;
                TilePainter.Instance.Painting(item.Value.BuffTileName, 1, Utility.StringToVector2Int(item.Key));
            }
        }
    }
}

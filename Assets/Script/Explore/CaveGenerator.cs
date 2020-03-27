using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator
{
    public enum Direction
    {
        East = 0,
        West,
        South,
        North,
        None,
    }

    private static CaveGenerator _instance;
    public static CaveGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CaveGenerator();
            }
            return _instance;
        }
    }

    private List<Vector2Int> _mapList = new List<Vector2Int>();

    
    public bool GetPosition(List<Vector2Int> mapList, List<Vector2Int> notBesideList, List<Vector2Int> requirementList, out Vector2Int position)
    {
        position = new Vector2Int();
        BoundsInt bounds = Utility.GetMapBounds(mapList);
        List<Vector2Int> positionList = new List<Vector2Int>();

        _mapList = mapList;

        for (int i = 0; i < bounds.size.x + 2; i++)
        {
            for (int j = 0; j < bounds.size.y + 2; j++)
            {
                positionList.Add(new Vector2Int(i + bounds.position.x - 1, j + bounds.position.y - 1));
            }
        }

        for (int i = 0; i < positionList.Count; i++)
        {
            position = positionList[Random.Range(0, positionList.Count)];
            //if (IsBeside(position, selectedType))

            if (notBesideList != null &&
              (notBesideList.Contains(position + Vector2Int.left) || notBesideList.Contains(position + Vector2Int.right) || notBesideList.Contains(position + Vector2Int.up) || notBesideList.Contains(position + Vector2Int.down)))
            {
                positionList.Remove(position);
                i--;
                continue;
            }

            if((requirementList==null || requirementList.Contains(position)) &&
               !_mapList.Contains(position) && 
               (_mapList.Contains(position + Vector2Int.left) || _mapList.Contains(position + Vector2Int.right) || _mapList.Contains(position + Vector2Int.up) || _mapList.Contains(position + Vector2Int.down)))
            {
                return true;
            }
            else
            {
                positionList.Remove(position);
                i--;
            }
        }

        return false;
    }

    /*
    public Vector2Int GetPosition(Dictionary<Vector2Int, BaseTile> mapDic, BaseTile.Terrain selectedType, BaseTile.Terrain requiredType)
    {
        int count = 0;

        Vector2Int position = new Vector2Int();
        List<Vector2Int> positionList = new List<Vector2Int>();

        _mapList = mapDic;


        foreach (KeyValuePair<Vector2Int, BaseTile> pair in mapDic)
        {
            positionList.Add(pair.Key);
        }

        for (int i = 0; i < positionList.Count; i++)
        {
            position = positionList[Random.Range(0, positionList.Count)];
            if (IsBeside(position, selectedType, requiredType))
            {
                return position;
            }
            else
            {
                positionList.Remove(position);
                i--;

                count++;
            }
        }

        return Vector2Int.zero;
    }

    private bool IsBeside(Vector2Int position, BaseTile.Terrain selectedType, BaseTile.Terrain requiredType = BaseTile.Terrain.None)
    {
        if ((requiredType == BaseTile.Terrain.None && !_mapList.ContainsKey(position)) || (_mapList.ContainsKey(position) && _mapList[position].Type == requiredType))
        {
            if ((_mapList.ContainsKey(position + Vector2Int.up) && _mapList[position + Vector2Int.up].Type == selectedType) ||
               (_mapList.ContainsKey(position + Vector2Int.down) && _mapList[position + Vector2Int.down].Type == selectedType) ||
               (_mapList.ContainsKey(position + Vector2Int.left) && _mapList[position + Vector2Int.left].Type == selectedType) ||
               (_mapList.ContainsKey(position + Vector2Int.right) && _mapList[position + Vector2Int.right].Type == selectedType)) //如果旁邊的 tile 的 type 為 selectedType
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }*/
    
}

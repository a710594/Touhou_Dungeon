using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgor
{
    private static AStarAlgor _instance;
    public static AStarAlgor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AStarAlgor();
            }
            return _instance;
        }
    }

    Dictionary<Vector2Int, int> _mapDic = new Dictionary<Vector2Int, int>(); //position, moveCost
    private Dictionary<Vector2Int, Vector2Int> _cameFromDic = new Dictionary<Vector2Int, Vector2Int>(); //key 是 value 的子節點

    public List<Vector2Int> GetPath(Vector2Int start, Vector2Int goal, List<Vector2Int> mapList, bool isRandom)
    {
        Dictionary<Vector2Int, int> mapDic = new Dictionary<Vector2Int, int>();
        for (int i=0; i<mapList.Count; i++)
        {
            mapDic.Add(mapList[i], 1);
        }
        return GetPath(start, goal, mapDic, isRandom);
    }

    public List<Vector2Int> GetPath(Vector2Int start, Vector2Int goal, Dictionary<Vector2Int, int> mapDic, bool isRandom)
    {
        _mapDic = mapDic;
        List<Vector2Int> closedset = new List<Vector2Int>(); //已被估算的節點集合
        List<Vector2Int> openset = new List<Vector2Int>(); //將要被估算的節點集合，初始只包含start
        openset.Add(start);
        _cameFromDic.Clear();
        Dictionary<Vector2Int, int> g_score = new Dictionary<Vector2Int, int>();
        g_score.Add(start, 0); //g(n)
        Dictionary<Vector2Int, int> h_score = new Dictionary<Vector2Int, int>();
        h_score.Add(start, Utility.GetDistance(start, goal)); //通過估計函數 估計h(start)
        Dictionary<Vector2Int, int> f_score = new Dictionary<Vector2Int, int>();
        f_score.Add(start, h_score[start]); //f(n)=h(n)+g(n)，由於g(n)=0，所以省略

        while (openset.Count > 0) //當將被估算的節點存在時，執行循環
        {
            Vector2Int x = openset[0];
            for (int i = 1; i < openset.Count; i++) //在將被估計的集合中找到f(x)最小的節點
            {
                if (f_score[openset[i]] < f_score[x])
                {
                    x = openset[i];
                }
                else if (f_score[openset[i]] == f_score[x] && isRandom)
                {
                    if (Random.Range(0, 2) == 0) //可能是造成 stack overflow 的原因?
                    {
                        x = openset[i];
                    }
                }
            }

            if (x == goal)
            {
                return ReconstructPath(goal);   //返回到x的最佳路徑
            }

            openset.Remove(x); //將x節點從將被估算的節點中刪除
            closedset.Add(x); //將x節點插入已經被估算的節點

            bool isBetter;
            List<Vector2Int> neighborList = GetNeighborPos(x);
            for (int i = 0; i < neighborList.Count; i++)  //循環遍歷與x相鄰節點
            {
                Vector2Int y = neighborList[i];

                if (closedset.Contains(y)) //若y已被估值，跳過
                {
                    continue;
                }

                int g = g_score[x] + _mapDic[y];    //從起點到節點y的距離

                if (!openset.Contains(y)) //若y不是將被估算的節點
                {
                    isBetter = true; //暫時判斷為更好
                }
                else if (g < g_score[y])
                {
                    isBetter = true; //暫時判斷為更好
                }
                else
                {
                    isBetter = false; //暫時判斷為更差
                }

                if (isBetter)
                {
                    _cameFromDic[y] = x; //將y設為x的子節點
                    g_score[y] = g; //更新y到原點的距離
                    h_score[y] = Utility.GetDistance(y, goal); //估計y到終點的距離
                    f_score[y] = g_score[y] + h_score[y];
                    openset.Add(y);
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Vector2Int currentNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        if (_cameFromDic.ContainsKey(currentNode))
        {
            path = ReconstructPath(_cameFromDic[currentNode]);
        }

        path.Add(currentNode);
        return path;
    }

    private List<Vector2Int> GetNeighborPos(Vector2Int current)
    {
        Vector2Int pos;
        List<Vector2Int> list = new List<Vector2Int>();

        if (_mapDic.ContainsKey(current + Vector2Int.left))
        {
            list.Add(current + Vector2Int.left);
        }

        if (_mapDic.ContainsKey(current + Vector2Int.right))
        {
            list.Add(current + Vector2Int.right);
        }

        if (_mapDic.ContainsKey(current + Vector2Int.up))
        {
            list.Add(current + Vector2Int.up);
        }

        if (_mapDic.ContainsKey(current + Vector2Int.down))
        {
            list.Add(current + Vector2Int.down);
        }

        return list;
    }
}

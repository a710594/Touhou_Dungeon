using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePainter : MonoBehaviour
{
    private static readonly string _tilePath = "Tile/";

    public static TilePainter Instance;

    public Tilemap[] TileMap;

    private Dictionary<string, TileBase> _tileDic = new Dictionary<string, TileBase>();
    private Dictionary<int, List<Vector2Int>> _paintingList = new Dictionary<int, List<Vector2Int>>(); //有著色的格子 orderInLayer/position list

    public void Init()
    {
    }

    public void Painting(string tileName, int orderInLayer, Vector2Int position)
    {
        TileMap[orderInLayer].SetTile(new Vector3Int(position.x, position.y, 0), GetTile(tileName));

        if (!_paintingList.ContainsKey(orderInLayer))
        {
            _paintingList.Add(orderInLayer, new List<Vector2Int>());
        }

        if (!_paintingList[orderInLayer].Contains(position))
        {
            _paintingList[orderInLayer].Add(position);
        }
    }

    public void Fill(string tileName, int orderInLayer, int startX, int startY, int endX, int endY)
    {
        BoundsInt bounds = new BoundsInt(startX, startY, 0, endX - startX, endY - startY, 1);

        TileBase tile = GetTile(tileName);
        TileBase[] tileArray = new TileBase[bounds.size.x * bounds.size.y];
        for (int index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = tile;
        }

        TileMap[orderInLayer].SetTilesBlock(bounds,  tileArray);
    }

    public void Clear(int orderInLayer, Vector2Int position)
    {
        //TileMap[orderInLayer].SetTile(new Vector3Int(position.x, position.y, 0), null);
        Painting("Clear", orderInLayer, position);
    }

    public void Clear(int orderInLayer)
    {
        if (!_paintingList.ContainsKey(orderInLayer))
        {
            return;
        }

        for (int i = 0; i < _paintingList[orderInLayer].Count; i++)
        {
            TileMap[orderInLayer].SetTile(new Vector3Int(_paintingList[orderInLayer][i].x, _paintingList[orderInLayer][i].y, 0), null);
        }
        _paintingList[orderInLayer].Clear();
    }

    public void ClearAll()
    {
        for (int i = 0; i < _paintingList.Count; i++)
        {
            Clear(_paintingList.ElementAt(i).Key);
        }
    }

    private TileBase GetTile(string tileName)
    {
        if (!_tileDic.ContainsKey(tileName))
        {
            _tileDic.Add(tileName, Resources.Load<TileBase>(_tilePath + tileName));
        }
        return _tileDic[tileName];
    }

    void Awake()
    {
        for (int i = 0; i < TileMap.Length; i++)
        {
            TileMap[i].CompressBounds();
        }

        Instance = this;
    }
}

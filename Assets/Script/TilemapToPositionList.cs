using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToPositionList : MonoBehaviour
{
    public Tilemap[] Tilemap;

    // Start is called before the first frame update
    public List<Vector2Int> GetPositionList(int layer)
    {

        BoundsInt bounds = Tilemap[layer].cellBounds;
        TileBase[] allTiles = Tilemap[layer].GetTilesBlock(bounds);
        List<Vector2Int> positionList = new List<Vector2Int>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    positionList.Add(new Vector2Int(bounds.x + x, bounds.y + y));
                }
            }
        }

        return positionList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

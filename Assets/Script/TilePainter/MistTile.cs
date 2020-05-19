using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MistTile : Tile
{
    public Sprite Center;
    public Sprite Up;
    public Sprite Down;
    public Sprite Left;
    public Sprite Right;
    public Sprite UpLeftConvex;
    public Sprite UpRightConvex;
    public Sprite DownLeftConvex;
    public Sprite DownRightConvex;
    public Sprite UpLeftConcave;
    public Sprite UpRightConcave;
    public Sprite DownLeftConcave;
    public Sprite DownRightConcave;
    public Sprite Other;

    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasMistTile(tilemap, position))
                    tilemap.RefreshTile(position);
            }
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        if (HasMistTile(tilemap, location + Vector3Int.up) && HasMistTile(tilemap, location + Vector3Int.down) && HasMistTile(tilemap, location + Vector3Int.left) && HasMistTile(tilemap, location + Vector3Int.right))
        {
            if (!HasMistTile(tilemap, location + Vector3Int.down + Vector3Int.left))
            {
                tileData.sprite = DownLeftConcave;
            }
            else if (!HasMistTile(tilemap, location + Vector3Int.down + Vector3Int.right))
            {
                tileData.sprite = DownRightConcave;
            }
            else if (!HasMistTile(tilemap, location + Vector3Int.up + Vector3Int.left))
            {
                tileData.sprite = UpLeftConcave;
            }
            else if (!HasMistTile(tilemap, location + Vector3Int.up + Vector3Int.right))
            {
                tileData.sprite = UpRightConcave;
            }
            else
            {
                tileData.sprite = Center;
            }
        }
        else if (!HasMistTile(tilemap, location + Vector3Int.up) && HasMistTile(tilemap, location + Vector3Int.down) && HasMistTile(tilemap, location + Vector3Int.left) && HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = Up;
        }
        else if (HasMistTile(tilemap, location + Vector3Int.up) && !HasMistTile(tilemap, location + Vector3Int.down) && HasMistTile(tilemap, location + Vector3Int.left) && HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = Down;
        }
        else if (HasMistTile(tilemap, location + Vector3Int.up) && HasMistTile(tilemap, location + Vector3Int.down) && !HasMistTile(tilemap, location + Vector3Int.left) && HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = Left;
        }
        else if (HasMistTile(tilemap, location + Vector3Int.up) && HasMistTile(tilemap, location + Vector3Int.down) && HasMistTile(tilemap, location + Vector3Int.left) && !HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = Right;
        }
        else if (!HasMistTile(tilemap, location + Vector3Int.up) && HasMistTile(tilemap, location + Vector3Int.down) && !HasMistTile(tilemap, location + Vector3Int.left) && HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = UpLeftConvex;
        }
        else if (!HasMistTile(tilemap, location + Vector3Int.up) && HasMistTile(tilemap, location + Vector3Int.down) && HasMistTile(tilemap, location + Vector3Int.left) && !HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = UpRightConvex;
        }
        else if (HasMistTile(tilemap, location + Vector3Int.up) && !HasMistTile(tilemap, location + Vector3Int.down) && !HasMistTile(tilemap, location + Vector3Int.left) && HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = DownLeftConvex;
        }
        else if (HasMistTile(tilemap, location + Vector3Int.up) && !HasMistTile(tilemap, location + Vector3Int.down) && HasMistTile(tilemap, location + Vector3Int.left) && !HasMistTile(tilemap, location + Vector3Int.right))
        {
            tileData.sprite = DownRightConvex;
        }
        else
        {
            tileData.sprite = Other;
        }
    }

    private bool HasMistTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/MistTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Mist Tile", "New Mist Tile", "Asset", "Save Mist Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MistTile>(), path);
    }
#endif
}

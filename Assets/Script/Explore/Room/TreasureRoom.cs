using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : Room
{
    protected override void SetSpace()
    {
        for (int i = Position.x; i < Position.x + Width; i++)
        {
            for (int j = Position.y; j < Position.y + Height; j++)
            {
                PositionList.Add(new Vector2Int(i, j));

                if (i == Position.x || i == Position.x + Width - 1 || j == Position.y || j == Position.y + Height - 1)
                {
                    WallList.Add(new Vector2Int(i, j));
                    TreasureDic.Add(new Vector2Int(i, j), new Treasure(RoomData.GetRandomTreasureID()));
                }
            }
        }
    }

    public override Vector2Int GetDoor()
    {
        int random = Random.Range(0, WallList.Count);
        Vector2Int wall = WallList[random];
        TreasureDic.Remove(wall);

        if (wall.x == Position.x) //left
        {
            wall += Vector2Int.left;
        }
        else if (wall.x == Position.x + Width - 1) //right
        {
            wall += Vector2Int.right;
        }
        else if (wall.y == Position.y) //down
        {
            wall += Vector2Int.down;
        }
        else if (wall.y == Position.y + Height - 1) //up
        {
            wall += Vector2Int.up;
        }

        return wall;
    }
}

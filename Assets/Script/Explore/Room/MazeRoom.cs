using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRoom : Room
{
    private Queue<Vector2Int> _spaceQueue = new Queue<Vector2Int>();
    private Vector2Int[] _directions = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down};

    public override void SetData(RoomData.RootObject roomData, DungeonData.RootObject dungeonData)
    {
        RoomData = roomData;
        DungeonData = dungeonData;
        Width = Random.Range(roomData.MinWidth, roomData.MaxWidth + 1);
        if (Width % 2 == 1)
        {
            Width += 1;
        }
        Height = Random.Range(roomData.MinHeight, roomData.MaxHeight + 1);
        if (Height % 2 == 1)
        {
            Height += 1;
        }
        _treasureAmount = Random.Range(roomData.MinTreasureAmount, roomData.MaxTreasureAmount + 1);
        _moneyAmount = Random.Range(roomData.MinMoneyAmount, roomData.MaxMoneyAmount + 1);

        WallDirectionList.Add(Vector2Int.left);
        WallDirectionList.Add(Vector2Int.right);
        WallDirectionList.Add(Vector2Int.up);
        WallDirectionList.Add(Vector2Int.down);
    }

    protected override void SetSpace()
    {
        PositionList.Add(Position);
        _spaceQueue.Enqueue(Position);
        DFS(Position);

        List<Vector2Int> tempList = new List<Vector2Int>(PositionList);
        Vector2Int moneyPosition = new Vector2Int();
        for (int i = 0; i < _moneyAmount; i++)
        {
            moneyPosition = tempList[Random.Range(0, tempList.Count)];
            tempList.Remove(moneyPosition);
            if (!WallList.Contains(moneyPosition))
            {
                MoneyDic.Add(moneyPosition, Random.Range(DungeonData.MinMoney, DungeonData.MaxMoney + 1));
            }
            else
            {
                i--;
            }
        }
    }

    private void DFS(Vector2Int walker)
    {
        Vector2Int direction = new Vector2Int();
        Vector2Int newWalker = new Vector2Int();
        List<Vector2Int> tempList = new List<Vector2Int>();

        while (_spaceQueue.Count > 0)
        {
            tempList = new List<Vector2Int>(_directions);
            while (tempList.Count > 0)
            {
                direction = tempList[Random.Range(0, tempList.Count)];
                newWalker = walker + direction * 2;
                if (newWalker.x >= Position.x && newWalker.x < Position.x + Width && newWalker.y >= Position.y && newWalker.y < Position.y + Height &&
                    !PositionList.Contains(newWalker))
                {
                    PositionList.Add(walker + direction);
                    PositionList.Add(walker + direction * 2);
                    _spaceQueue.Enqueue(walker + direction * 2);

                    if (walker.x == Position.x || walker.x == Position.x + Width - 1 || walker.y == Position.y || walker.y == Position.y + Height - 1)
                    {
                        WallList.Add(walker);
                    }

                    break;
                }
                else
                {
                    tempList.Remove(direction);
                }
            }

            if (tempList.Count > 0)
            {
                DFS(newWalker);
            }
            else //如果所有的方向都沒有路
            {
                if (TreasureDic.Count < _treasureAmount)
                {
                    TreasureDic.Add(walker, new Treasure(RoomData.GetRandomTreasureID()));
                }

                DFS(_spaceQueue.Dequeue());
            }
        }
    }
}

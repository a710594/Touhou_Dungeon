using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRoom : Room
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
                }
            }
        }

        Vector2Int treasurePosition = new Vector2Int();
        List<Vector2Int> tempList = new List<Vector2Int>(PositionList);
        for (int i=0; i<_treasureAmount; i++)
        {
            treasurePosition = tempList[Random.Range(0, tempList.Count)];
            tempList.Remove(treasurePosition);
            if (!WallList.Contains(treasurePosition))
            {
                TreasureDic.Add(treasurePosition, new Treasure(Data.GetRandomTreasureID()));
            }
            else
            {
                i--;
            }
        }

        Vector2Int moneyPosition = new Vector2Int();
        for (int i = 0; i < _moneyAmount; i++)
        {
            moneyPosition = tempList[Random.Range(0, tempList.Count)];
            tempList.Remove(moneyPosition);
            if (!WallList.Contains(moneyPosition))
            {
                MoneyDic.Add(moneyPosition, Random.Range(Data.MinMoney, Data.MaxMoney + 1));
            }
            else
            {
                i--;
            }
        }
    }
}

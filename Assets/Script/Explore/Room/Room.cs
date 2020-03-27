using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    public int Width;
    public int Height;
    public Vector2Int Position; //矩形房間的左下角
    public RoomData.RootObject Data;
    public List<Vector2Int> PositionList = new List<Vector2Int>();
    public List<Vector2Int> WallDirectionList = new List<Vector2Int>();
    public List<Vector2Int> WallList = new List<Vector2Int>();
    public List<Vector2Int> KeyList = new List<Vector2Int>();
    public Dictionary<Vector2Int, Treasure> TreasureDic = new Dictionary<Vector2Int, Treasure>(); //position, id
    public Dictionary<Vector2Int, int> MoneyDic = new Dictionary<Vector2Int, int>(); //position, money

    protected int _treasureAmount;
    protected int _moneyAmount;

    public virtual void SetData(RoomData.RootObject data)
    {
        Data = data;
        Width = Random.Range(data.MinWidth, data.MaxWidth + 1);
        Height = Random.Range(data.MinHeight, data.MaxHeight + 1);
        _treasureAmount = Random.Range(data.MinTreasureAmount, data.MaxTreasureAmount + 1);
        _moneyAmount = Random.Range(data.MinMoneyAmount, data.MaxMoneyAmount + 1);

        WallDirectionList.Add(Vector2Int.left);
        WallDirectionList.Add(Vector2Int.right);
        WallDirectionList.Add(Vector2Int.up);
        WallDirectionList.Add(Vector2Int.down);
    }

    public void SetPosition(Vector2Int position)
    {
        Position = position;
        SetSpace();
    }

    public virtual Vector2Int GetDoor()
    {
        int random = Random.Range(0, WallList.Count);
        Vector2Int wall = WallList[random];

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

    protected virtual void SetSpace()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseRoom : Room
{
    private class LittleRoom
    {
        //矩形房間的左下角
        public int PositionX;
        public int PositionY;

        public int Width;
        public int Height;
        public LittleRoom LeftChild;
        public LittleRoom RightChild;
        public LittleRoom Wall;

        public LittleRoom(int x, int y, int width, int height)
        {
            PositionX = x;
            PositionY = y;
            Width = width;
            Height = height;
        }
    }

    private int _wallAmount = 3;
    private int MinWidth = 3; //房間的最小寬度
    private int MinHeight = 3; //房間的最小長度
    private LittleRoom _firstLittleRoom;
    private List<LittleRoom> _leafList = new List<LittleRoom>();

    public HouseRoom()
    {
        Width = 15; //temp
        Height = 15; //temp

        WallDirectionList.Add(Vector2Int.left);
        WallDirectionList.Add(Vector2Int.right);
        WallDirectionList.Add(Vector2Int.up);
        WallDirectionList.Add(Vector2Int.down);
    }

    protected override void SetSpace()
    {
        _firstLittleRoom = new LittleRoom(0, 0, Width, Height);
        GenerateLittleRoom(_firstLittleRoom);
    }

    private void GenerateLittleRoom(LittleRoom room)
    {
        //if (Random.Range(0, 2) == 0) //縱的
        if (_wallAmount % 2 == 0)
        {
            int wallX = 0;
            if (GetRandomPosition(room.PositionX, room.Width, MinWidth, out wallX))
            {
                room.LeftChild = new LittleRoom(room.PositionX, room.PositionY, wallX - room.PositionX, room.Height);
                room.RightChild = new LittleRoom(wallX + 1, room.PositionY, room.PositionX + room.Width - wallX - 1, room.Height);
                room.Wall = new LittleRoom(wallX, room.PositionY, 1, room.Height);
            }
            else
            {
                Inorder(_firstLittleRoom);
                //MapPainter.Instance.Generate(_array);
            }
        }
        else //橫的
        {
            int wallY = 0;
            if (GetRandomPosition(room.PositionY, room.Height, MinHeight, out wallY))
            {
                room.LeftChild = new LittleRoom(room.PositionX, room.PositionY, room.Width, wallY - room.PositionY);
                room.RightChild = new LittleRoom(room.PositionX, wallY + 1, room.Width, room.PositionY + room.Height - wallY - 1);
                room.Wall = new LittleRoom(room.PositionX, wallY, room.Width, 1);
            }
            else
            {
                Inorder(_firstLittleRoom);
            }
        }

        if (room.LeftChild != null && room.RightChild != null)
        {
            _wallAmount--;
            if (_wallAmount == 0)
            {
                Inorder(_firstLittleRoom);
            }
            else
            {
                _leafList.Remove(room);
                _leafList.Add(room.LeftChild);
                _leafList.Add(room.RightChild);

                LittleRoom cutRoom = _leafList[Random.Range(0, _leafList.Count)];
                GenerateLittleRoom(cutRoom);
            }
        }
    }

    private bool GetRandomPosition(int startPoint, int length, int minLength, out int position)
    {
        position = 0;
        if (length > minLength * 2)
        {
            position = (int)Random.Range(startPoint + length * 0.25f, length * 0.75f);

            if (position - startPoint < minLength)
            {
                position = startPoint + minLength;
            }
            else if (startPoint + length - position < minLength)
            {
                position = startPoint + length - minLength;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Inorder(LittleRoom room)
    {
        if (room.LeftChild != null && room.RightChild != null)
        {
            Inorder(room.LeftChild);
            Inorder(room.RightChild);
            SetDoor(room.Wall.PositionX, room.Wall.PositionY, room.Wall.Width, room.Wall.Height);
        }
        else
        {
            SetRoom(room.PositionX, room.PositionY, room.Width, room.Height);
        }
    }

    private void SetRoom(int positionX, int positionY, int width, int height) //在地圖上畫出房間
    {
        for (int i = positionX; i < positionX + width; i++)
        {
            for (int j = positionY; j < positionY + height; j++)
            {
                PositionList.Add(new Vector2Int(i, j));

                if (i == Position.x || i == Position.x + Width - 1 || j == Position.y || j == Position.y + Height - 1)
                {
                    WallList.Add(new Vector2Int(i, j));
                }
            }
        }
    }

    private void SetDoor(int positionX, int positionY, int width, int height) //在地圖上畫出門
    {
        if (width < height)
        {
            PositionList.Add(new Vector2Int(positionX, Random.Range(positionY + 1, positionY + height)));
        }
        else
        {
            PositionList.Add(new Vector2Int(Random.Range(positionX + 1, positionX + width), positionY));
        }
    }
}

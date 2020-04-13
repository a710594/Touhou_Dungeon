using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility //各種和 TileMap 有關的計算方法
{
    /*
     * 取得擴散型範圍座標的方法,示意圖如下
     *    x
     *   xxx
     *  xxxxx
     *   xxx
     *    x
    */
    public static List<Vector2Int> GetRhombusPositionList(int length, Vector2Int position, bool isHollow)
    {
        int rowLength;
        List<Vector2Int> positionList = new List<Vector2Int>();

        for (int i = -length; i <= length; i++)
        {
            rowLength = length - Mathf.Abs(i);

            if (isHollow)
            {
                if (rowLength != 0)
                {
                    positionList.Add(position + Vector2Int.right * i + Vector2Int.up * -rowLength);
                    positionList.Add(position + Vector2Int.right * i + Vector2Int.up * rowLength);
                }
                else
                {
                    positionList.Add(position + Vector2Int.right * i);
                }
            }
            else
            {
                for (int j = -rowLength; j <= rowLength; j++)
                {
                    positionList.Add(position + Vector2Int.right * i + Vector2Int.up * j);
                }
            }
        }
        return positionList;
    }

    /*
     * 取得矩形範圍座標的方法,示意圖如下
     *    XXXX
     *   OXXXX
     *    XXXX
     *   O:使用技能的人 X:技能範圍
    */
    public static List<Vector2Int> GetRectanglePositionList(int length, int width, Vector2Int origin, Vector2Int direction)
    {
        Vector2Int widthDirection = Vector2Int.RoundToInt(Quaternion.AngleAxis(90, Vector3.forward) * (Vector3Int)direction);
        List<Vector2Int> positionList = new List<Vector2Int>();
        for (int i = 1; i <= length; i++)
        {
            for (int j=width / 2 * -1; j<= width / 2; j++)
            {
                positionList.Add(origin + direction * i + widthDirection * j);
            }
        }

        return positionList;
    }

    //draw line
    public static List<Vector2Int> GetLinePositionList(Vector2Int pos1, Vector2Int pos2)
    {
        List<Vector2Int> positionList = new List<Vector2Int>();
        float dx = pos2.x - pos1.x;
        float dy = pos2.y - pos1.y;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            int y;
            int fromX;
            int toX;

            if (pos1.x < pos2.x)
            {
                fromX = pos1.x;
                toX = pos2.x;
            }
            else
            {
                fromX = pos2.x;
                toX = pos1.x;
            }
            for (int x = fromX; x <= toX; x++)
            {
                y = Mathf.RoundToInt(pos1.y + dy * (x - pos1.x) / dx);
                positionList.Add(new Vector2Int(x, y));
            }
        }
        else
        {
            int x;
            int fromY;
            int toY;

            if (pos1.y < pos2.y)
            {
                fromY = pos1.y;
                toY = pos2.y;
            }
            else
            {
                fromY = pos2.y;
                toY = pos1.y;
            }
            for (int y = fromY; y <= toY; y++)
            {
                x = Mathf.RoundToInt(pos1.x + dx * (y - pos1.y) / dy);
                positionList.Add(new Vector2Int(x, y));
            }
        }

        return positionList;
    }

    //draw circle
    public static List<Vector2Int> GetCirclePositionList(Vector2Int center, int r, bool isHollow)
    {
        List<Vector2Int> positionList = new List<Vector2Int>();
        //positionList.Add(new Vector2Int(x_centre, y_centre));

        int x = r, y = 0;

        positionList.Add(new Vector2Int(r + center.x, center.y));
        positionList.Add(new Vector2Int(-r + center.x, center.y));
        positionList.Add(new Vector2Int(center.x, r + center.y));
        positionList.Add(new Vector2Int(center.x, -r + center.y));

        if (!isHollow)
        {
            for (int i = -r + center.x + 1; i <= r + center.x - 1; i++)
            {
                positionList.Add(new Vector2Int(i, center.y));
            }
        }

        // Initialising the value of P 
        int P = 1 - r;
        while (x > y)
        {

            y++;

            // Mid-point is inside or on the perimeter 
            if (P <= 0)
                P = P + 2 * y + 1;

            // Mid-point is outside the perimeter 
            else
            {
                x--;
                P = P + 2 * y - 2 * x + 1;
            }

            // All the perimeter points have already  
            // been printed 
            if (x < y)
                break;

            // Printing the generated point and its  
            // reflection in the other octants after 
            // translation 
            positionList.Add(new Vector2Int(x + center.x, y + center.y));
            positionList.Add(new Vector2Int(-x + center.x, y + center.y));
            if (!isHollow)
            {
                for (int i= -x + center.x + 1; i<= x + center.x - 1; i++)
                {
                    positionList.Add(new Vector2Int(i, y + center.y));
                }
            }
            positionList.Add(new Vector2Int(x + center.x, -y + center.y));
            positionList.Add(new Vector2Int(-x + center.x, -y + center.y));
            if (!isHollow)
            {
                for (int i = -x + center.x + 1; i <= x + center.x - 1; i++)
                {
                    positionList.Add(new Vector2Int(i, -y + center.y));
                }
            }

            // If the generated point is on the  
            // line x = y then the perimeter points 
            // have already been printed 
            if (x != y)
            {
                positionList.Add(new Vector2Int(y + center.x, x + center.y));
                positionList.Add(new Vector2Int(-y + center.x, x + center.y));
                if (!isHollow)
                {
                    for (int i = -y + center.x + 1; i <= y + center.x - 1; i++)
                    {
                        positionList.Add(new Vector2Int(i, x + center.y));
                    }
                }
                positionList.Add(new Vector2Int(y + center.x, -x + center.y));
                positionList.Add(new Vector2Int(-y + center.x, -x + center.y));
                if (!isHollow)
                {
                    for (int i = -y + center.x + 1; i <= y + center.x - 1; i++)
                    {
                        positionList.Add(new Vector2Int(i, -x + center.y));
                    }
                }
            }
        }
        return positionList;
    }

    //在棋盤上要走幾步
    public static int GetDistance(Vector2 pos1, Vector2 pos2)
    {
        return (int)(Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y));
    }

    public static BoundsInt GetMapBounds(List<Vector2Int> positionList)
    {
        BoundsInt bounds = new BoundsInt();

        bounds.xMin = positionList[0].x;
        bounds.xMax = positionList[0].x;
        bounds.yMin = positionList[0].y;
        bounds.yMax = positionList[0].y;

        for (int i = 1; i < positionList.Count; i++)
        {
            if (positionList[i].x < bounds.xMin)
            {
                bounds.xMin = positionList[i].x;
            }
            if (positionList[i].x > bounds.xMax)
            {
                bounds.xMax = positionList[i].x;
            }
            if (positionList[i].y < bounds.yMin)
            {
                bounds.yMin = positionList[i].y;
            }
            if (positionList[i].y > bounds.yMax)
            {
                bounds.yMax = positionList[i].y;
            }
        }

        return bounds;
    }

    public static int NormalDistributionRandom(int min, int max)
    {
        float random1 = Random.Range(min, max);
        float random2 = Random.Range(min, max);
        float random3 = Random.Range(min, max);
        return Mathf.RoundToInt((random1 + random2 + random3) / 3f);
    }

    public static List<Vector2Int> CutFrontAndTail(List<Vector2Int> list) //list data 去頭去尾
    {
        list.RemoveAt(0);
        list.RemoveAt(list.Count -1);

        return list;
    }

    //public static BattleCharacter GetCharacterByPosition(Vector2 position, List<BattleCharacter> characterList) //取得該格子上的角色
    //{
    //    for (int i = 0; i < characterList.Count; i++)
    //    {
    //        if ((Vector2)characterList[i].transform.position == position)
    //        {
    //            return characterList[i];
    //        }
    //    }
    //    return null;
    //}
}

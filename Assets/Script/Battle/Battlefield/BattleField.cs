using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField
{
    public Vector2Int Position;
    public int MoveCost;
    public string Name;
    public string Comment;
    public Buff Buff = null;

    public BattleField(Vector2Int position, BattleTileData.RootObject data)
    {
        Position = position;
        MoveCost = data.MoveCost;
        Name = data.Name;
        Comment = data.Comment;
    }

    public void SetData(BattleTileData.RootObject data)
    {
        MoveCost = data.MoveCost;
        Name = data.Name;
        Comment = data.Comment;
    }

    public void SetBuff(int id)
    {
        if (Buff == null)
        {
            BattleController.Instance.TurnEndHandler += CheckBuffCD;
        }

        BattleStatusData.RootObject data = BattleStatusData.GetData(id);
        Buff = new Buff(id);
        TilePainter.Instance.Painting(data.Field, 1, Position);
    }

    public void CheckBuffCD()
    {
        if (Buff.RemainTurn != -1) //-1代表永久
        {
            Buff.RemainTurn--;
            if (Buff.RemainTurn == 0)
            {
                Buff = null;
                BattleController.Instance.TurnEndHandler -= CheckBuffCD;
                TilePainter.Instance.Clear(1, Position);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField
{
    public Vector2Int Position;
    public int MoveCost; 
    public Buff Buff = null;

    public BattleField(Vector2Int position, int moveCost)
    {
        Position = position;
        MoveCost = moveCost;
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

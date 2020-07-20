using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField
{
    public Vector2Int Position;
    public int MoveCost;
    public string Name;
    public string TileName; //畫地圖用的
    public string BuffTileName; //畫地圖用的
    public string Comment;
    public BattleStatus Status = null;

    public BattleField() { }

    public BattleField(Vector2Int position, BattleTileData.RootObject data)
    {
        Position = position;
        MoveCost = data.MoveCost;
        Name = data.GetName();
        TileName = data.TileName;
        Comment = data.GetComment();
    }

    public void SetData(BattleTileData.RootObject data)
    {
        MoveCost = data.MoveCost;
        Name = data.GetName();
        TileName = data.TileName;
        Comment = data.GetComment();
    }

    public void SetBuff(int id, int lv)
    {
        if (Status == null)
        {
            BattleController.Instance.TurnEndHandler += CheckRemainTurn;
        }

        BattleStatusData.RootObject data = BattleStatusData.GetData(id);
        if ((int)data.ValueType <= 7)
        {
            Status = new Buff(id, lv);
        }
        else if (data.ValueType == BattleStatusData.TypeEnum.NoDamage)
        {
            Status = new NoDamage(id);
        }
        BuffTileName = data.Field;
        TilePainter.Instance.Painting(data.Field, 1, Position);
    }

    public void CheckRemainTurn()
    {
        if (Status.RemainTurn != -1) //-1代表永久
        {
            Status.RemainTurn--;
            if (Status.RemainTurn == 0)
            {
                Status = null;
                BattleController.Instance.TurnEndHandler -= CheckRemainTurn;
                TilePainter.Instance.Clear(1, Position);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatusScrollItem : ScrollItem
{
    public Text CommentLabel;
    public Text TurnLabel;
    public Image Icon;

    public override void SetData(object obj)
    {
        base.SetData(obj);
        BattleStatus data = (BattleStatus)_data;
        Icon.overrideSprite = Resources.Load<Sprite>("Image/" + data.Icon);
        CommentLabel.text = data.Comment;
        if (data.RemainTurn != -1)
        {
            TurnLabel.text = data.RemainTurn.ToString() + "回合";
        }
        else
        {
            TurnLabel.text = "∞回合";
        }
    }
}

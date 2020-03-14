using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldUI : MonoBehaviour
{
    public Text NameLabel;
    public Text MoveCostLabel;
    public Text CommentLabel;
    public Text BuffCommentLabel;
    public GameObject BuffGroup;

    public void SetVisible(bool isVisible) 
    {
        gameObject.SetActive(isVisible);
    }

    public void SetData(BattleField battleField) 
    {
        NameLabel.text = battleField.Name;
        MoveCostLabel.text = "移動消耗:" + battleField.MoveCost.ToString();
        CommentLabel.text = battleField.Comment;

        if (battleField.Buff != null)
        {
            BuffGroup.SetActive(true);
            BuffCommentLabel.text = battleField.Buff.Comment;
        }
        else
        {
            BuffGroup.SetActive(false);
        }
    }
}

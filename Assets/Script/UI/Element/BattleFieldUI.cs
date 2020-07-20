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
        MoveCostLabel.text =  LanguageData.GetText(9, LanguageSystem.Instance.CurrentLanguage) + ":" + battleField.MoveCost.ToString(); //移動消耗
        CommentLabel.text = battleField.Comment;

        if (battleField.Status != null)
        {
            BuffGroup.SetActive(true);
            BuffCommentLabel.text = battleField.Status.Comment;
        }
        else
        {
            BuffGroup.SetActive(false);
        }
    }
}

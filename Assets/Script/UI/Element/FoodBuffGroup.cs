using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodBuffGroup : MonoBehaviour
{
    public Text FoodNameLabel;
    public Text CommentLabel;
    public Button CloseButton;

    public void Open(FoodBuff foodBuff)
    {
        gameObject.SetActive(true);
        FoodNameLabel.text = foodBuff.FoodName;
        CommentLabel.text = foodBuff.Comment;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        CloseButton.onClick.AddListener(Close);
    }
}

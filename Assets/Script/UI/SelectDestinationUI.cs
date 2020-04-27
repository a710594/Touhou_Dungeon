using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDestinationUI : MonoBehaviour
{
    public Button CloseButton;
    public LoopScrollView ScrollView;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    private void FloorOnClick(object floor)
    {
        PrepareUI.Open((int)floor);
    }

    private void CloseOnClick()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        CloseButton.onClick.AddListener(CloseOnClick);
        ScrollView.SetData(new ArrayList { 1 });
        ScrollView.AddClickHandler(FloorOnClick);
    }
}

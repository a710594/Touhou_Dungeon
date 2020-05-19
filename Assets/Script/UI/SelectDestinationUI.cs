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
        SetData();
    }

    private void SetData()
    {
        List<int> floorList = new List<int>();
        for (int i=1; i<=ExploreController.Instance.ArriveFloor; i++)
        {
            floorList.Add(i);
        }
        ScrollView.SetData(new ArrayList (floorList));
        ScrollView.AddClickHandler(FloorOnClick);
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
    }
}

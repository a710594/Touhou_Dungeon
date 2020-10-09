using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDestinationUI : MonoBehaviour
{
    public Button CloseButton;
    public LoopScrollView GroupScrollView;
    public LoopScrollView FloorScrollView;

    public void Open()
    {
        gameObject.SetActive(true);
        GroupScrollView.gameObject.SetActive(true);
        FloorScrollView.gameObject.SetActive(false);
        SetData();
    }

    private void SetData()
    {
        List<DungeonGroupData.RootObject> groupList = DungeonGroupData.GetGroupList(ExploreController.Instance.ArriveFloor);
        GroupScrollView.SetData(new ArrayList(groupList));

        //List<int> floorList = new List<int>();
        //for (int i=1; i<=ExploreController.Instance.ArriveFloor; i++)
        //{
        //    floorList.Add(i);
        //}
        //FloorScrollView.SetData(new ArrayList (floorList));
    }

    private void GroupOnClick(object obj) 
    {
        int group = (int)obj;
        List<DungeonData.RootObject> floorList = DungeonData.GetFloorList(group, ExploreController.Instance.ArriveFloor);
        FloorScrollView.SetData(new ArrayList(floorList));
        FloorScrollView.gameObject.SetActive(true);
        GroupScrollView.gameObject.SetActive(false);
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
        GroupScrollView.AddClickHandler(GroupOnClick);
        FloorScrollView.AddClickHandler(FloorOnClick);
        CloseButton.onClick.AddListener(CloseOnClick);
    }
}

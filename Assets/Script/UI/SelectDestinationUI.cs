using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDestinationUI : MonoBehaviour
{
    public Text GroupLabel;
    public Button CloseButton;
    public LoopScrollView GroupScrollView;
    public LoopScrollView FloorScrollView;

    public void Open()
    {
        gameObject.SetActive(true);
        GroupScrollView.gameObject.SetActive(true);
        FloorScrollView.gameObject.SetActive(false);
        SetData();
        GroupLabel.text = string.Empty;
    }

    private void SetData()
    {
        List<DungeonGroupData.RootObject> groupList = DungeonGroupData.GetGroupList(ExploreController.Instance.ArriveFloor);
        GroupScrollView.SetData(new ArrayList(groupList));
    }

    private void GroupOnClick(object obj) 
    {
        DungeonGroupData.RootObject group = (DungeonGroupData.RootObject)obj;
        List<DungeonData.RootObject> floorList = DungeonData.GetFloorList(group.ID, ExploreController.Instance.ArriveFloor);
        FloorScrollView.SetData(new ArrayList(floorList));
        FloorScrollView.gameObject.SetActive(true);
        GroupScrollView.gameObject.SetActive(false);
        GroupLabel.text = group.Name;
    }

    private void FloorOnClick(object floor)
    {
        PrepareUI.Open(((DungeonData.RootObject)floor).ID);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDestinationUI : MonoBehaviour
{
    public Text GroupLabel;
    public Button MapCloseButton;
    public Button FloorCloseButton;
    //public LoopScrollView GroupScrollView;
    public LoopScrollView FloorScrollView;
    public GameObject MapGroup;
    public TextButton[] MapButtons;

    public void Open()
    {
        gameObject.SetActive(true);
        //GroupScrollView.gameObject.SetActive(true);
        MapGroup.SetActive(true);
        FloorScrollView.gameObject.SetActive(false);
        SetData();
        GroupLabel.text = string.Empty;
    }

    private void SetData()
    {
        List<DungeonGroupData.RootObject> groupList = DungeonGroupData.GetGroupList(ExploreController.Instance.ArriveFloor);
        //GroupScrollView.SetData(new ArrayList(groupList));
        for (int i=0; i<MapButtons.Length; i++)
        {
            if (i < groupList.Count)
            {
                MapButtons[i].SetData(groupList[i].Name, groupList[i]);
                MapButtons[i].gameObject.SetActive(true);
            }
            else
            {
                MapButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void MapOnClick(object obj) 
    {
        DungeonGroupData.RootObject group = (DungeonGroupData.RootObject)obj;
        List<DungeonData.RootObject> floorList = DungeonData.GetFloorList(group.ID, ExploreController.Instance.ArriveFloor);
        FloorScrollView.SetData(new ArrayList(floorList));
        FloorScrollView.gameObject.SetActive(true);
        //GroupScrollView.gameObject.SetActive(false);
        MapGroup.SetActive(false);
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
        //GroupScrollView.AddClickHandler(GroupOnClick);
        for (int i=0; i<MapButtons.Length; i++)
        {
            MapButtons[i].OnClickHandler = MapOnClick;
        }
        FloorScrollView.AddClickHandler(FloorOnClick);
        MapCloseButton.onClick.AddListener(CloseOnClick);
        FloorCloseButton.onClick.AddListener(CloseOnClick);
    }
}

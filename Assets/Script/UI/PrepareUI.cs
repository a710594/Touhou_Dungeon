using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareUI : MonoBehaviour
{
    public static PrepareUI Instance;

    public Button GoButton;
    public Button CloseButton;
    public LoopScrollView WarehouseScrollView;
    public LoopScrollView BagScrollView;

    private int _targetFloor;

    public static void Open(int targetFloor)
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("PrepareUI", ResourceManager.Type.UI).GetComponent<PrepareUI>();
        }

        Instance.Init(targetFloor);
    }

    public static void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(int targetFloor)
    {
        _targetFloor = targetFloor;
        ItemManager.Instance.AddItem(0, 1, ItemManager.Type.Bag);
        ItemManager.Instance.MinusItem(0, 1, ItemManager.Type.Warehouse);
        SetData();
    }

    private void SetData()
    {
        WarehouseScrollView.SetData(new ArrayList(ItemManager.Instance.GetItemDicByType(ItemManager.Type.Warehouse, ItemData.TypeEnum.All)));
        WarehouseScrollView.AddClickHandler(WarehouseIconOnClick);
        BagScrollView.SetData(new ArrayList(ItemManager.Instance.GetItemDicByType(ItemManager.Type.Bag, ItemData.TypeEnum.All)));
        BagScrollView.AddClickHandler(BagIconOnClick);
    }

    private void WarehouseIconOnClick(object obj)
    {
        ItemManager.Instance.MinusItem(obj, 1, ItemManager.Type.Warehouse);
        ItemManager.Instance.AddItem(obj, 1, ItemManager.Type.Bag);

        SetData();
    }

    private void BagIconOnClick(object obj)
    {
        if (obj is int && (int)obj == 0) //如果道具為緊急逃脫裝置,則不得移除,因為它是必備的道具
        {
            ConfirmUI.Open("必需攜帶緊急逃脫裝置。", "確定", null);
        }
        else
        {
            ItemManager.Instance.MinusItem(obj, 1, ItemManager.Type.Bag);
            ItemManager.Instance.AddItem(obj, 1, ItemManager.Type.Warehouse);
            SetData();
        }
    }

    private void GoOnClick()
    {
        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
        {
            LoadingUI.Instance.Open(()=> 
            {
                ExploreController.Instance.GenerateFloor(_targetFloor);
            });
        });
    }

    private void CloseOnClick()
    {
        ItemManager.Instance.PutBagItemIntoWarehouse();
        Close();
    }

    void Awake()
    {
        CloseButton.onClick.AddListener(CloseOnClick);
        GoButton.onClick.AddListener(GoOnClick);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCookUI : MonoBehaviour
{
    public static NewCookUI Instance;

    public LoopScrollView ScrollView;
    public Text NameLabel;
    public Text CommentLabel;
    public Text VolumeLabel;
    public Text ResultNameLabel;
    public Text ResultCommentLabel;
    public Image ItemImage;
    public Image ResultItemImage;
    public Button MakeButton;
    public Button CloseButton;
    public ButtonPlus[] MaterialButtons;

    private ItemManager.Type _managerType;
    private List<Item> MaterialList = new List<Item>();

    public static void Open(ItemManager.Type type)
    {
        ExploreController.Instance.StopEnemy();
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("NewCookUI", ResourceManager.Type.UI).GetComponent<NewCookUI>();
        }

        Instance.Init(type);
    }

    public static void Close()
    {
        ExploreController.Instance.ContinueEnemy();
        ExploreUI.SetCanMove();
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(ItemManager.Type type) //一般背包用
    {
        _managerType = type;

        SetScrollView(false);
        ClearInfo();
    }

    private void SetScrollView(bool isRefresh)
    {
        List<Item> itemList = new List<Item>();
        itemList = ItemManager.Instance.GetItemListByType(_managerType, ItemData.TypeEnum.All);

        if (isRefresh)
        {
            ScrollView.Refresh(new ArrayList(itemList));
        }
        else
        {
            ScrollView.SetData(new ArrayList(itemList));
        }
    }

    private void IconOnClick(object obj) 
    {
        Item item = (Item)obj;

        CommentLabel.text = item.Comment;
        VolumeLabel.text = "體積：" + item.Volume;
        ItemImage.gameObject.SetActive(true);
        ItemImage.overrideSprite = Resources.Load<Sprite>("Image/Item/" + item.Icon);

        if (MaterialList.Count < MaterialButtons.Length)
        {
            Debug.Log(item.Name);
            MaterialList.Add(item);
            MaterialButtons[MaterialList.Count - 1].SetData(item);
            MaterialButtons[MaterialList.Count - 1].Image.overrideSprite = Resources.Load<Sprite>("Image/Item/" + item.Icon);
        }

        Food food = NewCookData.GetFood(MaterialList);
        if (food != null)
        {
            ResultNameLabel.text = food.Name;
            ResultCommentLabel.text = food.Comment;
            ResultItemImage.gameObject.SetActive(true);
            ResultItemImage.overrideSprite = Resources.Load<Sprite>("Image/Item/" + food.Icon);
        }

        ItemManager.Instance.MinusItem(item.ID, 1, _managerType);
        SetScrollView(true);
    }

    private void MaterialOnClick(ButtonPlus button) 
    {
        Item item = (Item)button.Data;
        Debug.Log(item.Name);
        if (item != null)
        {
            MaterialList.Remove(item);
            for (int i = 0; i < MaterialButtons.Length; i++)
            {
                if (i < MaterialList.Count)
                {
                    MaterialButtons[i].SetData(MaterialList[i]);
                    MaterialButtons[i].Image.overrideSprite = Resources.Load<Sprite>("Image/Item/" + MaterialList[i].Icon);
                }
                else
                {
                    MaterialButtons[i].SetData(null);
                    MaterialButtons[i].Image.overrideSprite = null;
                }
            }
        }

        ItemManager.Instance.AddItem(item.ID, 1, _managerType);
        SetScrollView(true);
    }

    private void CloseOnClick()
    {
        Close();
    }

    private void ClearInfo()
    {
        NameLabel.text = "";
        CommentLabel.text = "";
        ItemImage.gameObject.SetActive(false);
        MakeButton.gameObject.SetActive(false);
    }

    void Awake()
    {
        MakeButton.gameObject.SetActive(false);

        CloseButton.onClick.AddListener(CloseOnClick);
        //MakeButton.onClick.AddListener(UseOnClick);
        ScrollView.AddClickHandler(IconOnClick);

        for (int i=0; i<MaterialButtons.Length; i++) 
        {
            MaterialButtons[i].ClickHandler = MaterialOnClick;
        }
    }
}

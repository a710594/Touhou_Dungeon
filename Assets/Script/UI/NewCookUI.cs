using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCookUI : MonoBehaviour
{
    public static NewCookUI Instance;

    public LoopScrollView ScrollView;
    public Text ResultNameLabel;
    public Text ResultCommentLabel;
    public Image ResultItemImage;
    public Button MakeButton;
    public Button CloseButton;
    public TipLabel TipLabel;
    public ButtonPlus[] MaterialButtons;

    private Food _food;
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
        List<Item> itemList = ItemManager.Instance.GetCanCookList(_managerType);

        if (isRefresh)
        {
            ScrollView.Refresh(new ArrayList(itemList));
        }
        else
        {
            ScrollView.SetData(new ArrayList(itemList));
        }
    }


    private void SetResult()
    {
        _food = NewCookData.GetFood(MaterialList);
        if (_food != null)
        {
            ResultNameLabel.text = _food.Name;
            ResultCommentLabel.text = _food.Comment;
            ResultItemImage.gameObject.SetActive(true);
            ResultItemImage.overrideSprite = Resources.Load<Sprite>("Image/Item/" + _food.Icon);
            MakeButton.gameObject.SetActive(true);
        }
        else
        {
            ResultNameLabel.text = string.Empty;
            ResultCommentLabel.text = string.Empty;
            ResultItemImage.gameObject.SetActive(false);
            MakeButton.gameObject.SetActive(false);
        }
    }

    private void IconOnClick(object obj) 
    {
        Item item = (Item)obj;

        ItemManager.Instance.MinusItem(item.ID, 1, _managerType);
        SetScrollView(true);

        if (MaterialList.Count < MaterialButtons.Length)
        {
            MaterialList.Add(item);
            MaterialButtons[MaterialList.Count - 1].SetData(item);
            MaterialButtons[MaterialList.Count - 1].Image.overrideSprite = Resources.Load<Sprite>("Image/Item/" + item.Icon);
        }

        SetResult();
    }

    private void MaterialOnClick(ButtonPlus button) 
    {
        Item item = (Item)button.Data;

        if (item != null)
        {
            ItemManager.Instance.AddItem(item.ID, 1, _managerType);
            SetScrollView(true);

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
            SetResult();
        }
    }

    private void MakeOnClick()
    {
        ItemManager.Instance.AddFood(_food, _managerType);

        MaterialList.Clear();
        for (int i = 0; i < MaterialButtons.Length; i++)
        {
            MaterialButtons[i].SetData(null);
            MaterialButtons[i].Image.overrideSprite = null;
        }

        TipLabel.SetLabel("獲得！");

        SetResult();
        ClearInfo();
    }

    private void CloseOnClick()
    {
        ExploreController.Instance.ContinueEnemy();
        ExploreUI.SetCanMove();

        for (int i=0; i<MaterialList.Count; i++)
        {
            ItemManager.Instance.AddItem(MaterialList[i].ID, 1, _managerType);
        }

        Close();
    }

    private void ClearInfo()
    {
        ResultNameLabel.text = string.Empty;
        ResultCommentLabel.text = string.Empty;
        ResultItemImage.gameObject.SetActive(false);
        MakeButton.gameObject.SetActive(false);
    }

    void Awake()
    {
        MakeButton.gameObject.SetActive(false);

        CloseButton.onClick.AddListener(CloseOnClick);
        MakeButton.onClick.AddListener(MakeOnClick);
        ScrollView.AddClickHandler(IconOnClick);

        for (int i=0; i<MaterialButtons.Length; i++) 
        {
            MaterialButtons[i].ClickHandler = MaterialOnClick;
        }
    }
}

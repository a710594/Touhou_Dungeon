using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookUI : MonoBehaviour
{
    public static CookUI Instance;

    public Button ProduceButton;
    public Button CloseButton;
    public Text CommentLabel;
    public Text AmountLabel;
    public LoopScrollView MenuScrollView;
    public IconCard[] MaterialIcons;

    private ItemManager.Type _itemManagerType;
    private CookData.RootObject _selectedData = null;

    public static void Open(ItemManager.Type type)
    {
        Time.timeScale = 0;
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("CookUI", ResourceManager.Type.UI).GetComponent<CookUI>();
        }
        Instance.Init(type);
    }

    public static void Close()
    {
        Time.timeScale = 1;
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(ItemManager.Type type)
    {
        _itemManagerType = type;
        _selectedData = null;

        MenuScrollView.SetData(new ArrayList(CookData.GetAllData()));
        MenuScrollView.AddClickHandler(MenuOnClick);
    }

    private void SetData(CookData.RootObject cookData)
    {
        int have;
        int need;
        ItemData.RootObject itemData;

        ProduceButton.interactable = true;
        if (cookData != null)
        {
            _selectedData = cookData;

            for (int i = 0; i < MaterialIcons.Length; i++)
            {
                if (i < cookData.MaterialList.Count)
                {
                    have = ItemManager.Instance.GetItemAmount(cookData.MaterialList[i], _itemManagerType);
                    need = cookData.AmountList[i];
                    MaterialIcons[i].gameObject.SetActive(true);
                    MaterialIcons[i].Init(cookData.MaterialList[i], have, need);

                    if (have < need)
                    {
                        ProduceButton.interactable = false;
                    }
                }
                else
                {
                    MaterialIcons[i].gameObject.SetActive(false);
                }
            }

            itemData = ItemData.GetData(cookData.ResultID);

            if (ItemManager.Instance.CurrentBagVolume + itemData.Volume > ItemManager.Instance.MaxBagVolume)
            {
                ProduceButton.interactable = false;
            }

            CommentLabel.text = itemData.Comment;
            AmountLabel.text = "數量：" + ItemManager.Instance.GetItemAmount(itemData.ID, _itemManagerType).ToString();
        }
        else
        {
            CommentLabel.text = string.Empty;
            AmountLabel.text = string.Empty;

            for (int i = 0; i < MaterialIcons.Length; i++)
            {
                ProduceButton.interactable = false;
                MaterialIcons[i].gameObject.SetActive(false);
            }
        }
    }

    private void MenuOnClick(object obj)
    {
        CookData.RootObject data = (CookData.RootObject)obj;
        SetData(data);
    }

    private void ProduceOnClick()
    {
        ItemData.RootObject data = ItemData.GetData(_selectedData.ResultID);

        ConsumeItem(_selectedData.MaterialList, _selectedData.AmountList);
        ItemManager.Instance.AddItem(_selectedData.ResultID, 1, _itemManagerType);

        SetData(_selectedData);
    }

    private void ConsumeItem(List<int> materialList, List<int> amountList)
    {
        for (int i = 0; i < materialList.Count; i++)
        {
            ItemManager.Instance.MinusItem(materialList[i], amountList[i], _itemManagerType);
        }
    }

    void Awake()
    {
        CommentLabel.text = string.Empty;
        AmountLabel.text = string.Empty;
        ProduceButton.interactable = false;

        for (int i=0; i<MaterialIcons.Length; i++)
        {
            MaterialIcons[i].gameObject.SetActive(false);
        }

        ProduceButton.onClick.AddListener(ProduceOnClick);
        CloseButton.onClick.AddListener(Close);
    }
}

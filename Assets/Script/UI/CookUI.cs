using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookUI : MonoBehaviour
{
    public static CookUI Instance;

    public ButtonPlus ProduceButton;
    public Button CloseButton;
    public Text CommentLabel;
    public Text AmountLabel;
    public LoopScrollView MenuScrollView;
    public TipLabel TipLabel;
    public Text[] MaterialNameLabel;
    public Text[] MaterialAmountLabel;

    private bool _canProduce = false;
    private string _tipText;
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

        ProduceButton.SetColor(Color.white);
        _canProduce = true;
        if (cookData != null)
        {
            _selectedData = cookData;

            for (int i = 0; i < MaterialNameLabel.Length; i++)
            {
                if (i < cookData.MaterialList.Count)
                {
                    have = ItemManager.Instance.GetItemAmount(cookData.MaterialList[i], _itemManagerType);
                    need = cookData.AmountList[i];
                    MaterialNameLabel[i].gameObject.SetActive(true);
                    MaterialNameLabel[i].text = ItemData.GetData(cookData.MaterialList[i]).GetName();
                    MaterialAmountLabel[i].gameObject.SetActive(true);
                    MaterialAmountLabel[i].text = have.ToString() + "/" + need.ToString();

                    if (have < need)
                    {
                        ProduceButton.SetColor(Color.grey);
                        _canProduce = false;
                        _tipText = "材料不足";
                    }
                }
                else
                {
                    MaterialNameLabel[i].gameObject.SetActive(false);
                    MaterialAmountLabel[i].gameObject.SetActive(false);
                }
            }

            itemData = ItemData.GetData(cookData.ResultID);

            if (ItemManager.Instance.CurrentBagVolume + itemData.Volume > ItemManager.Instance.MaxBagVolume)
            {
                ProduceButton.SetColor(Color.grey);
                _canProduce = false;
                _tipText = "背包已滿";
            }

            ProduceButton.gameObject.SetActive(true);
            CommentLabel.text = itemData.GetComment();
            AmountLabel.text = "數量：" + ItemManager.Instance.GetItemAmount(itemData.ID, _itemManagerType).ToString();
        }
        else
        {
            ProduceButton.gameObject.SetActive(false);
            CommentLabel.text = string.Empty;
            AmountLabel.text = string.Empty;

            for (int i = 0; i < MaterialNameLabel.Length; i++)
            {
                MaterialNameLabel[i].gameObject.SetActive(false);
                MaterialAmountLabel[i].gameObject.SetActive(false);
            }
        }
    }

    private void MenuOnClick(object obj)
    {
        CookData.RootObject data = (CookData.RootObject)obj;
        SetData(data);
    }

    private void ProduceOnClick(object obj)
    {
        if (_canProduce)
        {
            ItemData.RootObject data = ItemData.GetData(_selectedData.ResultID);

            ConsumeItem(_selectedData.MaterialList, _selectedData.AmountList);
            ItemManager.Instance.AddItem(_selectedData.ResultID, 1, _itemManagerType);

            SetData(_selectedData);
        }
        else
        {
            TipLabel.SetLabel(_tipText);
        }
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
        ProduceButton.gameObject.SetActive(false);

        for (int i=0; i< MaterialNameLabel.Length; i++)
        {
            MaterialNameLabel[i].gameObject.SetActive(false);
            MaterialAmountLabel[i].gameObject.SetActive(false);
        }

        ProduceButton.ClickHandler = ProduceOnClick;
        CloseButton.onClick.AddListener(Close);
    }
}

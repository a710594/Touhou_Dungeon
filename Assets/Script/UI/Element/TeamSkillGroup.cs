using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSkillGroup : MonoBehaviour
{
    private static readonly int _bookId = 1007;

    public Text NameLabel;
    public Text LvLabel;
    public Text DamageLabel;
    public Text TargetLabel;
    public Text PriorityLabel;
    public Text MPLabel;
    public Text CDLabel;
    public Text AddPowerLabel;
    public Text NeedPowerLabel;
    public Text CommentLabel;
    public Text ItemAmountLabel;
    public Button UpgradeButton;
    public TipLabel TipLabel;
    public LoopScrollView SkillScrollView;

    private bool _isSpellCard;
    private int _needItemAmount;
    private int _currentItemAmount;
    private TeamMember _member;
    private SkillData.RootObject _data;
    private ItemManager.Type _itemManagerType;

    public void Init(TeamMember member, bool isSpellCard)
    {
        Clear();

        _member = member;
        _isSpellCard = isSpellCard;

        //if (!isSpellCard)
        //{
        //    List<TeamSkillScrollItem.UnlockData> dataList = new List<TeamSkillScrollItem.UnlockData>();
        //    foreach (KeyValuePair<int, int> item in member.Data.SkillUnlockDic)
        //    {
        //        dataList.Add(new TeamSkillScrollItem.UnlockData(member.Data, SkillData.GetData(item.Key), member.Lv));
        //    }
        //    SkillScrollView.SetData(new ArrayList(dataList));
        //}
        //else
        //{
        //    SkillScrollView.SetData(new ArrayList(member.Data.SpellCardList));
        //}
        if (isSpellCard)
        {
            SkillScrollView.SetData(new ArrayList(member.SpellCardDic));
        }
        else
        {
            SkillScrollView.SetData(new ArrayList(member.SkillDic));
        }
        SkillScrollView.AddClickHandler(SkillOnClick);

        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Villiage)
        {
            _itemManagerType = ItemManager.Type.Warehouse;
        }
        else
        {
            _itemManagerType = ItemManager.Type.Bag;
        }
    }

    private void SetData(SkillData.RootObject data, int lv)
    {
        _data = data;
        _needItemAmount = lv;
        _currentItemAmount = ItemManager.Instance.GetItemAmount(_bookId, _itemManagerType);
        NameLabel.text = _data.GetName();
        LvLabel.text = "Lv." + lv;
        MPLabel.text = "MP：" + _data.MP;
        CDLabel.text = "冷卻：" + _data.CD;
        PriorityLabel.text = "行動速度：" + _data.Priority;
        AddPowerLabel.text = "增加 Power：" + _data.AddPower;
        NeedPowerLabel.text = "需要 Power：" + _data.NeedPower;
        CommentLabel.text = _data.GetComment();

        if (_data.ValueList.Count > 0)
        {
            if (_data.Type == SkillData.TypeEnum.Attack)
            {
                DamageLabel.text = "傷害：" + _data.ValueList[lv - 1];
            }
            else
            {
                DamageLabel.text = "回復：" + _data.ValueList[lv - 1];
            }
        }
        else
        {
            DamageLabel.text = "傷害：" + 0;
        }
        if (_data.Target == SkillData.TargetType.Us)
        {
            TargetLabel.text = "目標：我方";
        }
        else if (_data.Target == SkillData.TargetType.Them)
        {
            TargetLabel.text = "目標：敵方";
        }
        else if (_data.Target == SkillData.TargetType.All)
        {
            TargetLabel.text = "目標：全體";
        }

        if (lv < 5)
        {
            ItemAmountLabel.gameObject.SetActive(true);
            UpgradeButton.gameObject.SetActive(true);

            if (_needItemAmount > _currentItemAmount)
            {
                UpgradeButton.GetComponent<Image>().color = Color.gray;
                ItemAmountLabel.text = "<color=#FF0000>" + _currentItemAmount + "</color>/" + _needItemAmount;
            }
            else
            {
                UpgradeButton.GetComponent<Image>().color = Color.white;
                ItemAmountLabel.text = _currentItemAmount + "/" + _needItemAmount;
            }
        }
        else
        {
            ItemAmountLabel.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);
        }
    }

    private void Clear()
    {
        NameLabel.text = string.Empty;
        LvLabel.text = string.Empty;
        DamageLabel.text = string.Empty;
        TargetLabel.text = string.Empty;
        PriorityLabel.text = string.Empty;
        MPLabel.text = string.Empty;
        CDLabel.text = string.Empty;
        AddPowerLabel.text = string.Empty;
        NeedPowerLabel.text = string.Empty;
        CommentLabel.text = string.Empty;
        UpgradeButton.gameObject.SetActive(false);
        ItemAmountLabel.gameObject.SetActive(false);
    }

    private void SkillOnClick(object obj)
    {
        KeyValuePair<SkillData.RootObject, int> pair = (KeyValuePair<SkillData.RootObject, int>)obj;
        SetData(pair.Key, pair.Value);
    }

    private void UpgradeOnClick()
    {
        if (_needItemAmount > _currentItemAmount)
        {
            TipLabel.SetLabel("素材不足");
        }
        else
        {
            int currentLv;
            if (_data.NeedPower > 0) //Is SpellCard
            {
                _member.SpellCardLvUp(_data.ID);
                currentLv = _member.SpellCardDic[_data.ID];
            }
            else
            {
                _member.SkillLvUp(_data.ID);
                currentLv = _member.SkillDic[_data.ID];
            }
            ItemManager.Instance.MinusItem(_bookId, currentLv - 1, _itemManagerType);
            SetData(_data, currentLv);

            if (_isSpellCard)
            {
                SkillScrollView.SetData(new ArrayList(_member.SpellCardDic));
            }
            else
            {
                SkillScrollView.SetData(new ArrayList(_member.SkillDic));
            }
            SkillScrollView.AddClickHandler(SkillOnClick);
        }
    }

    private void Awake()
    {
        Clear();

        UpgradeButton.onClick.AddListener(UpgradeOnClick);
    }
}

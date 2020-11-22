using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSkillGroup : MonoBehaviour
{
    private static readonly int _bookId_1 = 1011;
    private static readonly int _bookId_2 = 1012;
    private static readonly int _bookId_3 = 1013;

    public Text NameLabel;
    public Text LvLabel;
    public Text DamageLabel;
    public Text HitRateLabel;
    public Text TargetLabel;
    public Text PriorityLabel;
    public Text MPLabel;
    public Text CDLabel;
    public Text AddPowerLabel;
    public Text NeedPowerLabel;
    public Text CommentLabel;
    public Text ItemAmountLabel;
    public Text BattleStatusTitlle;
    public Text NeedBookLabel;
    public Image NeedBookIcon;
    public Button UpgradeButton;
    public TipLabel TipLabel;
    public LoopScrollView SkillScrollView;
    public Text[] BattleStatusComment;

    private bool _isSpellCard;
    private int _currentItemAmount;
    private int _consumeBookId;
    private TeamMember _member;
    private SkillData.RootObject _data;
    private ItemManager.Type _itemManagerType;

    public void Init(TeamMember member, bool isSpellCard)
    {
        Clear();

        _member = member;
        _isSpellCard = isSpellCard;

        if (isSpellCard)
        {
            SkillScrollView.SetData(new ArrayList(member.SpellCardDic));
        }
        else
        {
            SkillScrollView.SetData(new ArrayList(member.SkillDic));
        }

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
        NameLabel.text = data.GetName();
        NameLabel.transform.parent.gameObject.SetActive(true);
        LvLabel.text = lv.ToString();
        LvLabel.transform.parent.gameObject.SetActive(true);
        MPLabel.text = data.MP.ToString();
        MPLabel.transform.parent.gameObject.SetActive(true);
        HitRateLabel.text = "命中："+data.HitRate+"%";
        CDLabel.text = "冷卻：" + data.CD;
        AddPowerLabel.text = "增加 Power：" + data.AddPower;
        NeedPowerLabel.text = "需要 Power：" + data.NeedPower;
        CommentLabel.text = data.GetComment();

        if (data.ValueList.Count > 0)
        {
            if (data.Type == SkillData.TypeEnum.Attack)
            {
                DamageLabel.text = "傷害：" + data.ValueList[lv - 1];
            }
            else
            {
                DamageLabel.text = "回復：" + data.ValueList[lv - 1];
            }
        }
        else
        {
            DamageLabel.text = "傷害：" + 0;
        }
        if (data.Target == SkillData.TargetType.Us)
        {
            TargetLabel.text = "目標：我方";
        }
        else if (data.Target == SkillData.TargetType.Them)
        {
            TargetLabel.text = "目標：敵方";
        }
        else if (data.Target == SkillData.TargetType.All)
        {
            TargetLabel.text = "目標：全體";
        }

        if (lv < data.MaxLv)
        {
            ItemAmountLabel.gameObject.SetActive(true);
            UpgradeButton.gameObject.SetActive(true);

            if (lv == 1)
            {
                _currentItemAmount = ItemManager.Instance.GetItemAmount(_bookId_1, _itemManagerType);
                NeedBookLabel.text = "初級技能書";
                NeedBookIcon.overrideSprite = Resources.Load<Sprite>("Image/Item/Book_1");
                _consumeBookId = _bookId_1;
            }
            else if (lv == 2)
            {
                _currentItemAmount = ItemManager.Instance.GetItemAmount(_bookId_2, _itemManagerType);
                NeedBookLabel.text = "中級技能書";
                NeedBookIcon.overrideSprite = Resources.Load<Sprite>("Image/Item/Book_2");
                _consumeBookId = _bookId_2;
            }
            else if (lv == 3)
            {
                _currentItemAmount = ItemManager.Instance.GetItemAmount(_bookId_3, _itemManagerType);
                NeedBookLabel.text = "高級技能書";
                NeedBookIcon.overrideSprite = Resources.Load<Sprite>("Image/Item/Book_3");
                _consumeBookId = _bookId_3;
            }

            if ( _currentItemAmount < 1)
            {
                UpgradeButton.GetComponent<Image>().color = Color.gray;
                ItemAmountLabel.text = "<color=#FF0000>" + _currentItemAmount + "</color>/" + 1;
            }
            else
            {
                UpgradeButton.GetComponent<Image>().color = Color.white;
                ItemAmountLabel.text = _currentItemAmount + "/" + 1;
            }
        }
        else
        {
            ItemAmountLabel.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);
        }

        BattleStatusTitlle.gameObject.SetActive(true);

        for (int i = 0; i < BattleStatusComment.Length; i++)
        {
            BattleStatusComment[i].gameObject.SetActive(false);
        }

        for (int i=0; i<BattleStatusComment.Length; i++)
        {
            if (data != null)
            {
                if (data.StatusID != 0)
                {
                    BattleStatusData.RootObject statusData = BattleStatusData.GetData(data.StatusID);
                    BattleStatusComment[i].gameObject.SetActive(true);
                    BattleStatusComment[i].text = statusData.GetComment(lv);
                }
                else
                {
                    i--;
                }

                if (data.SubID != 0)
                {
                    data = SkillData.GetData(data.SubID);
                }
                else
                {
                    data = null;
                }
            }
            else
            {
                break;
            }
        }
    }

    private void Clear()
    {
        NameLabel.text = string.Empty;
        NameLabel.transform.parent.gameObject.SetActive(false);
        LvLabel.text = string.Empty;
        LvLabel.transform.parent.gameObject.SetActive(false);
        MPLabel.text = string.Empty;
        MPLabel.transform.parent.gameObject.SetActive(false);
        DamageLabel.text = string.Empty;
        TargetLabel.text = string.Empty;
        PriorityLabel.text = string.Empty;
        CDLabel.text = string.Empty;
        AddPowerLabel.text = string.Empty;
        NeedPowerLabel.text = string.Empty;
        CommentLabel.text = string.Empty;
        UpgradeButton.gameObject.SetActive(false);
        ItemAmountLabel.gameObject.SetActive(false);

        BattleStatusTitlle.gameObject.SetActive(false);
        for (int i=0; i<BattleStatusComment.Length; i++)
        {
            BattleStatusComment[i].gameObject.SetActive(false);
        }
    }

    private void SkillOnClick(object obj)
    {
        KeyValuePair<SkillData.RootObject, int> pair = (KeyValuePair<SkillData.RootObject, int>)obj;
        SetData(pair.Key, pair.Value);
    }

    private void UpgradeOnClick()
    {
        if (_currentItemAmount < 1)
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
            ItemManager.Instance.MinusItem(_consumeBookId, 1, _itemManagerType);
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
        SkillScrollView.AddClickHandler(SkillOnClick);
    }
}

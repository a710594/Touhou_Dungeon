using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public Button MoveActionButton;
    public Button SkillActionButton;
    public Button SpellCardActionButton;
    public Button ItemActionButton;
    public Button IdleButton;
    public Button MoveConfirmButton;
    public Button ReturnActionButton; //返回選擇行動
    public ButtonPlus Screen; //偵測整個畫面的點擊事件
    public Text PowerLabel;
    public AnchorValueBar LittleHPBar;
    public FloatingNumberPool FloatingNumberPool;
    public BattleInfoUI InfoUI;
    public BattleSkillUI SkillInfoUI;
    public TipLabel TipLabel;
    public TipLabel TurnLabel;
    public LoopScrollView SkillScrollView;
    public GameObject ActionGroup;
    public BattleResultUI ResultUI;

    private Skill _tempSkill = null;
    private Dictionary<BattleCharacter, AnchorValueBar> _littleHPBarDic = new Dictionary<BattleCharacter, AnchorValueBar>();
    private Dictionary<BattleCharacter, FloatingNumberPool> _floatingNumberPoolDic = new Dictionary<BattleCharacter, FloatingNumberPool>();

    public static void Open()
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BattleUI", ResourceManager.Type.UI).GetComponent<BattleUI>();
        }
    }

    public static void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;
    }

    public void Init(List<BattleCharacter> characterList)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            InitCharacter(characterList[i]);
        }
    }

    public void InitCharacter(BattleCharacter character)
    {
        AnchorValueBar anchorValueBar;
        FloatingNumberPool floatingNumberPool;

        anchorValueBar = ResourceManager.Instance.Spawn(LittleHPBar.gameObject).GetComponent<AnchorValueBar>();
        anchorValueBar.SetValue(character.CurrentHP, character.MaxHP);
        anchorValueBar.SetAnchor(character.ValueBarAnchor);
        _littleHPBarDic.Add(character, anchorValueBar);

        floatingNumberPool = ResourceManager.Instance.Spawn(FloatingNumberPool.gameObject).GetComponent<FloatingNumberPool>();
        _floatingNumberPoolDic.Add(character, floatingNumberPool);
    }

    public void SetActionGroupVisible(bool isVisible)
    {
        ActionGroup.SetActive(isVisible);
    }

    public void SetMoveConfirmVisible(bool isVisible)
    {
        MoveConfirmButton.gameObject.SetActive(isVisible);
    }

    public void SetReturnActionVisible(bool isVisible)
    {
        ReturnActionButton.gameObject.SetActive(isVisible);
    }

    public void SetInfo(bool isVisible, BattleCharacter character = null)
    {
        InfoUI.gameObject.SetActive(isVisible);
        if (character != null)
        {
            InfoUI.SetData(character);
        }
    }

    public void SetStatus(BattleCharacter character, string comment, FloatingNumber.Type type, Action callback)
    {
        _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);

        _floatingNumberPoolDic[character].Play(comment, type, () =>
        {
        }, () =>
        {
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void SetSkillScrollViewVisible(bool isVisible)
    {
        SkillScrollView.gameObject.SetActive(isVisible);

        if (!isVisible)
        {
            SkillInfoUI.gameObject.SetActive(false);
        }
    }

    public void SetSkill(BattleCharacter character)
    {
        SkillScrollView.SetData(new ArrayList(character.SkillList));
        SkillScrollView.AddClickHandler(SkillOnClick);
    }

    public void SetSpellCard(BattleCharacter character)
    {
        SkillScrollView.SetData(new ArrayList(character.SpellCardList));
        SkillScrollView.AddClickHandler(SkillOnClick);
    }

    public void SetItem()
    {
        int itemId;
        ItemData.RootObject itemData;
        SkillData.RootObject skillData;
        ItemSkill skill;
        List<ItemSkill> itemSkillList = new List<ItemSkill>();
        Dictionary<object, int> itemDic = ItemManager.Instance.GetItemDicByType(ItemManager.Type.Bag, ItemData.TypeEnum.Medicine);

        if (itemDic != null)
        {
            foreach (KeyValuePair<object, int> item in itemDic)
            {
                itemId = (int)item.Key;
                itemData = ItemData.GetData(itemId);
                skillData = SkillData.GetData(itemData.Skill);
                skill = (ItemSkill)SkillFactory.GetNewSkill(skillData);
                skill.ItemID = itemId;
                itemSkillList.Add(skill);
            }
            SkillScrollView.SetData(new ArrayList(itemSkillList));
            SkillScrollView.AddClickHandler(SkillOnClick);
        }
        else
        {
            SkillScrollView.SetData(new ArrayList());
        }
    }

    public void SetFloatingNumber(BattleCharacter character, string text, FloatingNumber.Type type, Action callback)
    {
        _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);
        _floatingNumberPoolDic[character].Play(text, type, () =>
        {
            SetLittleHPBar(character, true);
        }, () =>
        {
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void SetLittleHPBar(BattleCharacter character, bool isVisible)
    {
        _littleHPBarDic[character].gameObject.SetActive(isVisible);
        _littleHPBarDic[character].SetValueTween(character.CurrentHP, character.MaxHP, null);
    }

    public void SetTurnLabel(int turn)
    {
        TurnLabel.SetLabel("Turn" + turn.ToString());
    }

    public void SetResult(bool isWin, List<int> orignalLvList = null, List<int> orignalExpList = null, List<int> itemList = null)
    {
        ResultUI.Open(isWin, orignalLvList, orignalExpList, itemList);
    }

    public void SetPower(int power) 
    {
        PowerLabel.text = "Power:" + power.ToString();
    }

    private void MoveActionOnClick()
    {
        BattleController.Instance.ChangeToMoveState();
        SetActionGroupVisible(false);
    }

    private void SkillActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        SetSkill(BattleController.Instance.SelectedCharacter);
    }

    private void SpellCardActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        SetSpellCard(BattleController.Instance.SelectedCharacter);
    }

    private void ItemActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        SetItem();
    }

    private void IdleOnClick()
    {
        BattleController.Instance.SetIdle();
        SetActionGroupVisible(false);
    }

    private void MoveConfirmOnClick()
    {
        BattleController.Instance.MoveConfirm();
    }

    private void ReturnActionOnClick()
    {
        BattleController.Instance.MoveCancel();
    }

    private void ScreenOnClick(object data)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int intPosition = Vector2Int.RoundToInt(worldPosition);
        intPosition.y -= 1; //地圖座標的偏移值
        BattleController.Instance.ScreenOnClick(intPosition);
    }

    private void StartDragCamera(object data)
    {
        CameraController.Instance.StartDrag(Input.mousePosition);
    }

    private void OnDragCamera(object data)
    {
        CameraController.Instance.OnDrag(Input.mousePosition);
    }

    private void SkillOnClick(object data)
    {
        SkillScrollItem scrollItem = (SkillScrollItem)data;
        if (scrollItem.CanUse)
        {
            Skill skill = scrollItem.Skill;

            skill.GetSkillDistance(BattleController.Instance.SelectedCharacter, BattleController.Instance.CharacterList);
            _tempSkill = skill;
            TipLabel.SetVisible(false);
            BattleController.Instance.SelectSkill(_tempSkill);
            //SkillConfirmButton.gameObject.SetActive(true);
        }
        else
        {
            TipLabel.SetLabel(scrollItem.NotUseReason);
        }

        SkillInfoUI.gameObject.SetActive(true);
        SkillInfoUI.SetData(scrollItem.Skill.Data);
    }

    private void SkillConfirmOnClick()
    {
        BattleController.Instance.SelectSkill(_tempSkill);
        _tempSkill = null;
    }

    private void Awake()
    {
        SetActionGroupVisible(false);
        SetMoveConfirmVisible(false);
        SetReturnActionVisible(false);
        SkillScrollView.gameObject.SetActive(false);
        ResultUI.gameObject.SetActive(false);

        MoveActionButton.onClick.AddListener(MoveActionOnClick);
        SkillActionButton.onClick.AddListener(SkillActionOnClick);
        SpellCardActionButton.onClick.AddListener(SpellCardActionOnClick);
        ItemActionButton.onClick.AddListener(ItemActionOnClick);
        IdleButton.onClick.AddListener(IdleOnClick);
        MoveConfirmButton.onClick.AddListener(MoveConfirmOnClick);
        ReturnActionButton.onClick.AddListener(ReturnActionOnClick);
        Screen.ClickHandler = ScreenOnClick;
        Screen.DownHandler = StartDragCamera;
        Screen.PressHandler = OnDragCamera;
    }
}

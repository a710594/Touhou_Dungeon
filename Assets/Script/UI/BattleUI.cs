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
    public Text ActionCountLabel;
    public Text SkillLabel;
    public AnchorValueBar LittleHPBar;
    public FloatingNumberPool FloatingNumberPool;
    public BattleInfoUI InfoUI;
    public BattleSkillUI SkillInfoUI;
    public BattleFieldUI BattleFieldUI;
    public TipLabel TipLabel;
    public TipLabel TurnLabel;
    public LoopScrollView SkillScrollView;
    public GameObject PowerPoint;
    public GameObject ActionGroup;
    public BattleResultUI ResultUI;

    private Skill _tempSkill = null;
    private Timer _timer = new Timer();
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

    public void Init(int power, List<BattleCharacter> characterList)
    {
        PowerLabel.text = "Power:" + power.ToString();
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
        anchorValueBar.SetValue(character.Info.CurrentHP, character.Info.MaxHP);
        anchorValueBar.SetAnchor(character.ValueBarAnchor);
        _littleHPBarDic.Add(character, anchorValueBar);

        floatingNumberPool = ResourceManager.Instance.Spawn(FloatingNumberPool.gameObject).GetComponent<FloatingNumberPool>();
        _floatingNumberPoolDic.Add(character, floatingNumberPool);
    }

    public void SetActionGroupVisible(bool isVisible)
    {
        ActionGroup.SetActive(isVisible);

        if (isVisible)
        {
            //ActionCountLabel.text = LanguageData.GetText(10, LanguageSystem.Instance.CurrentLanguage) +  ":" + BattleController.Instance.SelectedCharacter.Info.ActionCount; //行動次數
            SkillActionButton.interactable = !BattleController.Instance.SelectedCharacter.Info.HasUseSkill;
            SpellCardActionButton.interactable = !BattleController.Instance.SelectedCharacter.Info.HasUseSkill;
            ItemActionButton.interactable = !BattleController.Instance.SelectedCharacter.Info.HasUseSkill;
        }
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

    public void SetSkillScrollViewData(List<Skill> list)
    {
        SkillScrollView.SetData(new ArrayList(list));
        SkillScrollView.AddClickHandler(SkillOnClick);
    }

    public void RemoveSelectedSkill() 
    {
        SkillScrollView.RemoveSelectedItem();
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
        _littleHPBarDic[character].SetValueTween(character.Info.CurrentHP, character.Info.MaxHP,  null);
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

    public void SetSkillLabel(bool isVisible, string text = "")
    {
        SkillLabel.gameObject.SetActive(isVisible);
        SkillLabel.text = text;
    }

    public void ShowSpellCard(string name, string image, Action callback)
    {
        gameObject.SetActive(false);
        BattleFrontUI.Instance.ShowSpellCard(name, image, ()=> 
        {
            callback();
            gameObject.SetActive(true);
        });
    }

    public void SetBattleFieldVisible(bool isVisible) 
    {
        BattleFieldUI.SetVisible(isVisible);
    }

    public void SetBattleFieldData(BattleField battleField) 
    {
        BattleFieldUI.SetData(battleField);
    }

    public void DropPowerPoint(List<BattleCharacter> targetList) 
    {
        GameObject obj;
        for (int i=0; i<targetList.Count; i++) 
        {
            obj = Instantiate(PowerPoint, Vector3.zero, Quaternion.identity);
            obj.transform.parent = PowerPoint.transform.parent;
            obj.transform.position = Camera.main.WorldToScreenPoint(targetList[i].transform.position + Vector3.up * 0.5f);
            JumpPowerPoint(obj);
        }
    }

    private void JumpPowerPoint(GameObject obj) 
    {
        obj.transform.DOLocalJump(obj.transform.localPosition + Vector3.right * UnityEngine.Random.Range(-50, 50), 50, 1, 0.5f).OnComplete(() =>
        {
            obj.transform.DOMove(PowerLabel.transform.position, 0.5f).OnComplete(() =>
            {
                Destroy(obj);
            });
        });
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
        SetSkillScrollViewData(BattleController.Instance.SelectedCharacter.SkillList);
    }

    private void SpellCardActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        SetSkillScrollViewData(BattleController.Instance.SelectedCharacter.SpellCardList);
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

    private void ScreenOnClick(ButtonPlus button)
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

    private void EndDragCamera(object data)
    {
        CameraController.Instance.EndDrag();
    }

    private void SkillOnClick(object data)
    {
        SkillScrollItem scrollItem = (SkillScrollItem)data;
        if (scrollItem.CanUse)
        {
            Skill skill = scrollItem.Skill;

            skill.GetDistance(BattleController.Instance.SelectedCharacter, BattleController.Instance.CharacterList);
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
        Screen.UpHandler = EndDragCamera;
    }
}

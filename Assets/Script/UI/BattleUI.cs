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
    public Button UndoButton;
    public Button IdleButton;
    public Button MoveConfirmButton;
    public Button ReturnActionButton; //返回選擇行動
    public Button EscapeButton;
    public Button GiveUpButton;
    public ButtonPlus Screen; //偵測整個畫面的點擊事件
    public Text PowerLabel;
    public Text ActionCountLabel;
    public Text SkillLabel;
    public Text TurnLabel;
    public AnchorValueBar LittleHPBar;
    public FloatingNumberPool FloatingNumberPool;
    public BattleInfoUI InfoUI;
    public BattleSkillUI SkillInfoUI;
    public BattleFieldUI BattleFieldUI;
    public TipLabel TipLabel;
    public LoopScrollView SkillScrollView;
    public GameObject PowerPoint;
    public GameObject ActionGroup;
    public BattleResultUI ResultUI;
    public PriorityQueue PriorityQueue;

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

    public void Init(int power, List<BattleCharacter> characterList, bool canEscape)
    {
        PowerLabel.text = "Power:" + power.ToString();
        EscapeButton.gameObject.SetActive(canEscape);
        GiveUpButton.gameObject.SetActive(canEscape);
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
        anchorValueBar.SetHPQueue(character.Info.HPQueue.Count);
        anchorValueBar.SetAnchor(character.ValueBarAnchor);
        _littleHPBarDic.Add(character, anchorValueBar);
        if (character.LiveState == BattleCharacter.LiveStateEnum.Dead)
        {
            anchorValueBar.gameObject.SetActive(false);
        }

        floatingNumberPool = ResourceManager.Instance.Spawn(FloatingNumberPool.gameObject).GetComponent<FloatingNumberPool>();
        _floatingNumberPoolDic.Add(character, floatingNumberPool);
    }

    public void SetActionGroupVisible(bool isVisible)
    {
        ActionGroup.SetActive(isVisible);

        if (isVisible)
        {
            BattleCharacter character = BattleController.Instance.SelectedCharacter;
            ActionCountLabel.text = LanguageData.GetText(10, LanguageSystem.Instance.CurrentLanguage) +  ":" + character.Info.ActionCount; //行動次數
            SkillActionButton.interactable = !character.Info.HasUseSkill;
            SpellCardActionButton.interactable = !character.Info.HasUseSkill;
            ItemActionButton.interactable = !character.Info.HasUseSkill;
            UndoButton.interactable = character.CanUndoMove();
            SetEscapeButton();
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
            //ActionCountLabel.text = "行動次數：" + character.Info.ActionCount.ToString();
        }
        else
        {
            //ActionCountLabel.text = string.Empty;
        }
    }

    public void SetSkillScrollViewVisible(bool isVisible)
    {
        SkillScrollView.gameObject.SetActive(isVisible);

        if (!isVisible)
        {
            SkillInfoUI.gameObject.SetActive(false);
        }
    }

    private void SetSkillScrollViewData(List<Skill> list)
    {
        SkillScrollView.SetData(new ArrayList(list));
    }

    public void RemoveSelectedSkill() 
    {
        SkillScrollView.RemoveSelectedItem();
    }

    public void SetFloatingNumber(BattleCharacter character, string text, FloatingNumber.Type type)
    {
        _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);
        _floatingNumberPoolDic[character].Play(text, type);
    }

    public void SetFloatingNumber(BattleCharacter character, List<FloatingNumberData> list)
    {
        _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);
        _floatingNumberPoolDic[character].Play(list);
    }

    public void SetLittleHPBar(BattleCharacter character, bool isVisible)
    {
        _littleHPBarDic[character].gameObject.SetActive(isVisible);

        int lastHPQueueAmount = _littleHPBarDic[character].GetHPQueueAmount();
        if (lastHPQueueAmount == character.Info.HPQueue.Count)
        {
            _littleHPBarDic[character].SetValueTween(character.Info.CurrentHP, character.Info.MaxHP, null);
        }
        else
        {
            _littleHPBarDic[character].SetValueTween(0, character.Info.MaxHP, ()=> 
            {
                _littleHPBarDic[character].SetValueTween(character.Info.CurrentHP, character.Info.MaxHP, null);
                _littleHPBarDic[character].SetHPQueue(character.Info.HPQueue.Count);
            });
        }
    }

    public void SetPredictionHP(BattleCharacter character, int damage)
    {
        int prediction = character.Info.CurrentHP - damage;
        if (prediction < 0)
        {
            prediction = 0;
        }
        _littleHPBarDic[character].SetPrediction(character.Info.CurrentHP, prediction, character.Info.MaxHP);
    }

    public void StopPredictionHP(BattleCharacter character)
    {
        _littleHPBarDic[character].StopPrediction();
    }

    public void SetTurnLabel(int turn, Action callback)
    {
        SetInfo(false);
        PriorityQueue.transform.parent.gameObject.SetActive(false);
        PowerLabel.gameObject.SetActive(false);
        ActionCountLabel.gameObject.SetActive(false);
        TurnLabel.transform.parent.parent.gameObject.SetActive(true);
        TurnLabel.text = "Turn" + turn.ToString();
        TurnLabel.transform.parent.localPosition = Vector3.right * 1280;
        TurnLabel.transform.parent.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            _timer.Start(1f, () =>
            {
                TurnLabel.transform.parent.DOLocalMoveX(-1280, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    TurnLabel.transform.parent.parent.gameObject.SetActive(false);
                    PowerLabel.gameObject.SetActive(true);
                    ActionCountLabel.gameObject.SetActive(true);
                    if (callback != null)
                    {
                        callback();
                    }
                });
            });
        });
    }

    public void SetResult(bool isWin, Action callback, int orignalLv = 0, int orignalExp = 0, List<int> itemList = null)
    {
        ResultUI.Open(isWin, orignalLv, orignalExp, itemList, callback);
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
            obj.transform.SetParent(PowerPoint.transform.parent);
            obj.transform.position = Camera.main.WorldToScreenPoint(targetList[i].transform.position + Vector3.up * 0.5f);
            JumpPowerPoint(obj);
        }
    }

    public void InitPriorityQueue(List<BattleCharacter> characterList)
    {
        PriorityQueue.transform.parent.gameObject.SetActive(true);
        PriorityQueue.Init(characterList);
    }

    public void ScrollPriorityQueue(BattleCharacter character)
    {
        PriorityQueue.Scroll(character);
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
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

    private void SetEscapeButton()
    {
        if (BattleController.Instance.SelectedCharacter.Info.ActionCount == 2)
        {
            EscapeButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            EscapeButton.GetComponent<Image>().color = Color.gray;
        }
    }

    public void MoveActionOnClick()
    {
        BattleController.Instance.ChangeToMoveState();
        SetActionGroupVisible(false);
    }

    public void SkillActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        SetSkillScrollViewData(BattleController.Instance.SelectedCharacter.Info.SkillList);
    }

    private void SpellCardActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        SetSkillScrollViewData(BattleController.Instance.SelectedCharacter.Info.SpellCardList);
    }

    private void ItemActionOnClick()
    {
        BattleController.Instance.ChangeToSelectSkillState();
        SetActionGroupVisible(false);
        //SetItem();
        List<Skill> itemSkillList = BattleController.Instance.ItemSkillList;
        for (int i=0; i<itemSkillList.Count; i++)
        {
            itemSkillList[i].SetUser(BattleController.Instance.SelectedCharacter.Info);
        }
        SetSkillScrollViewData(itemSkillList);
    }

    private void UndoOnClick()
    {
        BattleController.Instance.MoveUndo();
        SetEscapeButton();
    }

    public void IdleOnClick()
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
        Skill skill = scrollItem.Skill;
        if (scrollItem.CanUse)
        {
            List<Vector2Int> positionList = skill.GetDistance(BattleController.Instance.SelectedCharacter);
            _tempSkill = skill;
            TipLabel.SetVisible(false);
            BattleController.Instance.SelectSkill(_tempSkill);

            for (int i=0; i<positionList.Count; i++)
            {
                TilePainter.Instance.Painting("RedGrid", 2, positionList[i]);
            }
        }
        else
        {
            TipLabel.SetLabel(scrollItem.NotUseReason);
        }

        SkillInfoUI.gameObject.SetActive(true);
        SkillInfoUI.SetData(skill.Data, skill.Lv);
    }

    private void EscapeOnClick()
    {
        if (BattleController.Instance.SelectedCharacter.Info.ActionCount == 2)
        {
            BattleController.Instance.ChangeToEscapeState();
        }
        else
        {
            TipLabel.SetLabel("需要有兩個行動次數才能逃跑");
        }
    }

    private void GiveUpOnClick()
    {
        ConfirmUI.Open("確定要放棄嗎？\n放棄後會直接回到村莊。", "確定", "取消", ()=> 
        {
            BattleController.Instance.GiveUp();
        }, null);
    }

    private void Update()
    {
        if (ProgressManager.Instance.Memo.FirstFlag) //已完成新手教學
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                CameraController.Instance.CameraMove(Vector2Int.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                CameraController.Instance.CameraMove(Vector2Int.right);
            }
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                CameraController.Instance.CameraMove(Vector2Int.up);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                CameraController.Instance.CameraMove(Vector2Int.down);
            }
        }
    }

    private void Awake()
    {
        SetActionGroupVisible(false);
        SetMoveConfirmVisible(false);
        SetReturnActionVisible(false);
        SkillScrollView.gameObject.SetActive(false);
        ResultUI.gameObject.SetActive(false);
        BattleFieldUI.gameObject.SetActive(false);

        MoveActionButton.onClick.AddListener(MoveActionOnClick);
        SkillActionButton.onClick.AddListener(SkillActionOnClick);
        SpellCardActionButton.onClick.AddListener(SpellCardActionOnClick);
        ItemActionButton.onClick.AddListener(ItemActionOnClick);
        UndoButton.onClick.AddListener(UndoOnClick);
        IdleButton.onClick.AddListener(IdleOnClick);
        MoveConfirmButton.onClick.AddListener(MoveConfirmOnClick);
        ReturnActionButton.onClick.AddListener(ReturnActionOnClick);
        EscapeButton.onClick.AddListener(EscapeOnClick);
        GiveUpButton.onClick.AddListener(GiveUpOnClick);
        Screen.ClickHandler = ScreenOnClick;
        Screen.DownHandler = StartDragCamera;
        Screen.PressHandler = OnDragCamera;
        Screen.UpHandler = EndDragCamera;
        SkillScrollView.AddClickHandler(SkillOnClick);
    }
}

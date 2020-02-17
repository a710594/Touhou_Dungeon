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
    public Button ItemActionButton;
    public Button MoveConfirmButton;
    public Button ReturnActionButton; //返回選擇行動
    public Button SkillConfirmButton;
    public ButtonPlus Screen; //偵測整個畫面的點擊事件
    public AnchorValueBar LittleHPBar;
    public FloatingNumberPool FloatingNumberPool;
    public BattleInfoUI InfoUI;
    public BattleSkillGroup SkillGroup;
    public TipLabel TipLabel;
    public LoopScrollView SkillScrollView;
    public GameObject ActionGroup;

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

    public void SetSkillData(BattleCharacter character)
    {
        //SkillGroup.gameObject.SetActive(true);
        SkillScrollView.gameObject.SetActive(true);
        SkillScrollView.SetData(new ArrayList(character.SkillList));
        SkillScrollView.AddClickHandler(SkillOnClick);
        SkillConfirmButton.gameObject.SetActive(false);
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

    //public void SetDamage(BattleCharacter character, int damage, FloatingNumber.Type type, Action callback)
    //{
    //    _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);
    //    _floatingNumberPoolDic[character].Play(damage.ToString(), type, () =>
    //    {
    //        SetLittleHPBar(character, true);
    //    }, () =>
    //    {
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    });
    //}

    //public void SetMiss(BattleCharacter character, Action callback)
    //{
    //    _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);
    //    _floatingNumberPoolDic[character].Play("Miss", FloatingNumber.Type.Miss, () =>
    //    {
    //    }, () =>
    //    {
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    });
    //}

    //public void SetRecover(BattleCharacter character, int recover, Action callback)
    //{
    //    _floatingNumberPoolDic[character].transform.position = Camera.main.WorldToScreenPoint(character.Sprite.transform.position);

    //    _floatingNumberPoolDic[character].Play(recover.ToString(), FloatingNumber.Type.Recover, () =>
    //    {
    //        SetLittleHPBar(character, true);
    //    }, () =>
    //    {
    //        if (callback != null)
    //        {
    //            callback();
    //        }
    //    });
    //}

    public void SetLittleHPBar(BattleCharacter character, bool isVisible)
    {
        _littleHPBarDic[character].gameObject.SetActive(isVisible);
        _littleHPBarDic[character].SetValueTween(character.CurrentHP, character.MaxHP, null);
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
    }

    private void ItemActionOnClick()
    {
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

    private void SkillOnClick(object data)
    {
        SkillScrollItem scrollItem = (SkillScrollItem)data;
        if (scrollItem.CanUse)
        {
            Skill skill = scrollItem.Skill;

            //SkillGroup.SetData(skill.Data);
            _tempSkill = skill;
            TipLabel.SetVisible(false);
            SkillConfirmButton.gameObject.SetActive(true);
            SkillConfirmButton.transform.SetParent(scrollItem.transform);
            SkillConfirmButton.transform.localPosition = new Vector3(-200, 0, 0);
        }
        else
        {
            TipLabel.SetLabel(scrollItem.NotUseReason);
        }
    }

    private void SkillConfirmOnClick()
    {
        BattleController.Instance.SelectSkill(_tempSkill);
        _tempSkill = null;
        SkillScrollView.gameObject.SetActive(false);
        SkillConfirmButton.gameObject.SetActive(false);
    }

    private void Awake()
    {
        SetActionGroupVisible(false);
        SetMoveConfirmVisible(false);
        SetReturnActionVisible(false);
        SkillScrollView.gameObject.SetActive(false);

        MoveActionButton.onClick.AddListener(MoveActionOnClick);
        SkillActionButton.onClick.AddListener(SkillActionOnClick);
        ItemActionButton.onClick.AddListener(ItemActionOnClick);
        MoveConfirmButton.onClick.AddListener(MoveConfirmOnClick);
        ReturnActionButton.onClick.AddListener(ReturnActionOnClick);
        SkillConfirmButton.onClick.AddListener(SkillConfirmOnClick);
        Screen.ClickHandler = ScreenOnClick;
    }
}

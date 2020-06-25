using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleResultUI : MonoBehaviour
{
    public GameObject WinLabel;
    public GameObject LoseLabel;
    public Button NextButton;
    public Button ConfirmButton;
    public LoopScrollView ScrollView;
    public GameObject CharacterGroup;
    public GameObject ItemGroup;
    public CharacterLvCard[] CharacterLvCards;

    private Action _winCallback;
    private Action _loseCallback;

    private bool _isWin;
    private Timer _timer = new Timer();

    public void Open(bool isWin, List<int> orignalLvList, List<int> orignalExpList, List<int> itemList, Action winCallback, Action loseCallback)
    {
        _isWin = isWin;
        _winCallback = winCallback;
        _loseCallback = loseCallback;
        gameObject.SetActive(true);
        WinLabel.SetActive(isWin);
        LoseLabel.SetActive(!isWin);

        if (isWin)
        {
            ItemGroup.SetActive(false);
            ConfirmButton.gameObject.SetActive(false);
            CharacterGroup.SetActive(false);
            NextButton.gameObject.SetActive(false);
            _timer.Start(0.5f, ()=> 
            {
                CharacterGroup.SetActive(true);
                NextButton.gameObject.SetActive(true);
                SetCharacterGroup(orignalLvList, orignalExpList);
                SetItemGroup(itemList);
            });
        }
        else
        {
            CharacterGroup.SetActive(false);
            ItemGroup.SetActive(false);
            NextButton.gameObject.SetActive(false);
            ConfirmButton.gameObject.SetActive(true);
        }
    }

    private void SetCharacterGroup(List<int> originalLvList, List<int> originalExpList)
    {
        List<TeamMember> memberList = TeamManager.Instance.MemberList;
        for (int i = 0; i < CharacterLvCards.Length; i++)
        {
            if (i < memberList.Count)
            {
                CharacterLvCards[i].gameObject.SetActive(true);
                CharacterLvCards[i].SetData(originalLvList[i], originalExpList[i], memberList[i]);
            }
            else
            {
                CharacterLvCards[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetItemGroup(List<int> itemList)
    {
        List<KeyValuePair<object, int>> itemPair = new List<KeyValuePair<object, int>>();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemPair.Add(new KeyValuePair<object, int>(itemList[i], 0)); //數量設為0是為了不讓 scrollview 顯示數量.實際數量都是1,沒有顯示的必要
        }
        ScrollView.SetData(new ArrayList(itemPair));
    }

    private void NextOnClick()
    {
        CharacterGroup.SetActive(false);
        ItemGroup.SetActive(true);
        NextButton.gameObject.SetActive(false);
        ConfirmButton.gameObject.SetActive(true);
    }

    private void ConfirmOnClick()
    {
        if (ItemManager.Instance.BagIsFull())
        {
            ConfirmUI.Open("背包已滿！請選擇要丟棄的道具。", "確定", () =>
            {
                BagUI.Open(ItemManager.Type.Bag);
            });
        }
        else
        {
            if (_isWin)
            {
                if (_winCallback != null)
                {
                    _winCallback();
                }
                else
                {
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
                    {
                        ExploreController.Instance.SetFloorFromMemo();
                    });
                }
            }
            else
            {
                if (_loseCallback != null)
                {
                    _loseCallback();
                }
                else
                {
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage, () =>
                    {
                        ItemManager.Instance.PutBagItemIntoWarehouse();
                        TeamManager.Instance.RecoverAllMember();
                    });
                }
            }
        }
    }

    void Awake()
    {
        NextButton.onClick.AddListener(NextOnClick);
        ConfirmButton.onClick.AddListener(ConfirmOnClick);
        //CloseTipButton.onClick.AddListener(CloseTipOnClick);
        //TipGroup.SetActive(false);
    }
}

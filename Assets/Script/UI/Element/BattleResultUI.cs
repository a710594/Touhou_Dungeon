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
    public Button ConfirmButton;
    public LoopScrollView ScrollView;
    public GameObject CharacterGroup;
    public GameObject ItemGroup;
    public Text LvLabel;
    public ValueBar ExpBar;

    private Action _callback;

    private bool _isWin;
    private Timer _timer = new Timer();

    public void Open(bool isWin, int orignalLv, int orignalExp, List<int> itemList, Action callback)
    {
        _isWin = isWin;
        _callback = callback;
        gameObject.SetActive(true);
        WinLabel.SetActive(isWin);
        LoseLabel.SetActive(!isWin);

        if (isWin)
        {
            SetCharacterGroup(orignalLv, orignalExp);
            SetItemGroup(itemList);
        }
        else
        {
            CharacterGroup.SetActive(false);
            ItemGroup.SetActive(false);
            ConfirmButton.gameObject.SetActive(true);
        }
    }

    private void SetCharacterGroup(int originalLv, int originalExp)
    {
        int needExp = TeamManager.Instance.NeedExp(originalLv);
        int currentLv = TeamManager.Instance.Lv;
        int currentExp = TeamManager.Instance.Exp;

        LvLabel.text = "Lv." + originalLv.ToString();
        if (originalLv < currentLv)
        {
            ExpBar.SetValueTween(originalExp, needExp, needExp, () =>
            {
                LvLabel.text = "Lv." + (originalLv + 1).ToString();
                SetCharacterGroup(originalLv + 1, 0);
            });
        }
        else
        {
            LvLabel.text = "Lv." + currentLv.ToString();
            ExpBar.SetValueTween(originalExp, currentExp, needExp, null);
        }
    }

    private void SetItemGroup(List<int> itemList)
    {
        List<KeyValuePair<int, int>> itemPair = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemPair.Add(new KeyValuePair<int, int>(itemList[i], 1));
        }
        ScrollView.SetData(new ArrayList(itemPair));
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

            if (_callback != null)
            {
                _callback();
            }
        }
    }

    void Awake()
    {
        ConfirmButton.onClick.AddListener(ConfirmOnClick);
        //CloseTipButton.onClick.AddListener(CloseTipOnClick);
        //TipGroup.SetActive(false);
    }
}

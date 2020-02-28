using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleResultUI : MonoBehaviour
{
    public GameObject WinLabel;
    public GameObject LoseLabel;
    public GameObject TipGroup;
    public Button NextButton;
    public Button ConfirmButton;
    public Button CloseTipButton;
    public LoopScrollView ScrollView;
    public Text NameLabel;
    public Text CommentLabel;
    public GameObject CharacterGroup;
    public GameObject ItemGroup;
    //public CharacterLvCard[] CharacterLvCards;

    public void Open(bool isWin, List<int> orignalLvList = null, List<int> orignalExpList = null, List<int> itemList = null)
    {
        gameObject.SetActive(true);
        WinLabel.SetActive(isWin);
        LoseLabel.SetActive(!isWin);

        //if (isWin)
        //{
        //    SetCharacterGroup(orignalLvList, orignalExpList);
        //    CharacterGroup.SetActive(true);
        //    SetItemGroup(itemList);
        //    ItemGroup.SetActive(false);
        //    NextButton.gameObject.SetActive(true);
        //    ConfirmButton.gameObject.SetActive(false);

        //}
        //else
        //{
            //CharacterGroup.SetActive(false);
            //ItemGroup.SetActive(false);
            //NextButton.gameObject.SetActive(false);
            ConfirmButton.gameObject.SetActive(true);
        //}
    }

    private void SetCharacterGroup(List<int> originalLvList, List<int> originalExpList)
    {
        //List<TeamMember> memberList = TeamManager.Instance.MemberList;
        //for (int i = 0; i < CharacterLvCards.Length; i++)
        //{
        //    if (i < memberList.Count)
        //    {
        //        CharacterLvCards[i].gameObject.SetActive(true);
        //        CharacterLvCards[i].SetData(originalLvList[i], originalExpList[i], memberList[i]);
        //    }
        //    else
        //    {
        //        CharacterLvCards[i].gameObject.SetActive(false);
        //    }
        //}
    }

    private void SetItemGroup(List<int> itemList)
    {
        List<KeyValuePair<object, int>> itemPair = new List<KeyValuePair<object, int>>();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemPair.Add(new KeyValuePair<object, int>(itemList[i], 0)); //數量設為0是為了不讓 scrollview 顯示數量.實際數量都是1,沒有顯示的必要
        }
        ScrollView.SetData(new ArrayList(itemPair));
        ScrollView.AddClickHandler(IconOnClick);
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
        //if (ItemManager.Instance.BagIsFull())
        //{
        //    ConfirmUI.Open("背包已滿！請選擇要丟棄的道具。", "確定", () =>
        //    {
        //        BagUI.Open(ItemManager.Type.Bag);
        //    });
        //}
        //else
        //{
        //    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
        //    {
        //        ExploreController.Instance.Reload();
        //    });
        //}

        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle,()=> 
        {
            //wait for init
            Timer timer = new Timer();
            timer.Start(0.5f, () =>
            {
                List<KeyValuePair<int, int>> enemyList = BattleGroupData.GetEnemy(1);
                BattleController.Instance.Init(1, enemyList);
            });
        });
    }

    private void IconOnClick(object obj)
    {
        //if (obj is int)
        //{
        //    ItemData data = ItemData.GetData((int)obj);
        //    NameLabel.text = data.Name;
        //    CommentLabel.text = data.Comment;
        //}
        //else if (obj is Equip)
        //{
        //    Equip equip = (Equip)obj;
        //    NameLabel.text = equip.Name;
        //    CommentLabel.text = equip.Comment;
        //}
        //TipGroup.SetActive(true);
    }

    private void CloseTipOnClick()
    {
        TipGroup.SetActive(false);
    }

    void Awake()
    {
        //NextButton.onClick.AddListener(NextOnClick);
        ConfirmButton.onClick.AddListener(ConfirmOnClick);
        //CloseTipButton.onClick.AddListener(CloseTipOnClick);
        //TipGroup.SetActive(false);
    }
}

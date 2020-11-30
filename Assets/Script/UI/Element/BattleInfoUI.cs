using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleInfoUI : MonoBehaviour
{
    public Image Image;
    public Text NameLabel;
    public Text LvLabel;
    public ValueBar HpBar;
    public ValueBar MpBar;
    public Button FoodBuffButton;
    public Button BattleStatusButton;
    public FoodBuffGroup FoodBuffGroup;
    public BattleStatusUI BattleStatusUI;
    public Image FoodBuffIcon;
    public Image[] BattleStatusIcon;

    private FoodBuff _foodBuff;
    private List<BattleStatus> _statusList = new List<BattleStatus>();

    public void SetData(BattleCharacter character)
    {
        NameLabel.text = character.Info.Name;
        LvLabel.text = character.Info.Lv.ToString();
        HpBar.SetValue(character.Info.CurrentHP, character.Info.MaxHP);

        _foodBuff = character.Info.FoodBuff;
        _statusList = new List<BattleStatus>(character.Info.StatusDic.Values);

        if (_foodBuff != null)
        {
            FoodBuffIcon.gameObject.SetActive(true);
        }
        else
        {
            FoodBuffIcon.gameObject.SetActive(false);
        }

        for (int i=0; i<BattleStatusIcon.Length; i++)
        {
            if (i < _statusList.Count)
            {
                BattleStatusIcon[i].gameObject.SetActive(true);
                BattleStatusIcon[i].overrideSprite = Resources.Load<Sprite>("Image/BattleStatus/" + _statusList[i].Icon);
            }
            else
            {
                BattleStatusIcon[i].gameObject.SetActive(false);
            }
        }

        if (character.Info.IsTeamMember)
        {
            Image.gameObject.SetActive(true);
            if (character.Info.JobData.MediumImage != string.Empty)
            {
                Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Medium/" + character.Info.JobData.MediumImage);
            }
            else
            {
                Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Medium/NoImage_M");
            }
            MpBar.gameObject.SetActive(true);
            MpBar.SetValue(character.Info.CurrentMP, character.Info.MaxMP);
        }
        else
        {
            Image.gameObject.SetActive(false);
            MpBar.gameObject.SetActive(false);
        }
    }

    public void SetHpBar(int currentHP, int maxHp)
    {
        HpBar.SetValue(currentHP, maxHp);
    }

    public void SetMpBar(int currentMP, int maxMp)
    {
        MpBar.SetValue(currentMP, maxMp);
    }

    private void BattleStatusOnClick() 
    {
        if (_statusList.Count > 0)
        {
            BattleStatusUI.Open(_statusList);
        }
    }

    private void FoodBuffOnClick()
    {
        if (_foodBuff != null)
        {
            FoodBuffGroup.Open(_foodBuff);
        }
    }

    void Awake()
    {
        FoodBuffButton.onClick.AddListener(FoodBuffOnClick);
        BattleStatusButton.onClick.AddListener(BattleStatusOnClick);
        BattleStatusUI.gameObject.SetActive(false);
    }
}


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
    public Button BattleStatusButton;
    public BattleStatusUI BattleStatusUI;
    public Image[] BattleStatusIcon;

    private bool _isScrollViewShow = false;
    private bool _isClickable = true;
    List<BattleStatus> _statusList = new List<BattleStatus>();

    public void SetData(BattleCharacter character)
    {
        NameLabel.text = character.Info.Name;
        LvLabel.text = "Lv." + character.Info.Lv.ToString();
        HpBar.SetValue(character.Info.CurrentHP, character.Info.MaxHP);

        _statusList = new List<BattleStatus>(character.Info.StatusDic.Values);

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

        if (character is BattleCharacterPlayer)
        {
            BattleCharacterPlayer player = (BattleCharacterPlayer)character;

            Image.gameObject.SetActive(true);
            if (player.MediumImage != string.Empty)
            {
                Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Medium/" + player.MediumImage);
            }
            else
            {
                Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Medium/NoImage_M");
            }
            MpBar.gameObject.SetActive(true);
            MpBar.SetValue(player.Info.CurrentMP, player.Info.MaxMP);
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

    void Awake()
    {
        BattleStatusButton.onClick.AddListener(BattleStatusOnClick);
    }
}


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
    public Image[] BuffIcon;

    private bool _isScrollViewShow = false;
    private bool _isClickable = true;
    List<BattleStatus> _statusList = new List<BattleStatus>();

    public void SetData(BattleCharacter character)
    {
        NameLabel.text = character.Name;
        LvLabel.text = "Lv." + character.Lv.ToString();
        HpBar.SetValue(character.CurrentHP, character.MaxHP);

        _statusList = new List<BattleStatus>(character.StatusDic.Values);

        for (int i=0; i<BuffIcon.Length; i++)
        {
            if (i < _statusList.Count)
            {
                BuffIcon[i].gameObject.SetActive(true);
                BuffIcon[i].overrideSprite = Resources.Load<Sprite>("Image/" + _statusList[i].Icon);
            }
            else
            {
                BuffIcon[i].gameObject.SetActive(false);
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
            MpBar.SetValue(player.CurrentMP, player.MaxMP);
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

    void Awake()
    {
    }
}


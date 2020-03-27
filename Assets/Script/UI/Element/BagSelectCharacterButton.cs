using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagSelectCharacterButton : MonoBehaviour
{
    public Action<TeamMember> ButtonOnClickHandler;

    public Image Icon;
    public Text NameLabel;
    public ValueBar HPBar;
    public ValueBar MPBar;
    public Button Button;

    private TeamMember _member;

    public void SetData(TeamMember member)
    {
        _member = member;
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Character/Small" + member.Data.SmallImage);
        NameLabel.text = member.Data.GetName();
        HPBar.SetValue(member.CurrentHP, member.MaxHP);
        MPBar.SetValue(member.CurrentMP, member.MaxMP);
    }

    public void HPBarTween()
    {
        HPBar.SetValueTween(_member.CurrentHP, _member.MaxHP, null);
    }

    public void MPBarTween()
    {
        MPBar.SetValueTween(_member.CurrentMP, _member.MaxMP, null);
    }

    private void ButtonOnClick()
    {
        if (ButtonOnClickHandler != null)
        {
            ButtonOnClickHandler(_member);
        }
    }

    private void Awake()
    {
        Button.onClick.AddListener(ButtonOnClick);
    }
}

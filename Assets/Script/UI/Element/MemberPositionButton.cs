using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberPositionButton : MonoBehaviour
{
    public Action<MemberPositionButton> OnClickHandler;
    public Action<MemberPositionButton> OnDownHandler;
    public Action<MemberPositionButton> OnUpHandler;

    public ButtonPlus Button;
    public Image Image;
    public Vector2Int MemberPosition; //隊形的位置
    public TeamMember Member;

    public void SetData(TeamMember member)
    {
        Member = member;
        if (member != null)
        {
            Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + member.Data.SmallImage);
        }
        else
        {
            Image.overrideSprite = null;
        }
    }

    private void OnClick(ButtonPlus button)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler(this);
        }
    }

    private void OnDown(ButtonPlus button)
    {
        if (Image.overrideSprite != null)
        {
            Image.transform.SetParent(transform.parent.parent);

            if (OnDownHandler != null)
            {
                OnDownHandler(this);
            }
        }
    }

    private void OnPress(ButtonPlus button)
    {
        if (Image.overrideSprite != null)
        {
            Image.transform.position = Input.mousePosition;
        }
    }

    private void OnUp(ButtonPlus button)
    {
        if (Image.overrideSprite != null)
        {
            Image.transform.SetParent(transform);
            Image.transform.localPosition = Vector3.zero;

            if (OnUpHandler != null)
            {
                OnUpHandler(this);
            }
        }
    }

    private void Awake()
    {
        Button.ClickHandler = OnClick;
        Button.DownHandler = OnDown;
        Button.PressHandler = OnPress;
        Button.UpHandler = OnUp;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberPositionButton : MonoBehaviour
{
    public Action<MemberPositionButton> OnClickHandler;

    public Button Button;
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

    private void OnClick()
    {
        if (OnClickHandler != null)
        {
            OnClickHandler(this);
        }
    }

    private void Awake()
    {
        Button.onClick.AddListener(OnClick);
    }
}

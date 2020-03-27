using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPositionGroup : MonoBehaviour
{
    public MemberPositionButton[] MemberPositionButton;

    private Vector3 _startDragPosition = new Vector3();
    private MemberPositionButton SelectButton_1 = null;
    private MemberPositionButton SelectButton_2 = null;

    public void SetData()
    {
        TeamMember member;
        Vector2Int position;
        Image bg = null;
        for (int i=0; i< TeamManager.Instance.MemberList.Count; i++)
        {
            member = TeamManager.Instance.MemberList[i];
            position = TeamManager.Instance.MemberPositionDic[member];

            for (int j=0; j< MemberPositionButton.Length; j++)
            {
                if (position == MemberPositionButton[j].MemberPosition)
                {
                    MemberPositionButton[j].SetData(member);
                }
            }
        }
    }

    private void ButtonOnClick(MemberPositionButton button)
    {
        TeamMember temp;

        if (SelectButton_1 == null)
        {
            SelectButton_1 = button;
        }
        else
        {
            SelectButton_2 = button;
            temp = SelectButton_2.Member;
            SelectButton_2.SetData(SelectButton_1.Member);
            SelectButton_1.SetData(temp);
            if (SelectButton_1.Member != null)
            {
                TeamManager.Instance.MemberPositionDic[SelectButton_1.Member] = SelectButton_1.MemberPosition;
            }
            if (SelectButton_2.Member != null)
            {
                TeamManager.Instance.MemberPositionDic[SelectButton_2.Member] = SelectButton_2.MemberPosition;
            }
            SelectButton_1 = null;
            SelectButton_2 = null;
        }
    }

    private void Awake()
    {
        for (int i=0; i<MemberPositionButton.Length; i++)
        {
            MemberPositionButton[i].OnClickHandler = ButtonOnClick;
        }
    }
}

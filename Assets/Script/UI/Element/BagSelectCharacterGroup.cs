using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagSelectCharacterGroup : MonoBehaviour
{
    public Action<TeamMember> CharacterOnClickHandler;

    public Button CloseButton;
    public BagSelectCharacterButton[] CharacterButtons;

    private bool _isClickable = true;
    private Timer _timer = new Timer();
    private Dictionary<TeamMember, BagSelectCharacterButton> CharacterButtonDic = new Dictionary<TeamMember, BagSelectCharacterButton>();

    public void SetData()
    {
        for (int i = 0; i < CharacterButtons.Length; i++)
        {

            List<TeamMember> memberList = TeamManager.Instance.GetAttendList();
            if (i < memberList.Count)
            {
                CharacterButtons[i].gameObject.SetActive(true);
                CharacterButtons[i].SetData(memberList[i]);
                CharacterButtonDic.Add(memberList[i], CharacterButtons[i]);
            }
            else
            {
                CharacterButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HPBarTween(TeamMember member)
    {
        CharacterButtonDic[member].HPBarTween();
    }

    public void MPBarTween(TeamMember member)
    {
        CharacterButtonDic[member].MPBarTween();
    }

    private void CharacterOnClick(TeamMember member)
    {
        if (!_isClickable)
        {
            return;
        }

        _isClickable = false;
        if (CharacterOnClickHandler != null)
        {
            CharacterOnClickHandler(member);
        }

        _timer.Start(0.5f, ()=> 
        {
            _isClickable = true;
            gameObject.SetActive(false);
        });
    }

    private void CloseOnClick()
    {
        gameObject.SetActive(false);
    }

    void Awake()
    {
        CloseButton.onClick.AddListener(CloseOnClick);

        for (int i=0; i<CharacterButtons.Length; i++)
        {
            CharacterButtons[i].ButtonOnClickHandler = CharacterOnClick;
        }  
    }
}

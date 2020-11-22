using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationUI : MonoBehaviour
{
    public static FormationUI Instance;

    public Button CloseButton;
    public MemberPositionButton[] MemberPositionButton;

    private Vector3 _startDragPosition = new Vector3();
    private MemberPositionButton _selectButton_1 = null;
    private MemberPositionButton _selectButton_2 = null;

    public static void Open()
    {
        ExploreController.Instance.StopEnemy();
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("FormationUI", ResourceManager.Type.UI).GetComponent<FormationUI>();
        }
        Instance.Init();
    }

    public static void Close()
    {
        ExploreController.Instance.ContinueEnemy();
        ExploreUI.SetCanMove();
        Destroy(Instance.gameObject);
        Instance = null;
    }

    public void Init()
    {
        TeamMember member;
        Vector2Int position;

        for (int j = 0; j < MemberPositionButton.Length; j++)
        {
            MemberPositionButton[j].SetData(null);
        }

        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            member = TeamManager.Instance.MemberList[i];
            position = member.Formation;

            for (int j = 0; j < MemberPositionButton.Length; j++)
            {
                if (position == MemberPositionButton[j].MemberPosition)
                {
                    MemberPositionButton[j].SetData(member);
                }
            }
        }
    }

    //private void ButtonOnClick(MemberPositionButton button)
    //{
    //    TeamMember temp;

    //    if (_selectButton_1 == null)
    //    {
    //        _selectButton_1 = button;
    //    }
    //    else
    //    {
    //        _selectButton_2 = button;
    //        temp = _selectButton_2.Member;
    //        _selectButton_2.SetData(_selectButton_1.Member);
    //        _selectButton_1.SetData(temp);
    //        if (_selectButton_1.Member != null)
    //        {
    //            _selectButton_1.Member.Formation = _selectButton_1.MemberPosition;
    //        }
    //        if (_selectButton_2.Member != null)
    //        {
    //            _selectButton_2.Member.Formation = _selectButton_2.MemberPosition;
    //        }
    //        _selectButton_1 = null;
    //        _selectButton_2 = null;
    //    }
    //}

    private void ButtonOnDown(MemberPositionButton button)
    {
        _selectButton_1 = button;
    }

    private void ButtonOnUp(MemberPositionButton button)
    {
        if (_selectButton_1 != null)
        {
            _selectButton_2 = MemberPositionButton[0];
            for (int i = 1; i < MemberPositionButton.Length; i++)
            {
                if (Vector2.Distance(MemberPositionButton[i].transform.position, Input.mousePosition) < Vector2.Distance(_selectButton_2.transform.position, Input.mousePosition))
                {
                    _selectButton_2 = MemberPositionButton[i];
                }
            }

            TeamMember temp = _selectButton_2.Member;
            _selectButton_2.SetData(_selectButton_1.Member);
            _selectButton_1.SetData(temp);
            if (_selectButton_1.Member != null)
            {
                _selectButton_1.Member.Formation = _selectButton_1.MemberPosition;
            }
            if (_selectButton_2.Member != null)
            {
                _selectButton_2.Member.Formation = _selectButton_2.MemberPosition;
            }
            _selectButton_1 = null;
            _selectButton_2 = null;
        }
    }

    private void CloseOnClick()
    {
        Close();
    }

    private void Awake()
    {
        CloseButton.onClick.AddListener(CloseOnClick);

        for (int i = 0; i < MemberPositionButton.Length; i++)
        {
            //MemberPositionButton[i].OnClickHandler = ButtonOnClick;
            MemberPositionButton[i].OnDownHandler = ButtonOnDown;
            MemberPositionButton[i].OnUpHandler = ButtonOnUp;
        }
    }
}
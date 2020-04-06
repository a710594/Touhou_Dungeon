using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamUI : MonoBehaviour
{
    private enum State
    {
        Character,
        Skill,
        Equip,
        Position,
    }

    public static TeamUI Instance;

    public ButtonPlus CharacterButton;
    public ButtonPlus SkillButton;
    public ButtonPlus EquipButton;
    public ButtonPlus PositionButton;
    public Button CloseButton;
    public TeamCharacterGroup CharacterGroup;
    public TeamSkillGroup SkillGroup;
    public TeamEquipGroup EquipGroup;
    public TeamPositionGroup PositionGroup;
    public TeamMemberButton[] TeamMemaberButton;

    private TeamMember _selectedMember;
    private State _currentState = State.Character;
    private Dictionary<TeamMember, TeamMemberButton> _teamMemaberButtonDic = new Dictionary<TeamMember, TeamMemberButton>();

    public static void Open()
    {
        Time.timeScale = 0;
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("TeamUI", ResourceManager.Type.UI).GetComponent<TeamUI>();
        }
        Instance.Init();
    }

    public static void Close()
    {
        Time.timeScale = 1;
        Destroy(Instance.gameObject);
        Instance = null;
    }

    public void Init()
    {
        _selectedMember = TeamManager.Instance.MemberList[0];
        _teamMemaberButtonDic[_selectedMember].SetColor(Color.white);
        SetCharacterData();
    }

    private void SetCharacterData()
    {
        CharacterGroup.SetData(_selectedMember);
    }

    private void SetSkillData()
    {
        SkillGroup.SetData(_selectedMember);
    }

    public void SetEquipData()
    {
        EquipGroup.Init(_selectedMember);
    }

    public void SetPositionData()
    {
        PositionGroup.SetData();
    }

    private void TeamMemberOnClick(string text, object data) //左邊的角色欄
    {
        _teamMemaberButtonDic[_selectedMember].SetColor(Color.gray);
        _selectedMember = (TeamMember)data;
        _teamMemaberButtonDic[_selectedMember].SetColor(Color.white);

        if (_currentState == State.Character)
        {
            SetCharacterData();
        }
        else if (_currentState == State.Skill)
        {
            SetSkillData();
        }
        else if (_currentState == State.Equip)
        {
            SetEquipData();
        }
        else
        {
            SetPositionData();
        }
    }

    private void CharacterGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(true);
        SkillGroup.gameObject.SetActive(false);
        EquipGroup.gameObject.SetActive(false);
        PositionGroup.gameObject.SetActive(false);
        CharacterButton.Image.color = Color.white;
        SkillButton.Image.color = Color.gray;
        EquipButton.Image.color = Color.gray;
        PositionButton.Image.color = Color.gray;
        SetCharacterData();
        _currentState = State.Character;
    }

    private void SkillGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(false);
        SkillGroup.gameObject.SetActive(true);
        EquipGroup.gameObject.SetActive(false);
        PositionGroup.gameObject.SetActive(false);
        CharacterButton.Image.color = Color.gray;
        SkillButton.Image.color = Color.white;
        EquipButton.Image.color = Color.gray;
        PositionButton.Image.color = Color.gray;
        SetSkillData();
        _currentState = State.Skill;
    }

    private void EquipGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(false);
        SkillGroup.gameObject.SetActive(false);
        EquipGroup.gameObject.SetActive(true);
        PositionGroup.gameObject.SetActive(false);
        CharacterButton.Image.color = Color.gray;
        SkillButton.Image.color = Color.gray;
        EquipButton.Image.color = Color.white;
        PositionButton.Image.color = Color.gray;
        SetEquipData();
        _currentState = State.Equip;
    }

    private void PositionGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(false);
        SkillGroup.gameObject.SetActive(false);
        EquipGroup.gameObject.SetActive(false);
        PositionGroup.gameObject.SetActive(true);
        CharacterButton.Image.color = Color.gray;
        SkillButton.Image.color = Color.gray;
        EquipButton.Image.color = Color.gray;
        PositionButton.Image.color = Color.white;
        SetPositionData();
        _currentState = State.Position;
    }

    private void CloseOnClick()
    {
        Close();
    }

    private void Awake()
    {
        CharacterGroup.gameObject.SetActive(true);
        SkillGroup.gameObject.SetActive(false);

        CharacterButton.ClickHandler = CharacterGroupOnClick;
        SkillButton.ClickHandler = SkillGroupOnClick;
        EquipButton.ClickHandler = EquipGroupOnClick;
        PositionButton.ClickHandler = PositionGroupOnClick;
        CloseButton.onClick.AddListener(CloseOnClick);

        for (int i=0; i<TeamMemaberButton.Length; i++)
        {
            if (i < TeamManager.Instance.MemberList.Count)
            {
                TeamMemaberButton[i].OnClickHandler = TeamMemberOnClick;
                TeamMemaberButton[i].SetData(TeamManager.Instance.MemberList[i].Data.GetName(), TeamManager.Instance.MemberList[i]);
                TeamMemaberButton[i].SetColor(Color.gray);
                TeamMemaberButton[i].gameObject.SetActive(true);
                _teamMemaberButtonDic.Add(TeamManager.Instance.MemberList[i], TeamMemaberButton[i]);
            }
            else
            {
                TeamMemaberButton[i].gameObject.SetActive(false);
            }
        }
    }
}

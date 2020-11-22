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
        SpellCard,
        Equip,
        Position,
    }

    public static TeamUI Instance;

    public ButtonPlus CharacterButton;
    public ButtonPlus SkillButton;
    public ButtonPlus SpellCardButton;
    public ButtonPlus EquipButton;
    public Button CloseButton;
    public TeamCharacterGroup CharacterGroup;
    public TeamSkillGroup SkillGroup;
    public TeamEquipGroup EquipGroup;
    public TextButton[] TeamMemaberButton;

    private TeamMember _selectedMember;
    private State _currentState = State.Character;
    private Dictionary<TeamMember, TextButton> _teamMemaberButtonDic = new Dictionary<TeamMember, TextButton>();

    public static void Open()
    {
        ExploreController.Instance.StopEnemy();
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("TeamUI", ResourceManager.Type.UI).GetComponent<TeamUI>();
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
        _selectedMember = TeamManager.Instance.MemberList[0];
        _teamMemaberButtonDic[_selectedMember].SetColor(Color.white);
        SetCharacterData();
    }

    public void SetCharacterData()
    {
        CharacterGroup.SetData(_selectedMember);
    }

    private void SetSkillData()
    {
        SkillGroup.Init(_selectedMember, false);
    }

    private void SetSpellCardData()
    {
        SkillGroup.Init(_selectedMember, true);
    }

    public void SetEquipData()
    {
        EquipGroup.Init(_selectedMember);
    }

    private void TeamMemberOnClick(object data) //左邊的角色欄
    {
        _selectedMember = (TeamMember)data;

        if (_currentState == State.Character)
        {
            SetCharacterData();
        }
        else if (_currentState == State.Skill)
        {
            SetSkillData();
        }
        else if (_currentState == State.SpellCard)
        {
            SetSpellCardData();
        }
        else if (_currentState == State.Equip)
        {
            SetEquipData();
        }
    }

    private void CharacterGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(true);
        SkillGroup.gameObject.SetActive(false);
        EquipGroup.gameObject.SetActive(false);
        SetCharacterData();
        _currentState = State.Character;
    }

    private void SkillGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(false);
        SkillGroup.gameObject.SetActive(true);
        EquipGroup.gameObject.SetActive(false);
        SetSkillData();
        _currentState = State.Skill;
    }

    private void SpellCardGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(false);
        SkillGroup.gameObject.SetActive(true);
        EquipGroup.gameObject.SetActive(false);
        SetSpellCardData();
        _currentState = State.SpellCard;
    }

    private void EquipGroupOnClick(object obj)
    {
        CharacterGroup.gameObject.SetActive(false);
        SkillGroup.gameObject.SetActive(false);
        EquipGroup.gameObject.SetActive(true);
        SetEquipData();
        _currentState = State.Equip;
    }

    private void CloseOnClick()
    {
        Close();
    }

    private void Awake()
    {
        CharacterGroup.gameObject.SetActive(true);
        SkillGroup.gameObject.SetActive(false);
        EquipGroup.gameObject.SetActive(false);

        CharacterButton.ClickHandler = CharacterGroupOnClick;
        SkillButton.ClickHandler = SkillGroupOnClick;
        SpellCardButton.ClickHandler = SpellCardGroupOnClick;
        EquipButton.ClickHandler = EquipGroupOnClick;
        CloseButton.onClick.AddListener(CloseOnClick);

        for (int i=0; i<TeamMemaberButton.Length; i++)
        {
            if (i < TeamManager.Instance.MemberList.Count)
            {
                TeamMemaberButton[i].OnClickHandler = TeamMemberOnClick;
                TeamMemaberButton[i].SetData(TeamManager.Instance.MemberList[i].Data.GetName(), TeamManager.Instance.MemberList[i]);
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

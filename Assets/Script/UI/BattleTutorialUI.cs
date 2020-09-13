using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTutorialUI : MonoBehaviour
{
    public static BattleTutorialUI Instance;

    public Button MoveActionButton;
    public Button MoveButton;
    public Button MoveConfirmButton;
    public Button[] SkillActionButtons;
    public Button[] SkillButtons;
    public Button[] SkillPositionButtons;
    public Button[] SkillConfirmButtons;
    public Button[] ContinueButtons;
    public Button[] IdleButtons;
    public GameObject Mask;
    public GameObject[] StepGroup;

    private int _currentStep = 0;
    private Timer _timer = new Timer();

    public static void Open()
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BattleTutorialUI", ResourceManager.Type.UI).GetComponent<BattleTutorialUI>();
        }
    }

    public void NextStep()
    {
        if (_currentStep - 1 >= 0)
        {
            StepGroup[_currentStep - 1].SetActive(false);
        }
        if (_currentStep < StepGroup.Length)
        {
            StepGroup[_currentStep].SetActive(true);
            _currentStep++;
        }
    }

    GameObject reimu;
    GameObject sanae;
    private void Step_8()
    {
        reimu = GameObject.Find("Reimu");
        reimu.GetComponent<BattleCharacter>().SetActive(true);

        sanae = GameObject.Find("Sanae");
        sanae.GetComponent<BattleCharacter>().SetActive(true);

        StepGroup[_currentStep - 1].SetActive(false);
        Mask.SetActive(true);
        BattleController.Instance.TurnStartHandler += TurnStart;
    }

    private void TurnStart()
    {
        reimu.transform.position = new Vector3(0, 0, 0);
        sanae.transform.position = new Vector3(-1, 0, 0);
        NextStep();
        Mask.SetActive(false);
        BattleController.Instance.TurnStartHandler -= TurnStart;
    }

    private void MoveActionOnClick()
    {
        BattleUI.Instance.MoveActionOnClick();
        NextStep();
    }

    private void MoveOnClick()
    {
        ScreenOnClick();
        NextStep();
    }

    private void MoveConfirmOnClick()
    {
        BattleController.Instance.MoveConfirm();
        NextStep();
    }

    public void SkillActionOnClick()
    {
        BattleUI.Instance.SkillActionOnClick();
        NextStep();
    }

    public void SkillOnClick(int index)
    {
        Skill skill = BattleController.Instance.SelectedCharacter.Info.SkillList[index];
        List<Vector2Int> positionList = skill.GetDistance(BattleController.Instance.SelectedCharacter);
        BattleController.Instance.SelectSkill(skill);

        for (int i = 0; i < positionList.Count; i++)
        {
            TilePainter.Instance.Painting("RedGrid", 2, positionList[i]);
        }

        BattleUI.Instance.SkillInfoUI.gameObject.SetActive(true);
        BattleUI.Instance.SkillInfoUI.SetData(skill.Data, skill.Lv);
        NextStep();
    }

    private void SkillPositionOnClick()
    {
        ScreenOnClick();
        NextStep();
    }

    private void SkillConfirmOnClick()
    {
        ScreenOnClick();
        if (_currentStep == 8)
        {
            Step_8();
        }
        else
        {
            StepGroup[_currentStep - 1].SetActive(false);
            Mask.SetActive(true);
            BattleController.Instance.SelectActionStartHandler = ShowEnd;
        }
    }

    private void ShowEnd()
    {
        NextStep();
        Mask.SetActive(false);
        BattleController.Instance.SelectActionStartHandler -= ShowEnd;
    }

    private void ContinueOnClick()
    {
        NextStep();
    }

    private void IdleOnClick()
    {
        BattleUI.Instance.IdleOnClick();
        if (_currentStep == 23 || _currentStep == 32)
        {
            StepGroup[_currentStep - 1].SetActive(false);
            Mask.SetActive(true);
            BattleController.Instance.SelectActionStartHandler = ShowEnd;
        }
        else
        {
            NextStep();
        }
    }

    private void ScreenOnClick()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int intPosition = Vector2Int.RoundToInt(worldPosition);
        intPosition.y -= 1; //地圖座標的偏移值
        BattleController.Instance.ScreenOnClick(intPosition);
    }

    private void Awake()
    {
        MoveActionButton.onClick.AddListener(MoveActionOnClick);
        MoveButton.onClick.AddListener(MoveOnClick);
        MoveConfirmButton.onClick.AddListener(MoveConfirmOnClick);
        for (int i=0; i<SkillActionButtons.Length; i++)
        {
            SkillActionButtons[i].onClick.AddListener(SkillActionOnClick);
        }

        SkillButtons[0].onClick.AddListener(()=> 
        {
            SkillOnClick(0);
        });
        SkillButtons[1].onClick.AddListener(() =>
        {
            SkillOnClick(1);
        });
        SkillButtons[2].onClick.AddListener(() =>
        {
            SkillOnClick(1);
        });
        SkillButtons[3].onClick.AddListener(() =>
        {
            SkillOnClick(0);
        });
        SkillButtons[4].onClick.AddListener(() =>
        {
            SkillOnClick(0);
        });
        SkillButtons[5].onClick.AddListener(() =>
        {
            SkillOnClick(5);
        });

        for (int i=0; i<SkillPositionButtons.Length; i++)
        {
            SkillPositionButtons[i].onClick.AddListener(SkillPositionOnClick);
        }
        for (int i=0; i<SkillConfirmButtons.Length; i++)
        {
            SkillConfirmButtons[i].onClick.AddListener(SkillConfirmOnClick);
        }
        for (int i = 0; i < ContinueButtons.Length; i++)
        {
            ContinueButtons[i].onClick.AddListener(ContinueOnClick);
        }

        for (int i = 0; i < IdleButtons.Length; i++)
        {
            IdleButtons[i].onClick.AddListener(IdleOnClick);
        }

        for (int i=0; i<StepGroup.Length; i++)
        {
            StepGroup[i].SetActive(false);
        }

        Mask.SetActive(true);
        _timer.Start(0.5f, ()=> 
        {
            Mask.SetActive(false);
            NextStep();
        });
    }
}

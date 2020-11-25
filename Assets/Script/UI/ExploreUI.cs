using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreUI : MonoBehaviour
{
    public static ExploreUI Instance;

    public Button OpenMapButton;
    public Button CloseMapButton;
    public Button BagButton;
    public Button TeamButton;
    public Button FormationButton;
    public Button CookButton;
    public Button SaveButton;
    public Button CloseMoveTutorialButton;
    public ButtonPlus[] InteractiveButtons;
    public MapUI MapUI;
    public TipLabel TipLabel;
    public GameObject LoadingGroup;
    public StairsGroup StairsGroup;
    public Joystick Joystick;

    private bool _canMove = true;  //角色是否可移動
    private bool _isJoystickMoving = false;
    private bool _isKeyboardMoving = false;

    public static void Open()
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("ExploreUI", ResourceManager.Type.UI).GetComponent<ExploreUI>();
        }
    }

    public static void Close()
    {
        Destroy(Instance.gameObject);
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public static void SetCanMove()
    {
        if (Instance != null)
        {
            Instance._canMove = true;
        }
    }

    public void InitLittleMap(int floor, Vector2Int playerPosition, Vector2Int startPosition, Vector2Int goalPosition, BoundsInt mapBound, List<Vector2Int> mapList)
    {
        MapUI.Init(floor, playerPosition, startPosition, goalPosition, mapBound, mapList);
    }

    public void RefreshLittleMap(Vector2Int characterPosition, List<Vector2Int> exploredList, List<Vector2Int> wallList)
    {
        MapUI.Refresh(characterPosition, exploredList, wallList);
    }

    public void SetInetractiveButtonVisible(List<Vector2Int> positionList)
    {
        for (int i=0; i< InteractiveButtons.Length; i++)
        {
            if (i < positionList.Count)
            {
                InteractiveButtons[i].gameObject.SetActive(true);
                InteractiveButtons[i].transform.position = Camera.main.WorldToScreenPoint((Vector2)positionList[i] + Vector2.up);
                InteractiveButtons[i].SetData(positionList[i]);
            }
            else
            {
                InteractiveButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenStairsGroup(int floor)
    {
        _canMove = false;
        StairsGroup.Open(floor, ()=> 
        {
            _canMove = true;
        });
    }

    public void StopTipLabel()
    {
        TipLabel.Stop();
    }

    public void SetLoadingGroup(bool isVisible) 
    {
        LoadingGroup.SetActive(isVisible);
    }

    private void OpenMapGroup()
    {
        _canMove = false;
        MapUI.SetBigMapGroupVisible(true);
        ExploreController.Instance.StopEnemy(); 
    }

    private void CloseMapGroup()
    {
        _canMove = true;
        MapUI.SetBigMapGroupVisible(false);
        ExploreController.Instance.ContinueEnemy();
    }

    private void OpenBag()
    {
        _canMove = false;
        BagUI.Open(ItemManager.Type.Bag);
    }

    private void OpenTeam()
    {
        _canMove = false;
        TeamUI.Open();
    }

    private void OpenFormation() 
    {
        _canMove = false;
        FormationUI.Open();
    }

    private void OpenCook()
    {
        _canMove = false;
        NewCookUI.Open(ItemManager.Type.Bag);
    }

    private void Save() 
    {
        GameSystem.Instance.SaveGame();
        TipLabel.SetLabel("存檔成功");
    }

    private void InteractiveOnClick(ButtonPlus button)
    {
        button.gameObject.SetActive(false);
        ExploreController.Instance.Interactive((Vector2Int)button.Data);
    }

    private void CloseMoveTutorialOnClick() 
    {
        CloseMoveTutorialButton.gameObject .SetActive(false);
        ProgressManager.Instance.Memo.MoveTutorial = true;
    }

    void Awake()
    {
        Instance = this; //temp

        OpenMapButton.onClick.AddListener(OpenMapGroup);
        CloseMapButton.onClick.AddListener(CloseMapGroup);
        BagButton.onClick.AddListener(OpenBag);
        TeamButton.onClick.AddListener(OpenTeam);
        FormationButton.onClick.AddListener(OpenFormation);
        CookButton.onClick.AddListener(OpenCook);
        SaveButton.onClick.AddListener(Save);
        CloseMoveTutorialButton.onClick.AddListener(CloseMoveTutorialOnClick);

        for (int i=0; i<InteractiveButtons.Length; i++)
        {
            InteractiveButtons[i].ClickHandler = InteractiveOnClick;
        }

        SaveButton.interactable = (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore);

        StairsGroup.gameObject.SetActive(false);
        CloseMoveTutorialButton.gameObject.SetActive(!ProgressManager.Instance.Memo.MoveTutorial);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ExploreController.Instance.ForceEnterBattle();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ExploreController.Instance.BackToVilliage();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Plot_8 plot_8 = new Plot_8();
            plot_8.Start();
        }

        if (_canMove)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                ExploreController.Instance.PlayerMove(Vector2Int.left);
                _isKeyboardMoving = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                ExploreController.Instance.PlayerMove(Vector2Int.right);
                _isKeyboardMoving = true;
            }
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                ExploreController.Instance.PlayerMove(Vector2Int.up);
                _isKeyboardMoving = true;
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                ExploreController.Instance.PlayerMove(Vector2Int.down);
                _isKeyboardMoving = true;
            }
            else
            {
                if (_isKeyboardMoving)
                {
                    ExploreController.Instance.PlayerPause();
                    _isKeyboardMoving = false;
                }
            }

            Vector2Int direction = new Vector2Int(Mathf.RoundToInt(Joystick.Horizontal), Mathf.RoundToInt(Joystick.Vertical));
            if (direction != Vector2Int.zero)
            {
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    direction.y = 0;
                }
                else
                {
                    direction.x = 0;
                }

                ExploreController.Instance.PlayerMove(direction);
                _isJoystickMoving = true;
            }
            else
            {
                if (_isJoystickMoving)
                {
                    ExploreController.Instance.PlayerPause();
                    _isJoystickMoving = false;
                }
            }
        }
    }
}

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
        Instance = null;
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
        MapUI.SetBigMapGroupVisible(true);
        ExploreController.Instance.StopEnemy(); 
    }

    private void CloseMapGroup()
    {
        MapUI.SetBigMapGroupVisible(false);
        ExploreController.Instance.ContinueEnemy();
    }

    private void OpenBag()
    {
        BagUI.Open(ItemManager.Type.Bag);
    }

    private void OpenTeam()
    {
        TeamUI.Open();
    }

    private void OpenFormation() 
    {
        FormationUI.Open();
    }

    private void OpenCook()
    {
        CookUI.Open(ItemManager.Type.Bag);
    }

    private void Save() 
    {
        GameSystem.Instance.SaveMemo();
    }

    private void InteractiveOnClick(ButtonPlus button)
    {
        button.gameObject.SetActive(false);
        ExploreController.Instance.Interactive((Vector2Int)button.Data);
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

        for (int i=0; i<InteractiveButtons.Length; i++)
        {
            InteractiveButtons[i].ClickHandler = InteractiveOnClick;
        }
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

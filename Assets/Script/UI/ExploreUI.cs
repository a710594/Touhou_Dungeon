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
    public Button CookButton;
    public ButtonPlus[] InteractiveButtons;
    public LittleMap LittleMap;
    public TipLabel TipLabel;
    public GameObject MapGroup;
    public StairsGroup StairsGroup;
    public Joystick Joystick;

    private bool _canMove = true;  //角色是否可移動

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

    public void InitLittleMap(Vector2Int playerPosition, Vector2Int startPosition, Vector2Int goalPosition, BoundsInt mapBound, List<Vector2Int> mapList)
    {
        LittleMap.Init(playerPosition, startPosition, goalPosition, mapBound, mapList);
    }

    public void RefreshLittleMap(Vector2Int characterPosition, List<Vector2Int> exploredList, List<Vector2Int> wallList)
    {
        LittleMap.Refresh(characterPosition, exploredList, wallList);
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

    private void OpenMapGroup()
    {
        MapGroup.SetActive(true);
        Time.timeScale = 0;
    }

    private void CloseMapGroup()
    {
        MapGroup.SetActive(false);
        Time.timeScale = 1;
    }

    private void OpenBag()
    {
        BagUI.Open(ItemManager.Type.Bag);
    }

    private void OpenTeam()
    {
        TeamUI.Open();
    }

    private void OpenCook()
    {
        CookUI.Open(ItemManager.Type.Bag);
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
        CookButton.onClick.AddListener(OpenCook);

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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExploreController.Instance.ForceEnterBattle();
        }

        Vector2Int direction = new Vector2Int(Mathf.RoundToInt(Joystick.Horizontal), Mathf.RoundToInt(Joystick.Vertical));
        if (direction != Vector2Int.zero && _canMove)
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
        }
        else 
        {
            ExploreController.Instance.PlayerPause();
        }
    }
}

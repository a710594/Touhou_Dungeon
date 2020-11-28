using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public static Action LanguageChangeHandler;
    public static GameSystem Instance;

    public Text TestText;

    private static bool _exists;

    public void SaveGame()
    {
        MySceneManager.SceneType sceneType = MySceneManager.Instance.CurrentScene;
        if (sceneType == MySceneManager.SceneType.Explore || sceneType == MySceneManager.SceneType.Villiage)
        {
            MySceneManager.Instance.Save();
            ItemManager.Instance.Save();
            TeamManager.Instance.Save();
            ProgressManager.Instance.Save();
            ExploreController.Instance.Save();
        }
    }

    public void AutoSave()
    {
#if !UNITY_EDITOR
        SaveGame();
#endif
    }

    public void ClearMemo()
    {
        Caretaker.Instance.ClearData<SceneMemo>();
        Caretaker.Instance.ClearData<ItemMemo>();
        Caretaker.Instance.ClearData<TeamMemo>();
        Caretaker.Instance.ClearData<ProgressMemo>();
        Caretaker.Instance.ClearData<MapMemo>();
    }

    public void InitManager()
    {
        Caretaker.Instance.Init();
        ExploreController.Instance.Init();
        TeamManager.Instance.Init();
        ItemManager.Instance.Init();
        MySceneManager.Instance.Init();
        ProgressManager.Instance.Init();
    }

    // Start is called before the first frame update
    private IEnumerator Init()
    {
        JobData.Load();
        SkillData.Load();
        EnemyData.Load();
        BattleTileData.Load();
        BattlefieldData.Load();
        BattleGroupData.Load();
        BattleStatusData.Load();
        ItemData.Load();
        EquipData.Load();
        ItemEffectData.Load();
        LanguageData.Load();
        DungeonData.Load();
        RoomData.Load();
        TreasureData.Load();
        CookData.Load();
        ConversationData.Load();
        ShopData.Load();
        DungeonGroupData.Load();
        ExpData.Load();
        NewCookData.Load();

        yield return new WaitForEndOfFrame();

        InitManager();

        MySceneManager.Instance.Load();
    }

    private void Awake()
    {
        if (!_exists)
        {
            _exists = true;
            Instance = this;
            StartCoroutine(Init());
            DontDestroyOnLoad(transform.gameObject);//使物件切換場景時不消失
        }
        else
        {
            Destroy(gameObject); //破壞場景切換後重複產生的物件
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ClearMemo();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AudioSystem.Instance.Stop(true);
            AudioSystem.Instance.Play("Forest", true);
            ExploreController.Instance.GenerateFloor(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AudioSystem.Instance.Stop(true);
            AudioSystem.Instance.Play("Forest", true);
            ExploreController.Instance.GenerateFloor(6);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            EquipUpgradeUI.Open(ItemManager.Type.Warehouse);
        }
    }

    //void OnApplicationPause()
    //{
    //    AutoSave();
    //}

    //void OnApplicationQuit()
    //{
    //    AutoSave();
    //}
}

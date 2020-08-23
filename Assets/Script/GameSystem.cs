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
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore || MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Villiage)
        {
            MySceneManager.Instance.Save();
            ItemManager.Instance.Save();
            TeamManager.Instance.Save();
            ProgressManager.Instance.Save();
            ExploreController.Instance.Save();
            if (BattleController.Instance != null) //現在在戰鬥中
            {
                BattleController.Instance.Save();
            }
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
        Caretaker.Instance.ClearData<BattleMemo>();
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
        DungeonBattleGroupData.Load();
        EventData.Load();
        //EventOptionData.Load();
        CookData.Load();
        ConversationData.Load();
        ShopData.Load();

        yield return new WaitForEndOfFrame();

        Caretaker.Instance.Init();
        ExploreController.Instance.Init();
        TeamManager.Instance.Init();
        ItemManager.Instance.Init();
        MySceneManager.Instance.Init();
        ProgressManager.Instance.Init();

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
            ExploreController.Instance.GenerateFloor(5, ExploreController.InitPlayerPosition.Start);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AudioSystem.Instance.Stop(true);
            AudioSystem.Instance.Play("Forest", true);
            ExploreController.Instance.GenerateFloor(6, ExploreController.InitPlayerPosition.Start);
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

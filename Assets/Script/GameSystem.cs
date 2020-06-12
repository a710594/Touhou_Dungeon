using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public static Action LanguageChangeHandler;

    public Text TestText;

    private static bool _exists;

    public void SaveMemo()
    {
        MySceneManager.Instance.Save();
        ItemManager.Instance.Save();
        TeamManager.Instance.Save();
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
        {
            ExploreController.Instance.Save();
        }
        else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Battle)
        {
            BattleController.Instance.Save();
        }
    }

    public void ClearMemo()
    {
    }

    public void LaodMemo()
    {
        MySceneManager.Instance.Load();
        //ExploreController.Instance.Load();
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
        EventOptionData.Load();
        CookData.Load();
        ConversationData.Load();
        ShopData.Load();

        yield return new WaitForEndOfFrame();

        Caretaker.Instance.Init();
        TeamManager.Instance.Init();
        ItemManager.Instance.Init();
        MySceneManager.Instance.Init();

        LaodMemo();

        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
            {
                ExploreController.Instance.SetFloorFromMemo();
            });
        }
        else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Battle)
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
            {
                BattleController.Instance.InitFromMemo();
            });
        }
    }

    private void Awake()
    {
        if (!_exists)
        {
            _exists = true;
            DontDestroyOnLoad(transform.gameObject);//使物件切換場景時不消失
        }
        else
        {
            Destroy(gameObject); //破壞場景切換後重複產生的物件
            return;
        }

        StartCoroutine(Init());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    LanguageSystem.Instance.ChangeLanguage(LanguageSystem.Language.English);
        //}
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SaveMemo();
        }
    }
}

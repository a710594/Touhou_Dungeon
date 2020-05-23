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

        TeamManager.Instance.Init();
        ItemManager.Instance.Init();
        MySceneManager.Instance.Init();

        string path = Application.streamingAssetsPath + "/Job.json";
        string jsonString;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        jsonString = File.ReadAllText(path);
#elif UNITY_ANDROID
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
        www.SendWebRequest();
        while (!www.isDone)
        {
        }
        jsonString = www.downloadHandler.text;
#endif
        TestText.text = jsonString;

        yield return null;
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
    }
}

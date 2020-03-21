using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static Action LanguageChangeHandler;

    public static LanguageData.Language CurrentLanguage = LanguageData.Language.English;

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
        FoodData.Load();
        LanguageData.Load();

        TeamManager.Instance.Init();
        ItemManager.Instance.Init();
        MySceneManager.Instance.Init();

        yield return new WaitForEndOfFrame();

        List<KeyValuePair<int, int>> enemyList = BattleGroupData.GetEnemy(1);
        BattleController.Instance.Init(1, enemyList);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentLanguage = LanguageData.Language.English;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private static bool _exists;

    // Start is called before the first frame update
    void Awake()
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

        TeamManager.Instance.Init();

        //wait for init
        Timer timer = new Timer();
        timer.Start(1, ()=> 
        {
            List<KeyValuePair<int, int>> enemyList = BattleGroupData.GetEnemy(1);
            BattleController.Instance.Init(1, enemyList);
        });
    }

    private void Update()
    {

    }
}

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

        Job.Load();
        Skill.Load();
        Enemy.Load();

        Enemy.Data data = Enemy.GetData(3);
        for (int i=0; i<10; i++)
        {
            List<int> dropItemList = data.GetDropItemList();
            for (int j=0; j<dropItemList.Count; j++)
            {
                Debug.Log(dropItemList[j]);
            }
            Debug.Log("///");
        }
    }

    private void Update()
    {

    }
}

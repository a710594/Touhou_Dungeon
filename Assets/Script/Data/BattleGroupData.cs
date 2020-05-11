using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BattleGroupData
{
    public class RootObject
    {
        public int ID { get; set; }
        public int Enemy_1 { get; set; }
        public int Lv_1 { get; set; }
        public int Enemy_2 { get; set; }
        public int Lv_2 { get; set; }
        public int Enemy_3 { get; set; }
        public int Lv_3 { get; set; }
        public int Exp;

        public List<int> EnemyList = new List<int>();
        public List<int> LvList = new List<int>();

        public List<KeyValuePair<int, int>> GetEnemy(int id) //id, lv
        {
            List<KeyValuePair<int, int>> enemyList = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < EnemyList.Count; i++)
            {
                enemyList.Add(new KeyValuePair<int, int>(EnemyList[i], LvList[i]));
            }

            return enemyList;
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/BattleGroup");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].Enemy_1 != 0)
            {
                dataList[i].EnemyList.Add(dataList[i].Enemy_1);
                dataList[i].LvList.Add(dataList[i].Lv_1);
            }

            if (dataList[i].Enemy_2 != 0)
            {
                dataList[i].EnemyList.Add(dataList[i].Enemy_2);
                dataList[i].LvList.Add(dataList[i].Lv_2);
            }

            if (dataList[i].Enemy_3 != 0)
            {
                dataList[i].EnemyList.Add(dataList[i].Enemy_3);
                dataList[i].LvList.Add(dataList[i].Lv_3);
            }

            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }
}

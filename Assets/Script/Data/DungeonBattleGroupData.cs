using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DungeonBattleGroupData
{
    public class RootObject
    {
        public int ID { get; set; }
        public int BattleGroup_1 { get; set; }
        public int Probability_1 { get; set; }
        public int BattleGroup_2 { get; set; }
        public int Probability_2 { get; set; }

        public List<int> BattleGroupList = new List<int>();
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/DungeonBattleGroup");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            for (int j = 0; j < dataList[i].Probability_1; j++)
            {
                dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_1);
            }
            for (int j = 0; j < dataList[i].Probability_2; j++)
            {
                dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_2);
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

    public static int GetRandomBattleGroup(int id)
    {
        RootObject data = GetData(id);
        return data.BattleGroupList[UnityEngine.Random.Range(0, data.BattleGroupList.Count)];
    }
}

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
        public int BattleGroup_3 { get; set; }
        public int Probability_3 { get; set; }
        public int GoalBattleGroup { get; set; }

        public List<int> BattleGroupList = new List<int>();

        public int GetRandomBattleGroup()
        {
            return BattleGroupList[Random.Range(0, BattleGroupList.Count)];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/DungeonBattleGroup");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].BattleGroup_1 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_1; j++)
                {
                    dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_1);
                }
            }
            if (dataList[i].BattleGroup_2 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_2; j++)
                {
                    dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_2);
                }
            }
            if (dataList[i].BattleGroup_3 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_3; j++)
                {
                    dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_3);
                }
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

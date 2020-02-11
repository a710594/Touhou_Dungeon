using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BattleStatusData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Icon { get; set; }
        public string Field { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MTK { get; set; }
        public int MEF { get; set; }
        public int AGI { get; set; }
        public int SEN { get; set; }
        public int MoveDistance { get; set; }
        public int Damage { get; set; }
        public int Probability { get; set; }
        public int Turn { get; set; }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/BattleStatus");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
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

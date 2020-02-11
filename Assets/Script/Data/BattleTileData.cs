using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BattleTileData
{
    public class Data
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MoveCost { get; set; }
    }

    private static Dictionary<int, Data> _dataDic = new Dictionary<int, Data>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/BattleTile");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<Data>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static Data GetData(int id)
    {
        Data data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }
}

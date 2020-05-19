using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TreasureData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public int Item_1 { get; set; }
        public int Probability_1 { get; set; }
        public int Item_2 { get; set; }
        public int Probability_2 { get; set; }

        public List<int> ItemList = new List<int>();

        public List<int> GetItemList()
        {
            List<int> list = new List<int>();
            int amount = UnityEngine.Random.Range(MinAmount, MaxAmount + 1);
            for (int i = 0; i < amount; i++)
            {
                list.Add(ItemList[UnityEngine.Random.Range(0, ItemList.Count)]);
            }
            return list;
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/Treasure.json";
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
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            for (int j = 0; j < dataList[i].Probability_1; j++)
            {
                dataList[i].ItemList.Add(dataList[i].Item_1);
            }
            for (int j = 0; j < dataList[i].Probability_2; j++)
            {
                dataList[i].ItemList.Add(dataList[i].Item_2);
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

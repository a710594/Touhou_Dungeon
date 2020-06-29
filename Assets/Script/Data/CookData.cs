using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CookData
{
    public class RootObject
    {
        public int ID { get; set; }
        public int ResultID { get; set; }
        public int Material_1 { get; set; }
        public int Amount_1 { get; set; }
        public int Material_2 { get; set; }
        public int Amount_2 { get; set; }
        public int Material_3 { get; set; }
        public int Amount_3 { get; set; }
        public int Material_4 { get; set; }
        public int Amount_4 { get; set; }
        public int Material_5 { get; set; }
        public int Amount_5 { get; set; }

        public List<int> MaterialList = new List<int>();
        public List<int> AmountList = new List<int>();
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/Cook.json";
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
            if (dataList[i].Material_1 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_1);
                dataList[i].AmountList.Add(dataList[i].Amount_1);
            }
            if (dataList[i].Material_2 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_2);
                dataList[i].AmountList.Add(dataList[i].Amount_2);
            }
            if (dataList[i].Material_3 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_3);
                dataList[i].AmountList.Add(dataList[i].Amount_3);
            }
            if (dataList[i].Material_4 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_4);
                dataList[i].AmountList.Add(dataList[i].Amount_4);
            }
            if (dataList[i].Material_5 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_5);
                dataList[i].AmountList.Add(dataList[i].Amount_5);
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

    public static List<RootObject> GetAllData()
    {
        return new List<RootObject>(_dataDic.Values);
    }
}

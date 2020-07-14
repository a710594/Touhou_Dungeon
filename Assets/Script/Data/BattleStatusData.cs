using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BattleStatusData
{
    public enum TypeEnum
    {
        ATK = 1,
        DEF,
        MTK,
        MEF,
        AGI,
        SEN,
        MOV,
        Damage,
        Probability,
        Striking,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public string Icon { get; set; }
        public string Field { get; set; }
        public int Value { get; set; }
        public TypeEnum ValueType { get; set; }
        public int Turn { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/BattleStatus.json";
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

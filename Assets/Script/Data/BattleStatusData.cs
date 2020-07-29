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
        NoDamage, //無敵
    }

    public class RootObject
    {
        public int ID { get; set; }
        public string Icon { get; set; }
        public string Field { get; set; }
        public int Value_1 { get; set; }
        public int Value_2 { get; set; }
        public int Value_3 { get; set; }
        public int Value_4 { get; set; }
        public int Value_5 { get; set; }
        public TypeEnum ValueType { get; set; }
        public int Turn { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; }

        public List<int> ValueList = new List<int>();

        public string GetComment(int lv)
        {
            string result;
            if (lv <= ValueList.Count)
            {
                result = Comment.Replace("{}", GetCommentValue(lv));
            }
            else
            {
                result = Comment;
            }
            return result;
        }

        public string GetCommentValue(int lv)
        {
            string value = string.Empty;
            if (lv <= ValueList.Count)
            {
                if (ValueList[lv - 1] >= 100)
                {
                    value = (ValueList[lv - 1] - 100).ToString();
                }
                else
                {
                    value = (100 - ValueList[lv - 1]).ToString();
                }
            }
            return value;
        }
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
            if (dataList[i].Value_1 != 0)
            {
                dataList[i].ValueList.Add(dataList[i].Value_1);
            }
            if (dataList[i].Value_2 != 0)
            {
                dataList[i].ValueList.Add(dataList[i].Value_2);
            }
            if (dataList[i].Value_3 != 0)
            {
                dataList[i].ValueList.Add(dataList[i].Value_3);
            }
            if (dataList[i].Value_4 != 0)
            {
                dataList[i].ValueList.Add(dataList[i].Value_4);
            }
            if (dataList[i].Value_5 != 0)
            {
                dataList[i].ValueList.Add(dataList[i].Value_5);
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

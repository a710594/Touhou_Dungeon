using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LanguageData
{
    public enum Language
    {
        Chinese,
        English,
    }
    public class RootObject
    {
        public int ID { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Language");
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

    public static string GetText(int id, Language language)
    {
        RootObject data = null;
        if (_dataDic.TryGetValue(id, out data))
        {
            if (language == Language.Chinese)
            {
                return data.Chinese;
            }
            else
            {
                return data.English;
            }
        }
        else
        {
            return string.Empty;
        }
    }
}

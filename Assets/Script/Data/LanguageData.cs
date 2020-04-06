﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LanguageData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string Chinese { get; set; }
        public string English { get; set; }

        public Dictionary<LanguageSystem.Language, string> LanguageDic = new Dictionary<LanguageSystem.Language, string>();
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Language");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            dataList[i].LanguageDic.Add(LanguageSystem.Language.Chinese, dataList[i].Chinese);
            dataList[i].LanguageDic.Add(LanguageSystem.Language.English, dataList[i].English);
            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }

    public static string GetText(int id, LanguageSystem.Language language)
    {
        RootObject data = null;
        if (_dataDic.TryGetValue(id, out data))
        {
            return data.LanguageDic[language];
        }
        else
        {
            return string.Empty;
        }
    }
}
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BattleTileData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string TileName { get; set; }
        public int MoveCost { get; set; }
        public string Name_Chinese { get; set; }
        public string Name_English { get; set; }
        public string Comment_Chinese { get; set; }
        public string Comment_English { get; set; }

        public Dictionary<LanguageSystem.Language, string> NameDic = new Dictionary<LanguageSystem.Language, string>();
        public Dictionary<LanguageSystem.Language, string> CommentDic = new Dictionary<LanguageSystem.Language, string>();

        public string GetName()
        {
            return NameDic[LanguageSystem.Instance.CurrentLanguage];
        }

        public string GetComment()
        {
            return CommentDic[LanguageSystem.Instance.CurrentLanguage];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/BattleTile.json";
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
            dataList[i].NameDic.Add(LanguageSystem.Language.Chinese, dataList[i].Name_Chinese);
            dataList[i].NameDic.Add(LanguageSystem.Language.English, dataList[i].Name_English);

            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_Chinese);
            dataList[i].CommentDic.Add(LanguageSystem.Language.English, dataList[i].Comment_English);

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

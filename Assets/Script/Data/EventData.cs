using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EventData
{
    public enum TypeEnum
    {
        None = -1,
        Nothing,
        Recover, //回復 HP 和 MP
        Teleport, //傳送
        Money,
        Battle,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public string Comment_Chinese { get; set; }
        public string OptionChinese_1 { get; set; }
        public TypeEnum Result_1_1;
        public string RessutChinese_1_1;
        public TypeEnum Result_1_2;
        public string RessutChinese_1_2;
        public string OptionChinese_2 { get; set; }
        public TypeEnum Result_2_1;
        public string RessutChinese_2_1;

        public List<List<Result>> ResultList = new List<List<Result>>();
        public Dictionary<LanguageSystem.Language, string> CommentDic = new Dictionary<LanguageSystem.Language, string>();
        public List<Dictionary<LanguageSystem.Language, string>> OptionCommentList = new List<Dictionary<LanguageSystem.Language, string>>();

        public string GetComment()
        {
            return CommentDic[LanguageSystem.Instance.CurrentLanguage];
        }

        public string GetOptionComment(int index)
        {
            return OptionCommentList[index][LanguageSystem.Instance.CurrentLanguage];
        }

        public Result GetRandomResult(int index)
        {
            return ResultList[index][UnityEngine.Random.Range(0, ResultList.Count)];
        }
    }

    public class Result
    {
        public Result(TypeEnum type, string text) 
        {
            Type = type;
            TextDic.Add(LanguageSystem.Language.Chinese, text);
        }

        public TypeEnum Type;
        public Dictionary<LanguageSystem.Language, string> TextDic = new Dictionary<LanguageSystem.Language, string>();

        public string GetComment()
        {
            return TextDic[LanguageSystem.Instance.CurrentLanguage];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/Event.json";
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
            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_Chinese);

            //Option_1
            dataList[i].OptionCommentList.Add(new Dictionary<LanguageSystem.Language, string>());
            dataList[i].OptionCommentList[0].Add(LanguageSystem.Language.Chinese, dataList[i].OptionChinese_1);
            dataList[i].ResultList.Add(new List<Result>());
            if (dataList[i].Result_1_1 != TypeEnum.None)
            {
                dataList[i].ResultList[0].Add(new Result(dataList[i].Result_1_1, dataList[i].RessutChinese_1_1));
            }
            if (dataList[i].Result_1_2 != TypeEnum.None)
            {
                dataList[i].ResultList[0].Add(new Result(dataList[i].Result_1_2, dataList[i].RessutChinese_1_2));
            }

            //Option_2
            dataList[i].OptionCommentList.Add(new Dictionary<LanguageSystem.Language, string>());
            dataList[i].OptionCommentList[1].Add(LanguageSystem.Language.Chinese, dataList[i].OptionChinese_2);
            dataList[i].ResultList.Add(new List<Result>());
            if (dataList[i].Result_2_1 != TypeEnum.None)
            {
                dataList[i].ResultList[1].Add(new Result(dataList[i].Result_2_1, dataList[i].RessutChinese_2_1));
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

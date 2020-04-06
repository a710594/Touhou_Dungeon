using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EventOptionData
{
    public enum TypeEnum
    {
        None = -1,
        Nothing,
        RecoverHP,
        RecoverMP,
        RecoverBoth, //回復 HP 和 MP
        Teleport, //傳送
        Money,
        Battle,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public TypeEnum Type_1 { get; set; }
        public int Value_1 { get; set; }
        public string Comment_1_Chinese { get; set; }
        public TypeEnum Type_2 { get; set; }
        public int Value_2 { get; set; }
        public string Comment_2_Chinese { get; set; }

        public List<Result> ResultList = new List<Result>();

        public Result GetRandomResult()
        {
            return ResultList[UnityEngine.Random.Range(0, ResultList.Count)];
        }
    }

    public class Result
    {
        public TypeEnum Type;
        public int Value;
        public Dictionary<LanguageSystem.Language, string> CommentDic = new Dictionary<LanguageSystem.Language, string>();

        public string GetComment()
        {
            return CommentDic[LanguageSystem.Instance.CurrentLanguage];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/EventOption");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].Type_1 != TypeEnum.None)
            {
                dataList[i].ResultList.Add(new Result());
                dataList[i].ResultList[0].Type = dataList[i].Type_1;
                dataList[i].ResultList[0].Value = dataList[i].Value_1;
                dataList[i].ResultList[0].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_1_Chinese);

            }

            if (dataList[i].Type_2 != TypeEnum.None)
            {
                dataList[i].ResultList.Add(new Result());
                dataList[i].ResultList[1].Type = dataList[i].Type_2;
                dataList[i].ResultList[1].Value = dataList[i].Value_2;
                dataList[i].ResultList[1].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_2_Chinese);

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

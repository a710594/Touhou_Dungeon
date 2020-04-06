using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EventData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string Comment_Chinese { get; set; }
        public int OptionID_1 { get; set; }
        public string OptionChinese_1 { get; set; }
        public int OptionID_2 { get; set; }
        public string OptionChinese_2 { get; set; }
        public int OptionID_3 { get; set; }
        public string OptionChinese_3 { get; set; }

        public Dictionary<LanguageSystem.Language, string> CommentDic = new Dictionary<LanguageSystem.Language, string>();
        public List<int> OptionIdList = new List<int>();
        public List<Dictionary<LanguageSystem.Language, string>> OptionCommentList = new List<Dictionary<LanguageSystem.Language, string>>();

        public string GetComment()
        {
            return CommentDic[LanguageSystem.Instance.CurrentLanguage];
        }

        public string GetOptionComment(int index)
        {
            return OptionCommentList[index][LanguageSystem.Instance.CurrentLanguage];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Event");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_Chinese);

            if (dataList[i].OptionID_1 != 0)
            {
                dataList[i].OptionIdList.Add(dataList[i].OptionID_1);
                dataList[i].OptionCommentList.Add(new Dictionary<LanguageSystem.Language, string>());
                dataList[i].OptionCommentList[0].Add(LanguageSystem.Language.Chinese, dataList[i].OptionChinese_1);
            }

            if (dataList[i].OptionID_2 != 0)
            {
                dataList[i].OptionIdList.Add(dataList[i].OptionID_2);
                dataList[i].OptionCommentList.Add(new Dictionary<LanguageSystem.Language, string>());
                dataList[i].OptionCommentList[1].Add(LanguageSystem.Language.Chinese, dataList[i].OptionChinese_2);
            }

            if (dataList[i].OptionID_3 != 0)
            {
                dataList[i].OptionIdList.Add(dataList[i].OptionID_3);
                dataList[i].OptionCommentList.Add(new Dictionary<LanguageSystem.Language, string>());
                dataList[i].OptionCommentList[2].Add(LanguageSystem.Language.Chinese, dataList[i].OptionChinese_3);
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

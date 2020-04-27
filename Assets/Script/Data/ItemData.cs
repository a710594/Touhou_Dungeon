using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemData
{
    public enum TypeEnum
    {
        All,
        Equip,
        Medicine,
        Food,
        Material,
        Seed,
        GoHome,
    }

    public enum SeedTypeEnum
    {
        Vegetable = 1,
        Mashroom = 2,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public string Name_Chinese { get; set; }
        public string Comment_Chinese { get; set; }
        public string Icon { get; set; }
        public TypeEnum Type { get; set; }
        public SeedTypeEnum SeedType { get; set; }
        public int Price { get; set; }
        public int Volume { get; set; }
        public int Skill { get; set; }

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
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Item");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            dataList[i].NameDic.Add(LanguageSystem.Language.Chinese, dataList[i].Name_Chinese);

            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_Chinese);

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

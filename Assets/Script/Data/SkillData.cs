using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SkillData
{
    public enum TypeEnum
    {
        Idle = 0,
        Attack,
        Cure,
        Buff,
        Poison,
        Paralysis,
        Sleeping,
        ClearAbnormal,
        Field,
        CureItem,
        BuffItem,
    }

    public enum RangeTypeEnum
    {
        Point = 0,
        Circle,
        Rectangle
    }

    public enum TargetEnum
    {
        Us = 1,
        Them,
        All,
        None, //只能在空地上使用
    }

    public class RootObject
    {
        public int ID { get; set; }
        public string Icon { get; set; }
        public TypeEnum Type { get; set; }
        public int Damage { get; set; }
        public int Distance { get; set; }
        public int Range_1 { get; set; }
        public int Range_2 { get; set; }
        public RangeTypeEnum RangeType { get; set; }
        public TargetEnum Target { get; set; }
        public bool IsMagic { get; set; }
        public int MP { get; set; }
        public int CD { get; set; }
        public int StatusID { get; set; }
        public string ParticleName { get; set; }
        public int NeedPower { get; set; }
        public int SubID { get; set; }
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
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Skill");
        string jsonString = textAsset.text;
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

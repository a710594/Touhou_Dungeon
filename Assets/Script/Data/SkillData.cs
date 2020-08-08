using System.IO;
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
        Striking,
        CureLeastHP,
        Summon,
        Train,
    }

    public enum RangeTypeEnum
    {
        Point = 0,
        Circle,
        Rectangle
    }

    public enum TargetType
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
        public int Value_1 { get; set; }
        public int Value_2 { get; set; }
        public int Value_3 { get; set; }
        public int Value_4 { get; set; }
        public int Value_5 { get; set; }
        public int HitRate { get; set; }
        public int Distance { get; set; }
        public RangeTypeEnum RangeType { get; set; }
        public int Range_1 { get; set; }
        public int Range_2 { get; set; }
        public TargetType Target { get; set; }
        public bool IsMagic { get; set; }
        public int MP { get; set; }
        public int CD { get; set; }
        public int Priority { get; set; }
        public int StatusID { get; set; }
        public int AddPower { get; set; }
        public int NeedPower { get; set; }
        public int Summon { get; set; }
        public int SubID { get; set; }
        public string ParticleName { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public List<int> ValueList = new List<int>();
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
        string path = Application.streamingAssetsPath + "/Skill.json";
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

            dataList[i].NameDic.Add(LanguageSystem.Language.Chinese, dataList[i].Name);

            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment);

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

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemData
{
    public enum TypeEnum
    {
        All,
        Material,
        Food,
        Medicine,
        Equip,
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
        public bool CanCook;
        public int Price { get; set; }
        public int Volume { get; set; }
        public int Skill { get; set; }
        public int Material_1 { get; set; }
        public int Amount_1 { get; set; }
        public int Material_2 { get; set; }
        public int Amount_2 { get; set; }
        public int Material_3 { get; set; }
        public int Amount_3 { get; set; }

        public List<int> MaterialList = new List<int>();
        public List<int> AmountList = new List<int>();

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
        //TextAsset textAsset = Resources.Load<TextAsset>("Json/Item");
        //string jsonString = textAsset.text;
        string path = Application.streamingAssetsPath + "/Item.json";
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

            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_Chinese);

            if (dataList[i].Material_1 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_1);
                dataList[i].AmountList.Add(dataList[i].Amount_1);
            }
            if (dataList[i].Material_2 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_2);
                dataList[i].AmountList.Add(dataList[i].Amount_2);
            }
            if (dataList[i].Material_3 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_3);
                dataList[i].AmountList.Add(dataList[i].Amount_3);
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

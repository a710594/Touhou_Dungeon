using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Skill
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
        Line
    }

    public enum TargetEnum
    {
        Us = 1,
        Them,
        All,
        None, //只能在空地上使用
    }

    public class Data
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public TypeEnum Type { get; set; }
        public int Damage { get; set; }
        public int Hits { get; set; }
        public int Distance { get; set; }
        public int Range { get; set; }
        public RangeTypeEnum RangeType { get; set; }
        public TargetEnum Target { get; set; }
        public int IsMagic { get; set; }
        public int Priority { get; set; }
        public int MP { get; set; }
        public int CD { get; set; }
        public int StatusID { get; set; }
        public string ParticleName { get; set; }
        public int AddEP { get; set; }
        public int NeedEP { get; set; }
        public string Comment { get; set; }
        public int SubID { get; set; }
    }

    private static Dictionary<int, Data> _dataDic = new Dictionary<int, Data>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Skill");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<Data>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static Data GetData(int id)
    {
        Data data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }
}

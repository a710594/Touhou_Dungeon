using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Job
{
    public class Data
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string MediumImage { get; set; }
        public string LargeImage { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MTK { get; set; }
        public int MEF { get; set; }
        public int AGI { get; set; }
        public int SEN { get; set; }
        public int MoveDistance { get; set; }
        public int Skill_1 { get; set; }
        public int Skill_2 { get; set; }
        public int Skill_3 { get; set; }
        public int Skill_4 { get; set; }
        public int Skill_5 { get; set; }
        public int Skill_6 { get; set; }

        public List<int> SkillList = new List<int>();
    }

    private static Dictionary<int, Data> _dataDic = new Dictionary<int, Data>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Job");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<Data>>(jsonString);

        for (int i=0; i<dataList.Count; i++)
        {
            if(dataList[i].Skill_1 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_1);

            if (dataList[i].Skill_2 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_2);

            if (dataList[i].Skill_3 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_3);

            if (dataList[i].Skill_4 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_4);

            if (dataList[i].Skill_5 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_5);

            if (dataList[i].Skill_6 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_6);

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

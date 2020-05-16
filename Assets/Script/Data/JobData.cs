using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JobData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string Comment { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string MediumImage { get; set; }
        public string LargeImage { get; set; }
        public string Animator { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MTK { get; set; }
        public int MEF { get; set; }
        public int AGI { get; set; }
        public int SEN { get; set; }
        public int MOV { get; set; }
        public int Skill_1 { get; set; }
        public int UnlockLv_1 { get; set; }
        public int Skill_2 { get; set; }
        public int UnlockLv_2 { get; set; }
        public int Skill_3 { get; set; }
        public int UnlockLv_3 { get; set; }
        public int Skill_4 { get; set; }
        public int UnlockLv_4 { get; set; }
        public int Skill_5 { get; set; }
        public int UnlockLv_5 { get; set; }
        public int Skill_6 { get; set; }
        public int UnlockLv_6 { get; set; }
        public string Name_Chinese { get; set; }
        public string Name_English { get; set; }

        public Dictionary<int, int> SkillUnlockDic = new Dictionary<int, int>(); //skill ID, unlock lv
        public Dictionary<LanguageSystem.Language, string> NameDic = new Dictionary<LanguageSystem.Language, string>();

        public List<int> GetUnlockSkill(int lv)
        {
            List<int>  unlockList = new List<int>();

            foreach (KeyValuePair<int, int> item in SkillUnlockDic)
            {
                if (lv >= item.Value)
                {
                    unlockList.Add(item.Key);
                }
            }

            return unlockList;
        }

        public bool IsUnlockSkill(int skillId, int lv)
        {
            int unlockLv = SkillUnlockDic[skillId];
            return lv >= unlockLv;
        }

        public string GetName()
        {
            return NameDic[LanguageSystem.Instance.CurrentLanguage];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Job");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i=0; i<dataList.Count; i++)
        {
            if (dataList[i].Skill_1 != 0)
            {
                dataList[i].SkillUnlockDic.Add(dataList[i].Skill_1, dataList[i].UnlockLv_1);
            }

            if (dataList[i].Skill_2 != 0)
            {
                dataList[i].SkillUnlockDic.Add(dataList[i].Skill_2, dataList[i].UnlockLv_2);
            }

            if (dataList[i].Skill_3 != 0)
            {
                dataList[i].SkillUnlockDic.Add(dataList[i].Skill_3, dataList[i].UnlockLv_3);
            }

            if (dataList[i].Skill_4 != 0)
            {
                dataList[i].SkillUnlockDic.Add(dataList[i].Skill_4, dataList[i].UnlockLv_4);
            }

            if (dataList[i].Skill_5 != 0)
            {
                dataList[i].SkillUnlockDic.Add(dataList[i].Skill_5, dataList[i].UnlockLv_5);
            }

            if (dataList[i].Skill_6 != 0)
            {
                dataList[i].SkillUnlockDic.Add(dataList[i].Skill_6, dataList[i].UnlockLv_6);
            }

            dataList[i].NameDic.Add(LanguageSystem.Language.Chinese, dataList[i].Name_Chinese);
            dataList[i].NameDic.Add(LanguageSystem.Language.English, dataList[i].Name_English);

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

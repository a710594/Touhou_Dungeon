using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSkillScrollItem : ScrollItem
{
    public class Data
    {
        public JobData.RootObject JobData;
        public SkillData.RootObject SkillData;
        public int CharacterLv;

        public Data(JobData.RootObject jobData, SkillData.RootObject skillData, int characterLv)
        {
            JobData = jobData;
            SkillData = skillData;
            CharacterLv = characterLv;
        }
    }

    public Text NameLabel;
    public Text UnlockLvLabel;
    //public Image Icon;
    public GameObject Mask;

    public override void SetData(object obj)
    {
        Data data = (Data)obj; //skillId, IsUnlock
        base.SetData(data.SkillData);
        NameLabel.text = data.SkillData.GetName();
        //Icon.overrideSprite = Resources.Load<Sprite>("Image/" + data.Icon);

        if (!data.JobData.IsUnlockSkill(data.SkillData.ID, data.CharacterLv))
        {
            Mask.SetActive(true);
            UnlockLvLabel.text = "Lv." + data.JobData.SkillUnlockDic[data.SkillData.ID] + "解鎖";
        }
        else
        {
            Mask.SetActive(false);
        }
    }
}

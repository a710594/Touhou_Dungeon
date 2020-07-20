using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSkillScrollItem : ScrollItem
{
    public class UnlockData
    {
        public JobData.RootObject JobData;
        public SkillData.RootObject SkillData;
        public int CharacterLv;

        public UnlockData(JobData.RootObject jobData, SkillData.RootObject skillData, int characterLv)
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
        /*if (obj is UnlockData)
        {
            UnlockData data = (UnlockData)obj; //skillId, IsUnlock
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
        else
        {
            SkillData.RootObject data = SkillData.GetData((int)obj);
            base.SetData(data);
            NameLabel.text = data.GetName();
            Mask.SetActive(false);
        }*/

        KeyValuePair<int, int> pair = (KeyValuePair<int, int>)obj;
        KeyValuePair<SkillData.RootObject, int> data = new KeyValuePair<SkillData.RootObject, int>(SkillData.GetData(pair.Key), pair.Value);
        base.SetData(data);
        NameLabel.text = data.Key.GetName();
        Mask.SetActive(false);
    }
}

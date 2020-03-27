using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSkillGroup : MonoBehaviour
{
    public Text NameLabel;
    public Text DamageLabel;
    public Text HitsLabel;
    public Text DistanceLabel;
    public Text RangeLabel;
    public Text RangeTypeLabel;
    public Text TargetLabel;
    public Text PriorityLabel;
    public Text MPLabel;
    public Text CDLabel;
    public Text AddEPLabel;
    public Text NeedEPLabel;
    public Text CommentLabel;
    public LoopScrollView SkillScrollView;

    public void SetData(TeamMember member)
    {
        Clear();

        List<KeyValuePair<int, int>> pairList = new List<KeyValuePair<int, int>>(); //skillId, characterLv
        List<int> skillList = member.Data.SkillList;
        for (int i = 0; i < skillList.Count; i++)
        {
            pairList.Add(new KeyValuePair<int, int>(skillList[i], member.Lv));
        }

        SkillScrollView.SetData(new ArrayList(pairList));
        SkillScrollView.AddClickHandler(SkillOnClick);
    }

    private void SkillOnClick(object obj)
    {
        SkillData.RootObject data = (SkillData.RootObject)obj;
        NameLabel.text = data.GetName();
        DamageLabel.text = "傷害：" + data.Damage;
        HitsLabel.text = "次數：" + data.Hits;
        DistanceLabel.text = "射程：" + data.Distance;
        RangeLabel.text = "範圍：" + data.Range_1;
        if (data.RangeType == SkillData.RangeTypeEnum.Point)
        {
            RangeTypeLabel.text = "範圍類型：點狀";
        }
        else if (data.RangeType == SkillData.RangeTypeEnum.Circle)
        {
            RangeTypeLabel.text = "範圍類型：擴散";
        }
        else if (data.RangeType == SkillData.RangeTypeEnum.Rectangle)
        {
            RangeTypeLabel.text = "範圍類型：貫通";
        }
        if (data.Target == SkillData.TargetEnum.Us)
        {
            TargetLabel.text = "目標：我方";
        }
        else if (data.Target == SkillData.TargetEnum.Them)
        {
            TargetLabel.text = "目標：敵方";
        }
        else if (data.Target == SkillData.TargetEnum.All)
        {
            TargetLabel.text = "目標：全體";
        }
        MPLabel.text = "消耗：" + data.MP;
        CDLabel.text = "冷卻：" + data.CD;
        CommentLabel.text = data.GetComment();
    }

    private void Clear()
    {
        NameLabel.text = string.Empty;
        DamageLabel.text = string.Empty;
        HitsLabel.text = string.Empty;
        DistanceLabel.text = string.Empty;
        RangeLabel.text = string.Empty;
        RangeTypeLabel.text = string.Empty;
        TargetLabel.text = string.Empty;
        PriorityLabel.text = string.Empty;
        MPLabel.text = string.Empty;
        CDLabel.text = string.Empty;
        AddEPLabel.text = string.Empty;
        NeedEPLabel.text = string.Empty;
        CommentLabel.text = string.Empty;
    }

    private void Awake()
    {
        Clear();
    }
}

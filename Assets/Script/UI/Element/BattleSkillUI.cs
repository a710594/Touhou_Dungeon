using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleSkillUI : MonoBehaviour
{
    public Text NameLabel;
    public Text CommentLabel;
    public Text MpLabel;
    public Text CdLabel;
    public Text DistanceLabel;
    public Text RangeLabel;

    public void SetData(SkillData.RootObject data)
    {
        NameLabel.text = data.Name;
        CommentLabel.text = data.Comment;
        MpLabel.text = "MP:" + data.MP.ToString();
        CdLabel.text = "CD:" + data.CD.ToString();
        DistanceLabel.text = "射程:" + data.Distance.ToString();
        RangeLabel.text = "範圍:" + data.Range_1.ToString();
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}

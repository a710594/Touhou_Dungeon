using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleSkillGroup : MonoBehaviour
{
    public Text CommentLabel;
    public Text MpLabel;
    public Text CdLabel;
    public Text NeedEpLabel;
    public Text AddEpLabel;
    public Text DistanceLabel;
    public Text RangeLabel;
    public Text PriorityLabel;

    public void SetData(SkillData.RootObject data)
    {
        CommentLabel.text = data.Comment;
        MpLabel.text = "MP:" + data.MP.ToString();
        CdLabel.text = "CD:" + data.CD.ToString();
        DistanceLabel.text = "射程:" + data.Distance.ToString();
        RangeLabel.text = "範圍:" + data.Range.ToString();
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}

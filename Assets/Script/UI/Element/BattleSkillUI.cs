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

    public void SetData(SkillData.RootObject data)
    {
        NameLabel.text = data.GetName();
        CommentLabel.text = data.GetComment();
        MpLabel.text = "MP:" + data.MP.ToString();
        CdLabel.text = "CD:" + data.CD.ToString();
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleSkillUI : MonoBehaviour
{
    public Text NameLabel;
    public Text LvLabel;
    public Text DamageLabel;
    public Text TargetLabel;
    public Text PriorityLabel;
    public Text MPLabel;
    public Text CDLabel;
    public Text AddPowerLabel;
    public Text NeedPowerLabel;
    public Text CommentLabel;
    public Text BattleStatusTitlle;
    public Text[] BattleStatusComment;

    public void SetData(SkillData.RootObject data, int lv)
    {
        NameLabel.text = data.GetName();
        LvLabel.text = "Lv." + lv;
        MPLabel.text = "MP：" + data.MP;
        CDLabel.text = "冷卻：" + data.CD;
        AddPowerLabel.text = "增加 Power：" + data.AddPower;
        NeedPowerLabel.text = "需要 Power：" + data.NeedPower;
        CommentLabel.text = data.GetComment();

        if (data.ValueList.Count > 0)
        {
            if (data.Type == SkillData.TypeEnum.Attack)
            {
                DamageLabel.text = "傷害：" + data.ValueList[lv - 1];
            }
            else
            {
                DamageLabel.text = "回復：" + data.ValueList[lv - 1];
            }
        }
        else
        {
            DamageLabel.text = "傷害：" + 0;
        }
        if (data.Target == SkillData.TargetType.Us)
        {
            TargetLabel.text = "目標：我方";
        }
        else if (data.Target == SkillData.TargetType.Them)
        {
            TargetLabel.text = "目標：敵方";
        }
        else if (data.Target == SkillData.TargetType.All)
        {
            TargetLabel.text = "目標：全體";
        }

        BattleStatusTitlle.gameObject.SetActive(true);
        for (int i = 0; i < BattleStatusComment.Length; i++)
        {
            if (data != null)
            {
                if (data.StatusID != 0)
                {
                    BattleStatusData.RootObject statusData = BattleStatusData.GetData(data.StatusID);
                    if ((int)statusData.ValueType <= 7)
                    {
                        BattleStatusComment[i].gameObject.SetActive(true);
                        BattleStatusComment[i].text = statusData.GetComment(lv);
                    }
                    else
                    {
                        BattleStatusComment[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    BattleStatusComment[i].gameObject.SetActive(false);
                }

                if (data.SubID != 0)
                {
                    data = SkillData.GetData(data.SubID);
                }
                else
                {
                    data = null;
                }
            }
            else
            {
                BattleStatusComment[i].gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }
}

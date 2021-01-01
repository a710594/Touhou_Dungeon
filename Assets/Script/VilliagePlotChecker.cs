using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilliagePlotChecker : MonoBehaviour
{
    //放在 villiage scene 中,檢查 plot 用

    void Start()
    {
        if (ProgressManager.Instance.Memo.BOSS_1_Flag && !ProgressManager.Instance.Memo.Stage_2_Flag)
        {
            Plot_7 plot_7 = new Plot_7();
            plot_7.Start();
        }
        else if (ProgressManager.Instance.Memo.BOSS_2_Flag && !ProgressManager.Instance.Memo.Stage_3_Flag)
        {
            Plot_12 plot_12 = new Plot_12();
            plot_12.Start();
        }
    }
}

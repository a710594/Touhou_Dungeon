using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorePlotChecker
{
    //由 ExploreController 呼叫,檢查 plot 用

    public void Check()
    {
        if (ExploreController.Instance.CurrentFloor == 7 &&  ProgressManager.Instance.Memo.Stage_2_Flag && !ProgressManager.Instance.Memo.Floor7_Flag)
        {
            Plot_8 plot_8 = new Plot_8();
            plot_8.Start();
        }
        else if (ExploreController.Instance.CurrentFloor == 13 && ProgressManager.Instance.Memo.Stage_3_Flag && !ProgressManager.Instance.Memo.Floor13_Flag)
        {
            Plot_13 plot_13 = new Plot_13();
            plot_13.Start();
        }
    }
}

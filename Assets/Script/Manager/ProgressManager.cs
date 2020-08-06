using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager //管理劇情進度
{
    private static ProgressManager _instance;
    public static ProgressManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProgressManager();
            }
            return _instance;
        }
    }

    public ProgressMemo Memo;

    public void Init() 
    {
        ProgressMemo memo = Caretaker.Instance.Load<ProgressMemo>();
        if (memo == null)
        {
            Memo = new ProgressMemo();
            for (int i=0; i<3; i++)
            {
                Memo.FlagList.Add(false);
            }
        }
        else
        {
            Memo = memo;
        }
    }

    public void Save() 
    {
        Caretaker.Instance.Save<ProgressMemo>(Memo);
    }

    public void SetFlag(int index, bool flag)
    {
        Memo.FlagList[index] = flag;
    }
}

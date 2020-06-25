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
            Memo.FlagList.Add(new KeyValuePair<bool, int>(false, 1001));
            Memo.FlagList.Add(new KeyValuePair<bool, int>(false, 2001));
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
}

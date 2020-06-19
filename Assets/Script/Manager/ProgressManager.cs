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
            Memo.FirstConversation = new KeyValuePair<bool, int>(false, 1);
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

    public void SetFlag(KeyValuePair<bool, int> pair, bool boo) 
    {
        pair = new KeyValuePair<bool, int>(boo, pair.Value);
    }
}

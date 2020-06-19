using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstConversation : MonoBehaviour //遊戲的第一個事件
{
    // Start is called before the first frame update
    void Awake()
    {
        KeyValuePair<bool, int> flag = ProgressManager.Instance.Memo.FirstConversation;
        if (!flag.Key)
        {
            ConversationUI.Open(flag.Value, () =>
            {
                flag = new KeyValuePair<bool, int>(false, flag.Value);
                ProgressManager.Instance.Memo.FirstConversation = flag;
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uuz : MonoBehaviour
{
    //放在 Explore_BOSS_1 場景的八雲紫身上的小小 Script

    void Start()
    {
        gameObject.SetActive(!ProgressManager.Instance.Memo.BOSS_2_Flag);
    }
}

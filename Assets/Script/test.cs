using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    private void Start()
    {
        ConversationData.Load();
        ConversationUI.Open(1001, false);
    }
}

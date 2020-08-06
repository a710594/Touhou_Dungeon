using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ConversationData.Load();
        ConversationUI.Open(1, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testLoopScrollView : MonoBehaviour
{
    public LoopScrollView ScrollView;

    // Start is called before the first frame update
    void Start()
    {
        List<string> list = new List<string>();
        for (int i=0; i<20; i++)
        {
            list.Add(i.ToString());
        }
        ScrollView.SetData(new ArrayList(list));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScrollView.SetFirstElement("17");
        }
    }
}

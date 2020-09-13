using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testLoopScrollView : MonoBehaviour
{
    public LoopScrollView ScrollView;

    private List<string> NumberList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<20; i++)
        {
            NumberList.Add(i.ToString());
        }
        ScrollView.SetData(new ArrayList(NumberList));
        ScrollView.AddClickHandler(ButtonOnClick);
    }

    private void ButtonOnClick(object obj)
    {
        NumberList.Remove((string)obj);
        ScrollView.Refresh(new ArrayList(NumberList));
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

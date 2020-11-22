using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorScrollItem : ScrollItem
{
    public Text Label;

    public override void SetData(object obj)
    {
        if (obj is DungeonGroupData.RootObject)
        {
            DungeonGroupData.RootObject data = (DungeonGroupData.RootObject)obj;
            base.SetData(data);
            Label.text = data.Name;
        }
        else if (obj is DungeonData.RootObject)
        {
            DungeonData.RootObject data = (DungeonData.RootObject)obj;
            base.SetData(data);
            Label.text = data.FloorName;
        }
    }
}

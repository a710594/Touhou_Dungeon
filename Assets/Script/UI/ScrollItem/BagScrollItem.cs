using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagScrollItem : ScrollItem
{
    public IconCard Card;

    public override void SetData(object obj)
    {
        if (obj is Item)
        {
            Item item = (Item)obj;
            if (item.Type == ItemData.TypeEnum.Equip)
            {
                Equip equip = (Equip)item;
                base.SetData(equip);
                Card.Init(equip);
            }
            else
            {
                base.SetData(item);
                Card.Init(item.ID, item.Amount);
            }
        }
        else if(obj is KeyValuePair<int, int>)
        {
            KeyValuePair<int, int> pair = (KeyValuePair<int, int>)obj;
            base.SetData(pair.Key);
            Card.Init(pair.Key, pair.Value);
        }
    }
}

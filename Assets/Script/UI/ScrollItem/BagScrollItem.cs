using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagScrollItem : ScrollItem
{
    public IconCard Card;

    public override void SetData(object obj)
    {
        if (obj is KeyValuePair<object, int>)
        {
            KeyValuePair<object, int> pair = (KeyValuePair<object, int>)obj;
            base.SetData(pair.Key);

            if (pair.Key is int)
            {
                int id = (int)pair.Key;
                int amount = pair.Value;

                if (amount != 0)
                {
                    Card.Init(id, amount);
                }
                else
                {
                    Card.Init(id);
                }
            }
            else if (pair.Key is Equip)
            {
                Card.Init((Equip)pair.Key);
            }
        }
        else
        {
            Equip equip = (Equip)obj;
            base.SetData(equip);
            Card.Init(equip);
        }
    }
}

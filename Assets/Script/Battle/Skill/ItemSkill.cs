using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkill : Skill
{
    public int ItemID;
    public int ItemAmount
    {
        get
        {
            return ItemManager.Instance.GetItemAmount(ItemID, ItemManager.Type.Bag);
        }
    }

    public override void Use(BattleCharacter executor, Action callback)
    {
        base.Use(executor, callback);
        ItemManager.Instance.MinusItem(ItemID, 1, ItemManager.Type.Bag);
    }
}

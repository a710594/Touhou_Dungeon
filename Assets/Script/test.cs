using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    private void Start()
    {
        NewCookData.Load();
        ItemData.Load();
        ItemEffectData.Load();

        ItemManager.Instance.Init();
        ItemManager.Instance.AddItem(1001, 3, ItemManager.Type.Warehouse);
        ItemManager.Instance.AddItem(1006, 3, ItemManager.Type.Warehouse);
        ItemManager.Instance.AddItem(21001, 3, ItemManager.Type.Warehouse);
        ItemManager.Instance.AddItem(21002, 3, ItemManager.Type.Warehouse);
        ItemManager.Instance.AddItem(21003, 3, ItemManager.Type.Warehouse);
        ItemManager.Instance.AddItem(21004, 3, ItemManager.Type.Warehouse);

        NewCookUI.Open(ItemManager.Type.Warehouse);
    }
}

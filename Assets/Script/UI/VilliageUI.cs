using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VilliageUI : MonoBehaviour
{
    public Button ShopButton;
    public Button CookButton;
    public Button TeamButton;
    public Button FormationButton;
    public Button WarehouseButton;

    private void CookOnClick() 
    {
        CookUI.Open(ItemManager.Type.Warehouse);
    }

    private void TeamOnClick() 
    {
        TeamUI.Open();
    }

    private void FormationOnClick() 
    {
        FormationUI.Open();
    }

    private void WarehouseOnClick() 
    {
        BagUI.Open(ItemManager.Type.Warehouse);
    }

    private void Awake()
    {
        CookButton.onClick.AddListener(CookOnClick);
        TeamButton.onClick.AddListener(TeamOnClick);
        FormationButton.onClick.AddListener(FormationOnClick);
        WarehouseButton.onClick.AddListener(WarehouseOnClick);
    }
}

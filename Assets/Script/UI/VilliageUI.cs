using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VilliageUI : MonoBehaviour
{
    public Button AdventureButton;
    public Button ShopButton;
    public Button CookButton;
    public Button TeamButton;
    public Button FormationButton;
    public Button WarehouseButton;
    public SelectDestinationUI SelectDestinationUI;

    private void AdventureOnClick()
    {
        SelectDestinationUI.Open();
    }

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
        SelectDestinationUI.gameObject.SetActive(false);

        AdventureButton.onClick.AddListener(AdventureOnClick);
        CookButton.onClick.AddListener(CookOnClick);
        TeamButton.onClick.AddListener(TeamOnClick);
        FormationButton.onClick.AddListener(FormationOnClick);
        WarehouseButton.onClick.AddListener(WarehouseOnClick);
    }
}

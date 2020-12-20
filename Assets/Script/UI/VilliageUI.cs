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
    public Button SaveButton;
    public Button BuyButton;
    public Button SellButton;
    public Button EquipUpgradeButton;
    public Button CloseShopGroupButton;
    public GameObject MainGroup;
    public GameObject ShopGroup;
    public TipLabel TipLabel;
    public SelectDestinationUI SelectDestinationUI;

    private void AdventureOnClick()
    {
        SelectDestinationUI.Open();
    }

    private void ShopOnClick()
    {
        ShopGroup.SetActive(true);
        MainGroup.SetActive(false);
    }

    private void CookOnClick() 
    {
        NewCookUI.Open(ItemManager.Type.Warehouse);
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

    private void SaveOnClick()
    {
        GameSystem.Instance.SaveGame();
        TipLabel.SetLabel("存檔成功");
    }

    private void BuyOnClick()
    {
        ShopUI.Open();
    }

    private void SellOnClick()
    {
        SellUI.Open();
    }

    private void EquipUpgradeOnClick()
    {
        EquipUpgradeUI.Open(ItemManager.Type.Warehouse);
    }

    private void CloseShopGroupOnClick()
    {
        ShopGroup.SetActive(false);
        MainGroup.SetActive(true);
    }

    private void Awake()
    {
        ShopGroup.SetActive(false);
        SelectDestinationUI.gameObject.SetActive(false);

        AdventureButton.onClick.AddListener(AdventureOnClick);
        ShopButton.onClick.AddListener(ShopOnClick);
        CookButton.onClick.AddListener(CookOnClick);
        TeamButton.onClick.AddListener(TeamOnClick);
        FormationButton.onClick.AddListener(FormationOnClick);
        WarehouseButton.onClick.AddListener(WarehouseOnClick);
        SaveButton.onClick.AddListener(SaveOnClick);
        BuyButton.onClick.AddListener(BuyOnClick);
        SellButton.onClick.AddListener(SellOnClick);
        EquipUpgradeButton.onClick.AddListener(EquipUpgradeOnClick);
        CloseShopGroupButton.onClick.AddListener(CloseShopGroupOnClick);

        AudioSystem.Instance.Stop();
        AudioSystem.Instance.Play("Jinja", true);
    }
}

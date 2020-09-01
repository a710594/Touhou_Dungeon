﻿using System.Collections;
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
    public Button CloseShopGroupButton;
    public GameObject MainGroup;
    public GameObject ShopGroup;
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

    private void SaveOnClick()
    {
        GameSystem.Instance.SaveGame();
    }

    private void BuyOnClick()
    {
        ShopUI.Open();
    }

    private void SellOnClick()
    {
        SellUI.Open();
    }

    private void CloseShopGroupOnClick()
    {
        ShopGroup.SetActive(false);
        MainGroup.SetActive(true);
    }

    private void Awake()
    {
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
        CloseShopGroupButton.onClick.AddListener(CloseShopGroupOnClick);

        AudioSystem.Instance.Stop();
        AudioSystem.Instance.Play("Jinja", true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public Button MoveActionButton;
    public Button SkillActionButton;
    public Button ItemActionButton;
    public Button MoveConfirmButton;
    public Button ReturnActionButton; //返回選擇行動
    public ButtonPlus Screen; //偵測整個畫面的點擊事件
    public GameObject ActionGroup;

    public static void Open()
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BattleUI", ResourceManager.Type.UI).GetComponent<BattleUI>();
        }
    }

    public static void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;
    }

    public void SetActionGroupVisible(bool isVisible)
    {
        ActionGroup.SetActive(isVisible);
    }

    public void SetMoveConfirmVisible(bool isVisible)
    {
        MoveConfirmButton.gameObject.SetActive(isVisible);
    }

    public void SetReturnActionVisible(bool isVisible)
    {
        ReturnActionButton.gameObject.SetActive(isVisible);
    }

    private void MoveActionOnClick()
    {
        BattleController.Instance.ChangeToMoveState();
        SetActionGroupVisible(false);
    }

    private void SkillActionOnClick()
    {
    }

    private void ItemActionOnClick()
    {
    }

    private void MoveConfirmOnClick()
    {
        BattleController.Instance.MoveConfirm();
    }

    private void ReturnActionOnClick()
    {
    }

    private void ScreenOnClick(object data)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int intPosition = Vector2Int.RoundToInt(worldPosition);
        intPosition.y -= 1; //地圖座標的偏移值
        BattleController.Instance.ScreenOnClick(intPosition);
    }

    private void Awake()
    {
        SetActionGroupVisible(false);
        SetMoveConfirmVisible(false);
        SetReturnActionVisible(false);

        MoveActionButton.onClick.AddListener(MoveActionOnClick);
        SkillActionButton.onClick.AddListener(SkillActionOnClick);
        ItemActionButton.onClick.AddListener(ItemActionOnClick);
        MoveConfirmButton.onClick.AddListener(MoveConfirmOnClick);
        ReturnActionButton.onClick.AddListener(ReturnActionOnClick);
        Screen.ClickHandler = ScreenOnClick;
    }
}

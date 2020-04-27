using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUI : MonoBehaviour
{
    public Text ConfirmLabel;
    public Text CancelLabel;
    public Text CommentLabel;
    public Button ConfirmButton;
    public Button CancelButton;

    private Action _onConfirmHandler;
    private Action _onCancelHandler;

    private static ConfirmUI _instance;

    public static void Open(string commentText, string confirmText, Action confirmCallback)
    {
        if (_instance == null)
        {
            _instance = ResourceManager.Instance.Spawn("ConfirmUI", ResourceManager.Type.UI).GetComponent<ConfirmUI>();
        }
        _instance.Init(commentText, confirmText, confirmCallback);
    }

    public static void Open(string commentText, string confirmText, string cancelText, Action confirmCallback, Action cancelCallback)
    {
        if (_instance == null)
        {
            _instance = ResourceManager.Instance.Spawn("ConfirmUI", ResourceManager.Type.UI).GetComponent<ConfirmUI>();
        }
        _instance.Init(commentText, confirmText, cancelText, confirmCallback, cancelCallback);
    }

    private void Close()
    {
        Destroy(_instance.gameObject);
        _instance = null;
    }

    private void Init(string commentText, string confirmText, Action confirmCallback)
    {
        CommentLabel.text = commentText;
        ConfirmLabel.text = confirmText;
        CancelButton.gameObject.SetActive(false);
        _onConfirmHandler = confirmCallback;
    }

    private void Init(string commentText, string confirmText, string cancelText, Action confirmCallback, Action cancelCallback)
    {
        CommentLabel.text = commentText;
        ConfirmLabel.text = confirmText;
        CancelLabel.text = cancelText;
        _onConfirmHandler = confirmCallback;
        _onCancelHandler = cancelCallback;
    }

    private void ConfirmOnClick()
    {
        _instance.Close();
        if (_onConfirmHandler != null)
        {
            _onConfirmHandler();
            _onConfirmHandler = null;
        }
    }

    private void CancelOnClick()
    {
        _instance.Close();
    }

    void Awake()
    {
        ConfirmButton.onClick.AddListener(ConfirmOnClick);
        CancelButton.onClick.AddListener(CancelOnClick);
    }
}

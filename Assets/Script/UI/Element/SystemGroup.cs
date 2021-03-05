using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemGroup : MonoBehaviour
{
    public Button SaveButton;
    public Button QuitButton;
    public Button CloseButton;
    public TipLabel TipLabel;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Save()
    {
        GameSystem.Instance.SaveGame();
        TipLabel.SetLabel("存檔成功");
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Awake()
    {
        SaveButton.onClick.AddListener(Save);
        QuitButton.onClick.AddListener(Quit);
        CloseButton.onClick.AddListener(Close);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatusUI : MonoBehaviour
{
    public LoopScrollView ScrollView;
    public Button CloseButton;

    public void Open(List<BattleStatus> statusList)
    {
        gameObject.SetActive(true);
        ScrollView.SetData(new ArrayList(statusList));
    }

    private void Close() 
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        CloseButton.onClick.AddListener(Close);
    }
}

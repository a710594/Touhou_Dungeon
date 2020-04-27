using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    public static EventUI Instance;

    public Text CommentLabel;
    public Button CloseButton;
    public ButtonPlus[] OptionButtons;

    private Action FinishCallback;

    public static void Open(int id, Action callback)
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("EventUI", ResourceManager.Type.UI).GetComponent<EventUI>();
        }
        Instance.SetData(id, callback);
        Time.timeScale = 0;
    }

    public void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;
        Time.timeScale = 1;

        if (FinishCallback != null)
        {
            FinishCallback();
        }
    }

    public void SetData(int id, Action callback)
    {
        EventData.RootObject data = EventData.GetData(id);
        CommentLabel.text = data.GetComment();
        FinishCallback = callback;

        for (int i=0; i<OptionButtons.Length; i++)
        {
            if (i < data.OptionIdList.Count)
            {
                OptionButtons[i].gameObject.SetActive(true);
                OptionButtons[i].SetData(data.OptionIdList[i]);
                OptionButtons[i].Label.text = data.GetOptionComment(i);
            }
            else
            {
                OptionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OptionOnCLick(ButtonPlus buttonPlus)
    {
        EventOptionData.RootObject data = EventOptionData.GetData((int)buttonPlus.Data);
        EventOptionData.Result result = data.GetRandomResult();
        EventResult eventResult = EventResultFactory.GetNewEventResult(result);
        eventResult.Execute();
        CommentLabel.text = result.GetComment();

        for (int i=0; i<OptionButtons.Length; i++)
        {
            OptionButtons[i].gameObject.SetActive(false);
        }
    }

    private void CloseOnClick()
    {
        Close();
    }

    private void Awake()
    {
        CloseButton.onClick.AddListener(CloseOnClick);

        for (int i=0; i<OptionButtons.Length; i++)
        {
            OptionButtons[i].ClickHandler = OptionOnCLick;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    public static EventUI Instance;

    public Text CommentLabel;
    public Button ConfirmButton;
    public ButtonPlus[] OptionButtons;

    private bool _isDoNothing = false;
    private Action<bool> FinishCallback;

    public static void Open(int id, Action<bool> callback)
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
            FinishCallback(_isDoNothing);
        }
    }

    public void SetData(int id, Action<bool> callback)
    {
        EventData.RootObject data = EventData.GetData(id);
        CommentLabel.text = data.GetComment();
        FinishCallback = callback;

        for (int i=0; i<OptionButtons.Length; i++)
        {
            if (i < data.OptionCommentList.Count)
            {
                OptionButtons[i].gameObject.SetActive(true);
                OptionButtons[i].SetData(data.ResultList[i]);
                string text = data.GetOptionComment(i);
                if (text.Contains("{10%}"))
                {
                    text = text.Replace("{10%}", ((int)(ItemManager.Instance.Money * 0.1f)).ToString());
                }
                OptionButtons[i].Label.text = text;
            }
            else
            {
                OptionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OptionOnCLick(ButtonPlus buttonPlus)
    {
        List<EventData.Result> resultList = (List<EventData.Result>)buttonPlus.Data;
        EventData.Result result = resultList[UnityEngine.Random.Range(0, resultList.Count)];
        EventResult eventResult = EventResultFactory.GetNewEventResult(result);
        eventResult.Execute();
        string comment = result.GetComment();
        if (comment.Contains("{}"))
        {
            comment = comment.Replace("{}", (ExploreController.Instance.ArriveFloor * 100).ToString());
        }
        CommentLabel.text = comment;

        if (result.Type == EventData.TypeEnum.Nothing)
        {
            _isDoNothing = true;
        }
        else
        {
            _isDoNothing = false;
        }

        for (int i=0; i<OptionButtons.Length; i++)
        {
            OptionButtons[i].gameObject.SetActive(false);
        }
        ConfirmButton.gameObject.SetActive(true);
    }

    private void ConfirmOnClick()
    {
        Close();
    }

    private void Awake()
    {
        ConfirmButton.onClick.AddListener(ConfirmOnClick);

        for (int i=0; i<OptionButtons.Length; i++)
        {
            OptionButtons[i].ClickHandler = OptionOnCLick;
        }
    }
}

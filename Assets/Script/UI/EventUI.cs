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
    private Event _event;
    private Action<bool> FinishCallback;

    public static void Open(Event @event , Action<bool> callback)
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("EventUI", ResourceManager.Type.UI).GetComponent<EventUI>();
        }
        Instance.SetData(@event, callback);
        ExploreController.Instance.StopEnemy();
    }

    public void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;

        if (FinishCallback != null)
        {
            FinishCallback(_isDoNothing);
        }
    }

    public void SetData(Event @event, Action<bool> callback)
    {
        _event = @event;
        CommentLabel.text = _event.Comment;
        FinishCallback = callback;

        for (int i=0; i<OptionButtons.Length; i++)
        {
            if (i < _event.Options.Count)
            {
                OptionButtons[i].gameObject.SetActive(true);
                OptionButtons[i].SetData(i);
                OptionButtons[i].Label.text = _event.Options[i];
            }
            else
            {
                OptionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OptionOnCLick(ButtonPlus buttonPlus)
    {
        Event.Result result = _event.GetResult((int)buttonPlus.Data);
        result.Execute();
        CommentLabel.text = result.Comment;
        _isDoNothing = result.isDoNothing;

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

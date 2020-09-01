using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConversationUI : MonoBehaviour
{
    public static ConversationUI Instance;

    public Action<int> Handler;

    public Text NameLabel;
    public Typewriter Typewriter;
    public Button NextButton;
    public Button SkipButton;
    public Image Background;
    public Image FadeImage;
    public Image[] CharacterImage;

    private int _siblingIndex;
    private bool _isPlayingBGM = false;
    private bool _isClickable = true;
    private bool _isFadeEnd = true; //結束時淡出
    private ConversationData.RootObject _data;
    private Timer _timer = new Timer();
    private Action _onFinishHandler;
    private List<string> _conversationList = new List<string>();

    public static void Open(int id, bool isFadeEnd, Action callback = null)
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("ConversationUI", ResourceManager.Type.UI).GetComponent<ConversationUI>();
        }
        Instance.Init(id, isFadeEnd, callback);
    }

    public static void Close()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    private void Init(int id, bool isFadeEnd, Action callback = null)
    {
        ConversationData.RootObject data = ConversationData.GetData(id);
        Typewriter.ClearText();
        SetData(data);

        _data = data;
        _isFadeEnd = isFadeEnd;
        _onFinishHandler = callback;
        //_isFinal = data.IsFinal;
        //_onFinishHandler += Close;
    }

    private void NextConversationID(int nextId)
    {
        if (nextId == 0)
        {
            Finish();

            return;
        }

        ConversationData.RootObject data = ConversationData.GetData(nextId);
        _data = data;
        SetData(data);
    }

    private void SetData(ConversationData.RootObject data)
    {
        NameLabel.text = data.Name;
        Typewriter.Show(data.GetComment());

        if (data.Background == "x")
        {
            Background.gameObject.SetActive(false);
        }
        else if (data.Background != "-")
        {
            Background.gameObject.SetActive(true);
            Background.sprite = Resources.Load<Sprite>("Image/Background/" + data.Background);
        }

        if (data.BGM == "x")
        {
            _isPlayingBGM = false;
        }
        else if(data.BGM != "-")
        {
            _isPlayingBGM = true;
            AudioSystem.Instance.Play(data.BGM, true);
        }

        for (int i=0; i<data.Images.Length; i++)
        {
            if (data.Images[i] == "x")
            {
                CharacterImage[i].gameObject.SetActive(false);
            }
            else if (data.Images[i] == "-")
            {
                CharacterImage[i].color = Color.gray;
                CharacterImage[i].transform.SetSiblingIndex(_siblingIndex);
            }
            else
            {
                CharacterImage[i].gameObject.SetActive(true);
                CharacterImage[i].color = Color.white;
                CharacterImage[i].transform.SetSiblingIndex(_siblingIndex + 1);
                if (data.Images[i] != "o")
                {
                    CharacterImage[i].sprite = Resources.Load<Sprite>("Image/Character/Large/" + data.Images[i]);
                    CharacterImage[i].SetNativeSize();
                }
            }
        }

        for (int i = 0; i < data.Motions.Length; i++)
        {
            if (data.Motions[i] == ConversationData.MotionEnum.Jump)
            {
                CharacterImage[i].transform.DOJump(CharacterImage[i].transform.position, 50, 1, 0.5f);
            }
            else if(data.Motions[i] == ConversationData.MotionEnum.Shake)
            {
                CharacterImage[i].transform.DOShakePosition(0.5f, 20);
            }
        }

        if (Handler != null)
        {
            Handler(data.ID);
        }
    }

    private void Finish()
    {
        if (_isPlayingBGM)
        {
            AudioSystem.Instance.Stop(true);
        }

        NameLabel.text = string.Empty;
        Typewriter.ClearText();

        _isClickable = false;
        if (_isFadeEnd)
        {
            FadeImage.DOFade(1, 1).OnComplete(() =>
            {
                if (_onFinishHandler != null)
                {
                    _onFinishHandler();
                }
                Close();
            });
        }
        else
        {
            if (_onFinishHandler != null)
            {
                _onFinishHandler();
            }
            Close();
        }
    }

    private void NextOnClick()
    {
        if (!_isClickable)
        {
            return;
        }

        if (!Typewriter.IsTyping)
        {
            NextConversationID(_data.NextID);
        }
        else
        {
            Typewriter.SetText();
        }
    }

    private void SkipOnClick()
    {
        Finish();
    }

    private void Awake()
    {
        _siblingIndex = CharacterImage[0].transform.GetSiblingIndex();

        for (int i=0; i<CharacterImage.Length; i++)
        {
            CharacterImage[i].gameObject.SetActive(false);
        }

        NextButton.onClick.AddListener(NextOnClick);
        SkipButton.onClick.AddListener(SkipOnClick);
    }
}
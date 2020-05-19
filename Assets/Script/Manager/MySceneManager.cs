using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager
{
    public enum SceneType
    {
        Villiage,
        Explore,
        Battle,
    }

    private static MySceneManager _instance;
    public static MySceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MySceneManager();
            }
            return _instance;
        }
    }

    public SceneType CurrentScene
    {
        get;
        protected set;
    }

    public Action BeforeSceneLoadedHandler;
    public Action AfterSceneLoadedHandler;

    private bool _isLock = false;
    private SceneType _tempType;

    public void Init()
    {
        CurrentScene = SceneType.Battle;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public void ChangeScene(SceneType type, Action callback = null)
    {
        if (!_isLock)
        {
            _isLock = true;
            _tempType = type;

            if (BeforeSceneLoadedHandler != null)
            {
                BeforeSceneLoadedHandler();
                BeforeSceneLoadedHandler = null;
            }

            //FadeImage.Instance.Fade(1, 1, () =>
            //{
            SceneManager.LoadSceneAsync((int)type);
            AfterSceneLoadedHandler += callback;
            //});
        }
    }

    public void ChangeSceneImmediately(SceneType type, Action callback = null)
    {
        if (!_isLock)
        {
            _isLock = true;
            _tempType = type;

            if (BeforeSceneLoadedHandler != null)
            {
                BeforeSceneLoadedHandler();
                BeforeSceneLoadedHandler = null;
            }

            SceneManager.LoadSceneAsync((int)type);
            AfterSceneLoadedHandler += callback;
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isLock = false;
        CurrentScene = _tempType;

        if (AfterSceneLoadedHandler != null)
        {
            AfterSceneLoadedHandler();
            AfterSceneLoadedHandler = null;
        }

        //FadeImage.Instance.Fade(0, 1);
    }
}

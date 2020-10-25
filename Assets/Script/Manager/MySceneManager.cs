using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager
{
    public enum SceneType
    {
        Logo,
        Villiage,
        Explore,
        Battle,
        FirstBattle,
        Explore_BOSS_1,
        Battle_BOSS_1,
        Explore_BOSS_2,
        Battle_BOSS_2,
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

    public SceneType LastScene
    {
        get;
        protected set;
    }

    public Action BeforeSceneLoadedHandler;
    public Action AfterSceneLoadedHandler;

    private bool _isLock = false;
    private SceneType _tempType;
    private SceneMemo _sceneMemo;

    public void Init()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public void Load()
    {
        _sceneMemo = Caretaker.Instance.Load<SceneMemo>();
        if (_sceneMemo != null)
        {
            CurrentScene = _sceneMemo.CurrentScene;
        }
        else
        {
            _sceneMemo = new SceneMemo();
        }
    }

    public void Save()
    {
        _sceneMemo.SetData(CurrentScene);
        Caretaker.Instance.Save<SceneMemo>(_sceneMemo);
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

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isLock = false;
        LastScene = CurrentScene;
        CurrentScene = _tempType;

        if (AfterSceneLoadedHandler != null)
        {
            AfterSceneLoadedHandler();
            AfterSceneLoadedHandler = null;
        }

        //FadeImage.Instance.Fade(0, 1);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSystem
{
    public enum Language
    {
        Chinese,
        English,
    }

    public Action<Language> LanguageChangeHandler;

    private static LanguageSystem _instance;
    public static LanguageSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LanguageSystem();
            }
            return _instance;
        }
    }

    private Language _currentLanguage = Language.Chinese;
    public Language CurrentLanguage
    {
        get
        {
            return _currentLanguage;
        }
    }

    public void ChangeLanguage(Language language)
    {
        _currentLanguage = language;
        if (LanguageChangeHandler != null)
        {
            LanguageChangeHandler(_currentLanguage);
        }
    }
}

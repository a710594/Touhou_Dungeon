using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
    public int ID;
    public Text Label;

    public void SetLanguage(LanguageSystem.Language language) 
    {
        Label.text = LanguageData.GetText(ID, language);
    }

    private void Awake()
    {
        SetLanguage(LanguageSystem.Instance.CurrentLanguage);
        LanguageSystem.Instance.LanguageChangeHandler += SetLanguage;
    }
}

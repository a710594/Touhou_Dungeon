using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
    public int ID;
    public Text Label;

    public void SetLanguage() 
    {
        Label.text = LanguageData.GetText(ID, GameSystem.CurrentLanguage);
    }

    private void Awake()
    {
        SetLanguage();
    }
}

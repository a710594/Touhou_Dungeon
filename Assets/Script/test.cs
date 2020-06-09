using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public InputField InputField;
    public Text Text;
    public Button LoadButton;
    public Button SaveButton;

    private TestMemento _testMemento = new TestMemento();

    private void Load()
    {
        _testMemento = Caretaker.Instance.Load<TestMemento>();
        Text.text = _testMemento.Text;

        foreach (KeyValuePair<string, int> item in _testMemento.Dic)
        {
            Debug.Log(Utility.StringToVector2Int(item.Key) + " " +  item.Value);
        }
    }

    private void Save()
    {
        _testMemento.Text = InputField.text;

        Dictionary<string, int> dic = new Dictionary<string, int>();
        dic.Add(Utility.Vector2IntToString(Vector2Int.up), 123);
        _testMemento.Dic = dic;

        Caretaker.Instance.Save<TestMemento>(_testMemento);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Caretaker.Instance.Init();
        LoadButton.onClick.AddListener(Load);
        SaveButton.onClick.AddListener(Save);
    }
}

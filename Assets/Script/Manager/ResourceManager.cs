using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager
{
    public enum Type
    {
        UI,
        Other,
    }

    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManager();
            }
            return _instance;
        }
    }

    private static string _path;
    private Dictionary<string, List<GameObject>> _objectPool = new Dictionary<string, List<GameObject>>();

    public GameObject Spawn(string name, Type type)
    {
        GameObject obj = null;
        Transform parent = null;

        if (type == Type.UI)
        {
            _path = "Prefab/UI/";
            parent = GameObject.Find("Canvas").transform;
        }
        else
        {
            _path = "Prefab/";
        }

        obj = (GameObject)GameObject.Instantiate(Resources.Load(_path + name), Vector3.zero, Quaternion.identity);
        obj.name = name;
        if (parent != null)
        {
            obj.transform.SetParent(parent);
        }
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        if (type == Type.UI)
        {
            obj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            obj.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }

        return obj;
    }

    public GameObject Spawn(GameObject sample)
    {
        GameObject obj = null;
        Transform parent = null;
        parent = sample.transform.parent;

        obj = GameObject.Instantiate(sample, Vector3.zero, Quaternion.identity);
        if (parent != null)
        {
            obj.transform.SetParent(parent);
        }
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        return obj;
    }
}

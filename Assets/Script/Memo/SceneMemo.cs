using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMemo
{
    public MySceneManager.SceneType CurrentScene = MySceneManager.SceneType.Villiage;

    public void SetData(MySceneManager.SceneType scene)
    {
        CurrentScene = scene;
    }
}

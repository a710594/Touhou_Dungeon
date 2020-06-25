using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour //因為要用 StartCoroutine 必須有 MonoBehaviour, 才建立了這個 class
{
    public void Generate(int floor)
    {
        StartCoroutine(GenerateFloor(floor));
    }

    public IEnumerator GenerateFloor(int floor)
    {
        //ExploreUI.Instance.SetLoadingGroup(true);
        yield return null;
        //DungeonBuilder.Instance.Generate(floor);
        //DungeonPainter.Instance.Paint(_mapInfo);

        //_playerPosition = _mapInfo.Start;
        //_mapInfo.GuardList.Add(_mapInfo.Goal);
        //SetFloor();
        //ExploreUI.Instance.SetLoadingGroup(false);
    }

    private void Start()
    {
        //Generate(1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public FieldEnemyRandom FieldEnemyRandom;

    private void Start()
    {
        FieldEnemyRandom.Init(101, transform.position);
        FieldEnemyRandom.SetData(GameObject.Find("ExploreCharacter").transform, ExploreController.Instance._pathFindList);
    }
}

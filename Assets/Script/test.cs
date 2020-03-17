using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 vector = Vector3.right;
        List<Vector2Int> positionList = Utility.GetLinePositionList(5, 3, Vector2Int.zero, Vector2Int.down);
        for (int i=0; i<positionList.Count; i++)
        {
            TilePainter.Instance.Painting("RedGrid", 0, positionList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2Int circlePoint = new Vector2Int();
        List<Vector2Int> circleList = new List<Vector2Int>();
        List<Vector2Int> lineList = new List<Vector2Int>();

        circleList = Utility.GetCirclePositionList(Vector2Int.zero, 6, false);
        for (int i = 0; i < circleList.Count; i++)
        {
            circlePoint = circleList[i];
            lineList = Utility.GetLinePositionList(Vector2Int.zero, circlePoint);

            if (lineList[0] != Vector2Int.zero)
            {
                lineList.Reverse();
            }
            for (int j = 0; j < lineList.Count; j++)
            {
                circleList.Remove(lineList[j]);
                if (lineList[j] == circlePoint)
                {
                    i--;
                }
                TilePainter.Instance.Painting("Ground", 2, lineList[j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

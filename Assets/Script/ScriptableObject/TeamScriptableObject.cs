using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//初始的隊伍成員

[CreateAssetMenu(fileName = "TeamScriptableObject", menuName = "TeamScriptableObject", order = 1)]
public class TeamScriptableObject : ScriptableObject
{
    public List<int> MemberList = new List<int>();
    public List<Vector2Int> PositionList = new List<Vector2Int>();
}

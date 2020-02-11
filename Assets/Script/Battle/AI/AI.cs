using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int[] SkillID;

    protected BattleCharacterAI _character;
    protected List<Skill> _skillList = new List<Skill>();

    public void Init()
    {
        for (int i = 0; i < SkillID.Length; i++)
        {
            _skillList.Add(SkillFactory.GetNewSkill(SkillID[i]));
        }
    }

    public void Init(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            _skillList.Add(SkillFactory.GetNewSkill(list[i]));
        }
    }

    public virtual void StartAI(BattleCharacterAI character)
    {
        _character = character;
        StartCoroutine(Run());
    }

    protected virtual IEnumerator Run()
    {
        yield return null;
    }

    private void Start()
    {

    }
}

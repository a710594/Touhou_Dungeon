using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int damage = 0;
        BattleCharacterInfo battleCharacterInfo_1 = new BattleCharacterInfo();
        BattleCharacterInfo battleCharacterInfo_2 = new BattleCharacterInfo();
        TeamMember teamMember_1 = new TeamMember();
        TeamMember teamMember_2 = new TeamMember();
        Skill skill;

        teamMember_1.Init(1, 1);
        battleCharacterInfo_1.Init(teamMember_1);

        battleCharacterInfo_2.Init(1, 1);

        skill = SkillFactory.GetNewSkill(1);
        damage = ((AttackSkill)skill).CalculateDamage(battleCharacterInfo_1, battleCharacterInfo_2, false);

        Debug.Log(battleCharacterInfo_1.Name + " 對 " + battleCharacterInfo_2 + " 造成了 " + damage + " 傷害");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

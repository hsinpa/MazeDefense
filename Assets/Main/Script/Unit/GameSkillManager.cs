using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Skill;
using TD.Database;

public class GameSkillMapper
{
    private Dictionary<string, BaseSkill> skillMappingTable;

    public GameSkillMapper() {
        skillMappingTable = GetSkillMappingTable();
    }

    public BaseSkill GetSkill(string p_id) {

        if (skillMappingTable != null && skillMappingTable.ContainsKey(p_id)) {
            return skillMappingTable[p_id];
        }
        return null;
    }

    private Dictionary<string, BaseSkill> GetSkillMappingTable() {

        return new Dictionary<string, BaseSkill> {
            { VariableFlag.Skill.Penetration, null },
            { VariableFlag.Skill.DiminishOverTime, null },
            { VariableFlag.Skill.Diffusion, new SkillDiffusion() },
            { VariableFlag.Skill.Slow, null },
            { VariableFlag.Skill.Teleport, null },
        };

    }

}

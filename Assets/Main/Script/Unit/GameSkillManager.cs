using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Skill;
using TD.Database;

public class GameSkillHandler
{
    private Dictionary<string, BaseSkill> skillMappingTable;

    public GameSkillHandler() {
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
            { VaraibleFlag.Skill.Penetration, new SkillDiffusion() },
            { VaraibleFlag.Skill.DiminishOverTime, new SkillDiffusion() },
            { VaraibleFlag.Skill.Diffusion, new SkillDiffusion() },
            { VaraibleFlag.Skill.Slow, new SkillDiffusion() },
            { VaraibleFlag.Skill.Teleport, new SkillDiffusion() },
        };

    }

}

using System.Collections;
using System.Collections.Generic;
using TD.Database;
using TD.Map;
using TD.Unit;
using UnityEngine;

namespace TD.Skill {

    public class SkillDiffusion : BaseSkill
{

        public List<UnitInterface> Execute(SkillStats skillStats, List<UnitInterface> units, GameDamageManager.DMGRegistry dmgRegistry, MapBlockManager mapBlockManager, MapGrid mapGrid)
        {

            var findUnits = mapGrid.FindUnitsFromRange<MonsterUnit>(dmgRegistry.target.currentTile, skillStats.parameter_1);
            if (findUnits != null) {
                int unitLength = findUnits.Count;
                if (unitLength > 0) {

                    for (int i = 0; i < unitLength; i++) {
                        //If FIND_UNIT not exist, and its not the initial target
                        if (!units.Contains(findUnits[i]) && findUnits[i] != dmgRegistry.target) {
                            units.Add(findUnits[i]);
                        }
                    }
                }
            }

            return units;
        }
    }

}
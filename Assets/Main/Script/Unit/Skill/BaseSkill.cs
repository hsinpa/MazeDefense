using TD.Database;
using TD.Unit;
using TD.Map;
using System.Collections.Generic;

namespace TD.Skill
{
    public interface BaseSkill
    {
        List<UnitInterface> Execute(SkillStats skillStats, List<UnitInterface> units, GameDamageManager.DMGRegistry dmgRegistry, MapBlockManager mapBlockManager, MapGrid mapGrid);
    }
}
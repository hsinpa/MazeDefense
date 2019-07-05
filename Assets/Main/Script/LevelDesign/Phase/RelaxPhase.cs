using System.Collections;
using System.Collections.Generic;
using TD.Database;
using TD.Unit;
using UnityEngine;

namespace TD.AI
{
    public class RelaxPhase : PhaseInterface
    {
        List<MonsterStats> _monsterStats;

        public void SetUp(List<MonsterStats> monsterUnits)
        {
            _monsterStats = monsterUnits;
        }

        public LevelDirector.UnitWave Calculate(int wave, GameUnitManager.UnitInfo towerUnitInfo, GameUnitManager.UnitInfo monsterUnitInfo)
        {
            throw new System.NotImplementedException();
        }

    }
}
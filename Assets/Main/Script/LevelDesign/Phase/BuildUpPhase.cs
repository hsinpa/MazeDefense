using System.Collections;
using System.Collections.Generic;
using TD.Unit;
using UnityEngine;
using TD.AI;
using UtilityBase;
using TD.Database;
namespace TD.AI
{

    public class BuildUpFphase : PhaseInterface
    {
        List<MonsterStats> _monsterStats;

        public void SetUp(List<MonsterStats> monsterUnits)
        {
            _monsterStats = monsterUnits;
        }

        public LevelDirector.UnitWave Calculate(int wave, GameUnitManager.UnitInfo towerUnitInfo, GameUnitManager.UnitInfo monsterUnitInfo)
        {
            int combinationNum = Random.Range(4, 8);

            LevelDirector.UnitWave unitWave = new LevelDirector.UnitWave();
            unitWave.phaseStructure = new LevelDirector.UnitStructure[combinationNum];
            int overallWarFieldScore = towerUnitInfo.value + monsterUnitInfo.value;

            for (int i = 0; i < combinationNum; i++)
            {
                int spawnDelayTime = Random.Range(8, 15);

                float modifier = Graph.Linear(Graph.Normalized(i + 1, combinationNum), 1, 0.5f);
                LevelDirector.UnitStructure unitStructure = new LevelDirector.UnitStructure();
                unitStructure.unitArray = new LevelDirector.Units[2];

                unitStructure.unitArray[0].monsterStats = PhaseUtility.FindMonsterByStrategy(_monsterStats, VariableFlag.Strategy.CastleFirst);
                unitStructure.unitArray[0].spawnNum = PhaseUtility.FindMonsterNum(15, 30, modifier);

                unitStructure.unitArray[1].monsterStats = PhaseUtility.FindMonsterByStrategy(_monsterStats, VariableFlag.Strategy.CastleFirst);
                unitStructure.unitArray[1].spawnNum = PhaseUtility.FindMonsterNum(15, 25, modifier);

                unitStructure.timeToNextWave = spawnDelayTime;
                unitWave.phaseStructure[i] = unitStructure;
            }

            return unitWave;
        }
    }
}
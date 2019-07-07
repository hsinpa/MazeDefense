using System.Collections;
using System.Collections.Generic;
using TD.Database;
using TD.Unit;
using UnityEngine;
using UtilityBase;

namespace TD.AI
{
    public class ClimaxPhase : PhaseInterface
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
                unitStructure.unitArray = new LevelDirector.Units[3];

                unitStructure.unitArray[0].monsterStats = PhaseUtility.FindMonsterByStrategy(_monsterStats, VariableFlag.Strategy.CastleFirst);
                unitStructure.unitArray[0].spawnNum = PhaseUtility.FindMonsterNum(25, 50, modifier);

                unitStructure.unitArray[1].monsterStats = PhaseUtility.FindMonsterByStrategy(_monsterStats, VariableFlag.Strategy.TowersFirst);
                unitStructure.unitArray[1].spawnNum = PhaseUtility.FindMonsterNum(10, 20, modifier);
                
                unitStructure.unitArray[2].monsterStats = PhaseUtility.FindMonsterByStrategy(_monsterStats, VariableFlag.Strategy.MoveStraight);
                unitStructure.unitArray[2].spawnNum = PhaseUtility.FindMonsterNum(5, 15, modifier);

                unitStructure.timeToNextWave = spawnDelayTime;
                unitWave.phaseStructure[i] = unitStructure;
            }

            return unitWave;
        }


    }
}
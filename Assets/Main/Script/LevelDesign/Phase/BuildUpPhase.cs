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

                unitStructure.unitArray[0].monsterStats = FindMonsterByStrategy(VariableFlag.Strategy.CastleFirst);
                unitStructure.unitArray[0].spawnNum = FindMonsterNum(25, 50, modifier);

                unitStructure.unitArray[1].monsterStats = FindMonsterByStrategy(VariableFlag.Strategy.CastleFirst);
                unitStructure.unitArray[1].spawnNum = FindMonsterNum(20, 40, modifier);

                unitStructure.timeToNextWave = spawnDelayTime;
                unitWave.phaseStructure[i] = unitStructure;
            }

            return unitWave;
        }

        private int FindMonsterNum(int minRange, int maxRange, float modifier)
        {
            int spawnMonster = Random.Range(minRange, maxRange);
            return (int)(spawnMonster * modifier);
        }

        private MonsterStats FindMonsterByStrategy(VariableFlag.Strategy strategy)
        {
            List<MonsterStats> mList = _monsterStats.FindAll(x => x.strategy == strategy);
            int mLength = mList.Count;
            if (mList != null && mLength > 0)
            {
                return mList[Random.Range(0, mLength)];
            }
            return null;
        }
    }
}
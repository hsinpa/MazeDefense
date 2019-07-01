using System.Collections;
using System.Collections.Generic;
using TD.Unit;
using UnityEngine;
using TD.AI;
using UtilityBase;
using TD.Database;

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

        for (int i = 0; i < combinationNum; i++) {
            float spawnDelayTime = Random.Range(8, 15);
            int spawnMonster = Random.Range(25, 50);

            float modifier = Graph.Linear(Graph.Normalized(i + 1, combinationNum), 1, 0.5f);
            LevelDirector.UnitStructure unitStructure = new LevelDirector.UnitStructure();
            unitStructure.unitDict = new Dictionary<TD.Database.MonsterStats, int>();

            unitStructure.unitDict.Add(FindMonsterByStrategy(VariableFlag.Strategy.CastleFirst),
                (int)(spawnMonster * modifier));

        }

        return unitWave;
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

using TD.Unit;
using TD.AI;
using TD.Database;
using System.Collections.Generic;

public interface PhaseInterface 
{
    void SetUp(List<MonsterStats> monsterUnits);
    LevelDirector.UnitWave Calculate(int wave, GameUnitManager.UnitInfo towerUnitInfo, GameUnitManager.UnitInfo monsterUnitInfo);
}

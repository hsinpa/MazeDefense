using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityBase;
using TD.Unit;
using Utility;
using TD.Database;
using System.Linq;

namespace TD.AI
{
    public class LevelDirector
    {
        public delegate void CallReinforcement(UnitStructure unitStructure);

        #region Parameter
        private int MaxUnit;
        private int MinUnit = 20;
        private int Pulling = 5;
        private int PullingRange = 2;
        private float PullingTimeRecord = 1;

        private int ReinforcementLength = 0;

        private GameUnitManager.UnitInfo unitInfo;

        private List<MonsterStats> _monsterStats;
        private UtilityTheory _utilityTheory;
        private GameUnitManager _gameUnitManager;
        #endregion

        #region Event
        public CallReinforcement OnCallReinforcement;
        #endregion

        public enum Phase {
            BuildUp,
            Climax,
            Relax
        }

        public LevelDirector(GameUnitManager gameUnitManager, List<MonsterStats> monsterUnits, int maxUnit) {
            _gameUnitManager = gameUnitManager;
            MaxUnit = maxUnit;
            _monsterStats = monsterUnits;
            _utilityTheory = new UtilityTheory();

        }


        public void OnUpdate() {

            unitInfo = _gameUnitManager.GetUnitCount(VariableFlag.Pooling.MonsterID);

            if (PullingTimeRecord < LevelDesignManager.Time) {

                CheckReinforcement();


                PullingTimeRecord = (LevelDesignManager.Time) + (Random.Range(Pulling - PullingRange, Pulling + PullingRange));
            } 
        }


        private void CheckReinforcement() {
            float reinforcePercent = Graph.QuadraticFunction(Graph.Normalized(10, 200), -1f, 0, 1);
            bool hasPermission =  UtilityMethod.PercentageGame(reinforcePercent);

            if (hasPermission) {
                if (OnCallReinforcement != null)
                    OnCallReinforcement(FindReinforceStructure());
            }
        }

        private UnitStructure FindReinforceStructure() {
            UnitStructure _UnitStructure;
            _UnitStructure.unitDict = new Dictionary<MonsterStats, int>();

            GameUnitManager.UnitInfo towerInfo = _gameUnitManager.GetUnitCount(VariableFlag.Pooling.TowerID);
            GameUnitManager.UnitInfo unitInfo = _gameUnitManager.GetUnitCount(VariableFlag.Pooling.MonsterID );
            int overallWarFieldScore = towerInfo.value + unitInfo.value;
            int minimunScore = 50;

            if (overallWarFieldScore < minimunScore)
                overallWarFieldScore = minimunScore;

            _UnitStructure.unitDict.Add(FindMonsterByStrategy(VariableFlag.Strategy.CastleFirst),
             (int)(Graph.QuadraticFunction(Graph.Normalized(unitInfo.value, overallWarFieldScore), -1f, 0, 1) * 50)
            );

            _UnitStructure.unitDict.Add(FindMonsterByStrategy(VariableFlag.Strategy.MoveStraight),
                             (int)(Graph.QuadraticFunction(Graph.Normalized(unitInfo.value, overallWarFieldScore), -1f, 0, 1) * 5)
                            );

            _UnitStructure.unitDict.Add(FindMonsterByStrategy(VariableFlag.Strategy.TowersFirst),
                 (int)(Graph.QuadraticFunction(Graph.Normalized(unitInfo.value, overallWarFieldScore), -1f, 0, 1) * 10)
                );

            return _UnitStructure;
        }

        private MonsterStats FindMonsterByStrategy(VariableFlag.Strategy strategy) {
            List<MonsterStats> mList = _monsterStats.FindAll(x => x.strategy == strategy);
            int mLength = mList.Count;
            if (mList != null && mLength > 0) {
                return mList[Random.Range(0, mLength)];
            }
            return null;
        }

        public struct UnitStructure {
            /// <summary>
            /// Monster ID, Spawn Length
            /// </summary>
            public Dictionary<MonsterStats, int> unitDict;

            public int timeToNextWave;
        }

        public struct UnitWave {
            public UnitStructure[] phaseStructure;
        }

    }
}
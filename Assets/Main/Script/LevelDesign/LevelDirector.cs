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
        public delegate void CallReinforcement(UnitStructure unitWave);

        #region Parameter
        private int MaxUnit;
        private int MinUnit = 20;
        private int Pulling = 5;
        private int PullingRange = 2;
        private float PullingTimeRecord = 1;

        private int WaveLength = 0;
        private int WaveCombination = 0;
        private int WaveCombinationLength = 0;
        private UnitWave _UnitWave;

        private GameUnitManager.UnitInfo unitInfo;

        private List<MonsterStats> _monsterStats;
        private UtilityTheory _utilityTheory;
        private GameUnitManager _gameUnitManager;
        private Dictionary<Phase, PhaseInterface> _phaseInterfaceDict;
        private int _phaseDictLength = 0;
        private Phase currentPhase ;
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

            _phaseInterfaceDict = SetUpPhaseAI();
            _phaseDictLength = _phaseInterfaceDict.Count;

            currentPhase = Phase.BuildUp;
            BuildWaveStructure(currentPhase);
        }

        public void OnUpdate() {

            unitInfo = _gameUnitManager.GetUnitCount(VariableFlag.Pooling.MonsterID);

            if (PullingTimeRecord < LevelDesignManager.Time) {
                CheckReinforcement();
            }
        }


        private void CheckReinforcement() {
            float reinforcePercent = Graph.QuadraticFunction(Graph.Normalized(10, 200), -1f, 0, 1);
            bool hasPermission =  UtilityMethod.PercentageGame(reinforcePercent);

            if (hasPermission) {

                if (WaveCombination >= WaveCombinationLength)
                {
                    int phaseIndex = ((int)currentPhase + 1) % _phaseDictLength;
                    currentPhase = (Phase)phaseIndex;
                    Debug.Log(currentPhase.ToString("g"));
                    //BuildWaveStructure();
                }
                
                else if (_UnitWave.phaseStructure != null) {
                    Debug.Log(WaveCombination +", " + WaveCombinationLength );

                    if (OnCallReinforcement != null)
                        OnCallReinforcement(_UnitWave.phaseStructure[WaveCombination]);

                    PullingTimeRecord = (LevelDesignManager.Time) + (Random.Range(_UnitWave.phaseStructure[WaveCombination].timeToNextWave - PullingRange,
                                                            _UnitWave.phaseStructure[WaveCombination].timeToNextWave + PullingRange));

                    WaveCombination++;
                }
            }
        }

        private void BuildWaveStructure(Phase phaseType)
        {
            PhaseInterface findPhaseInterface = null;
            GameUnitManager.UnitInfo towerInfo = _gameUnitManager.GetUnitCount(VariableFlag.Pooling.TowerID);
            GameUnitManager.UnitInfo unitInfo = _gameUnitManager.GetUnitCount(VariableFlag.Pooling.MonsterID);

            if (_phaseInterfaceDict.TryGetValue(phaseType, out findPhaseInterface)) {
                findPhaseInterface.SetUp(_monsterStats);

                _UnitWave = findPhaseInterface.Calculate(WaveLength, towerInfo, unitInfo);

                WaveCombination = 0;
                WaveCombinationLength = _UnitWave.phaseStructure.Length;

                return;
            };

            WaveCombination = 0;
            WaveCombinationLength = 0;

            _UnitWave = default(UnitWave);
        }

        private Dictionary<Phase, PhaseInterface> SetUpPhaseAI() {
            Dictionary<Phase, PhaseInterface> phaseInterface = new Dictionary<Phase, PhaseInterface>();
            phaseInterface.Add(Phase.BuildUp, new BuildUpFphase());
            phaseInterface.Add(Phase.Climax, new BuildUpFphase());
            phaseInterface.Add(Phase.Relax, new BuildUpFphase());
            return phaseInterface;
        }

        public struct Units {
            public MonsterStats monsterStats;
            public int spawnNum;
        }

        public struct UnitStructure {
            /// <summary>
            /// Monster ID, Spawn Length
            /// </summary>
            public Units[] unitArray;

            public int timeToNextWave;
        }

        public struct UnitWave {
            public UnitStructure[] phaseStructure;
        }

    }
}
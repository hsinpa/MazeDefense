using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using System.Linq;
using Utility;
using Hsinpa.StateEntity;

namespace TD.Unit {
    public class GameUnitManager : MonoBehaviour
    {
        private List<UnitInterface> unitList;
        private List<UnitInterface> pendingDestroyList = new List<UnitInterface>();

        private int unitLength = 0;

        public GameDamageManager gameDamageManager;
        private GameSkillMapper gameSkillMapper;

        private Dictionary<string, float> UnitCountDict;
        private Dictionary<string, float> UnitValueDict;

        private UnitInfo unitInfo;

        #region Public Method
        public void SetUp(MapBlockManager mapBlockManager, MapGrid mapGrid, int unitCapacity) {
            gameSkillMapper = new GameSkillMapper();
            gameDamageManager = new GameDamageManager(gameSkillMapper, mapBlockManager, mapGrid);

            unitList = new List<UnitInterface>(unitCapacity);

            UnitCountDict = new Dictionary<string, float>();
            UnitValueDict = new Dictionary<string, float>();
        }

        public void AddUnit(UnitInterface unit) {
            unit.ReadyToAction(RemoveUnit);

            unitList.Add(unit);
            unitLength++;

            string unitID = GetUnitIDByType(unit);
            if (unitID != null) {
                UnitCountDict = GeneralUtility.DictionaryIncrement<string>(UnitCountDict, unitID, 1);
                UnitValueDict = GeneralUtility.DictionaryIncrement<string>(UnitValueDict, unitID, unit.unitStats.value);
            }
        }

        public void RemoveUnit(UnitInterface unit) {
            pendingDestroyList.Add(unit);

            string unitID = GetUnitIDByType(unit);
            if (unitID != null)
            {
                UnitCountDict = GeneralUtility.DictionaryIncrement<string>(UnitCountDict, unitID, -1);
                UnitValueDict = GeneralUtility.DictionaryIncrement<string>(UnitValueDict, unitID, -unit.unitStats.value);
            }
        }
        #endregion

        #region Private Method
        void Update()
        {
            HandleDestroyUnit();

            bool is_mapblock_dragging = StateEntityManager.Query(StateEntityManager.BoolOperation.AND, EntityData.Tag.MapBlockDrag);
            if (!is_mapblock_dragging) {
                for (int i = 0; i < unitLength; i++)
                {
                    unitList[i].OnUpdate();
                }
            }

            gameDamageManager.OnUpdate();
        }

        private void HandleDestroyUnit() {
            int pendingCount = pendingDestroyList.Count;

            for (int i = 0; i < pendingCount; i++)
            {
                int index = unitList.IndexOf( pendingDestroyList[i] );

                if (index >= 0) {
                    Pooling.PoolManager.instance.Destroy(pendingDestroyList[i].unitObject);
                    unitList.RemoveAt(index);
                    unitLength--;
                }
            }
            pendingDestroyList.Clear();
        }
        #endregion

        public UnitInfo GetUnitCount(string unit_id) {
            unitInfo.value = 0;
            unitInfo.count = 0;

            if (UnitCountDict != null && UnitCountDict.ContainsKey(unit_id))
                unitInfo.count = (int)(UnitCountDict[unit_id]);

            if (UnitValueDict != null && UnitValueDict.ContainsKey(unit_id))
                unitInfo.value = (int)(UnitValueDict[unit_id]);

            return unitInfo;
        }

        public void Reset()
        {
            unitLength = 0;
            unitList.Clear();
            pendingDestroyList.Clear();
            gameDamageManager.Reset();
        }

        private string GetUnitIDByType(UnitInterface p_unit) {
            if (p_unit.GetType() == typeof(MonsterUnit))
            {
                return VariableFlag.Pooling.MonsterID;
            }

            if (p_unit.GetType() == typeof(TowerUnit))
            {
                return VariableFlag.Pooling.TowerID;
            }

            return null;
        }

        public struct UnitInfo {
            public int count;
            public int value;
        }
    }
}
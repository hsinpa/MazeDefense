using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using System.Linq;

namespace TD.Unit {
    public class GameUnitManager : MonoBehaviour
    {
        private List<UnitInterface> unitList = new List<UnitInterface>();
        private List<UnitInterface> pendingDestroyList = new List<UnitInterface>();

        private int unitLength = 0;

        public GameDamageManager gameDamageManager;
        private GameSkillMapper gameSkillMapper;

        #region Public Method
        public void SetUp(MapBlockManager mapBlockManager, MapGrid mapGrid) {
            gameSkillMapper = new GameSkillMapper();
            gameDamageManager = new GameDamageManager(gameSkillMapper, mapBlockManager, mapGrid);
        }

        public void AddUnit(UnitInterface unit) {
            unit.ReadyToAction(RemoveUnit);

            unitList.Add(unit);
            unitLength++;
        }

        public void RemoveUnit(UnitInterface unit) {
            pendingDestroyList.Add(unit);
        }
        #endregion

        #region Private Method
        void Update()
        {
            HandleDestroyUnit();

            for (int i = 0; i < unitLength; i++) {
                unitList[i].OnUpdate();
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


        public void Reset()
        {
            unitLength = 0;
            unitList.Clear();
            pendingDestroyList.Clear();
            gameDamageManager.Reset();
        }

    }
}
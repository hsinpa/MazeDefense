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
        private MapGrid _mapGrid;

        #region Public Method
        public void SetUp(MapGrid mapGrid) {
            _mapGrid = mapGrid;
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
        }

    }
}
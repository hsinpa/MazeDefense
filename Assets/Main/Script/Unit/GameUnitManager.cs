using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Unit {
    public class GameUnitManager : MonoBehaviour
    {
        private List<BaseUnit> unitList = new List<BaseUnit>();
        private List<BaseUnit> pendingDestroyList = new List<BaseUnit>();

        private int unitLength = 0;

        #region Public Method
        public void AddUnit(BaseUnit unit) {
            unit.ReadyToAction(RemoveUnit);

            unitList.Add(unit);
            unitLength++;
        }

        public void RemoveUnit(BaseUnit unit) {
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
            unitList.Clear();
        }

    }
}
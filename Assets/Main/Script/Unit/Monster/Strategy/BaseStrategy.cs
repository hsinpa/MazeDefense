using TD.Map;
using TD.Database;
using TD.Unit;
using UnityEngine;
using System.Collections.Generic;

namespace TD.AI {
    public abstract class BaseStrategy
    {
        protected MonsterStats monsterStat;
        protected MapGrid mapGrid;
        protected MonsterUnit unit;

        protected Vector3 moveDelta;

        /// <summary>
        /// ObjectID, Time
        /// </summary>
        protected Dictionary<int, float> FireRateRecord = new Dictionary<int, float>();

        protected float GetFireRateRecord(int objectID)
        {
            if (FireRateRecord.ContainsKey(objectID))
            {
                return FireRateRecord[objectID];
            }
            else
            {
                return 0;
            }
        }

        protected void InsertFireRateRecord(int objectID, float time) {
            if (FireRateRecord.ContainsKey(objectID))
            {
                FireRateRecord[objectID] = time;
            }
            else {
                FireRateRecord.Add(objectID, time);
            }
        }

        public void SetUp(MonsterUnit unit, MonsterStats monsterStat, MapGrid mapGrid) {
            this.unit = unit;
            this.monsterStat = monsterStat;
            this.mapGrid = mapGrid;
        }

        public virtual void Execute(TileNode blockComponent) {

        }
    }
}
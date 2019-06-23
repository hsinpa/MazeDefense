using TD.Map;
using TD.Database;
using TD.Unit;
using UnityEngine;
using System.Collections.Generic;
using TD.Map;
using Utility;

namespace TD.AI {
    public abstract class BaseStrategy
    {
        #region Parameter
        protected MonsterStats monsterStat;
        protected MapGrid mapGrid;
        protected MonsterUnit unit;
        protected int _uniqueID;

        protected Vector3 moveDelta;
        protected Vector2 zeroDelta = new Vector2(0, 0);

        protected VariableFlag.Strategy strategy;

        private GameDamageManager _gameDamageManager;

        private Dictionary<int, float> AttackTimeDict = new Dictionary<int, float>();
        
        #endregion

        /// <summary>
        /// ObjectID, Time
        /// </summary>
        protected Dictionary<int, float> FireRateRecord = new Dictionary<int, float>();

        public void SetUp(MonsterUnit unit, int uniqueID, MonsterStats monsterStat, MapGrid mapGrid, GameDamageManager gameDamageManager)
        {
            this.unit = unit;
            this.monsterStat = monsterStat;
            this.mapGrid = mapGrid;
            this._gameDamageManager = gameDamageManager;
            this._uniqueID = uniqueID;

            if (!AttackTimeDict.ContainsKey(uniqueID)) {
                AttackTimeDict.Add(uniqueID, 0);
            }
        }

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

        protected void AttackOnTower(TowerUnit towerUnit)
        {
            float lastAttackTime = 0;
            if (AttackTimeDict.ContainsKey(_uniqueID))
                lastAttackTime = AttackTimeDict[_uniqueID];
            else
                AttackTimeDict.Add(_uniqueID, lastAttackTime);

            //Go for it
            if (LevelDesignManager.Time > lastAttackTime) {
                float meleeAttackTime = 5f;
                float reachTime = GeneralUtility.GetReachTimeGivenInfo(towerUnit.transform.position, unit.transform.position, meleeAttackTime, LevelDesignManager.DeltaTime);

                GameDamageManager.DMGRegistry registry = GeneralUtility.GetDMGRegisterCard(towerUnit, monsterStat, LevelDesignManager.Time, reachTime);

                _gameDamageManager.AddRequest(registry);
                AttackTimeDict[_uniqueID] = LevelDesignManager.Time + monsterStat.spd;
            }
        }



        public virtual VariableFlag.Strategy Think(TileNode blockComponent, VariableFlag.Strategy currentStrategy) {
            return VariableFlag.Strategy.None;
        }

        public void Execute(TileNode currentNode, VariableFlag.Strategy strategy)
        {

            if (currentNode.IsValidNode && currentNode.towerUnit != null)
            {
                //Enter attack mode
                AttackOnTower(currentNode.towerUnit);
                return;
            }


            if (strategy == VariableFlag.Strategy.None)
                return;

            moveDelta.Set((currentNode.GetFlowFieldPath(strategy).x),
                            currentNode.GetFlowFieldPath(strategy).y, 0);

            moveDelta *= Time.deltaTime * monsterStat.moveSpeed * 0.3f;

            unit.transform.position += moveDelta;
        }

        protected VariableFlag.Strategy ChooseMoveStrategy(TileNode currentNode, VariableFlag.Strategy[] strategyOrder, VariableFlag.Strategy defaultStrategy) {

            if (strategyOrder == null || strategyOrder.Length <= 0)
                return defaultStrategy;

            int sLength = strategyOrder.Length;
            for (int i = 0; i < sLength; i++) {
                if (currentNode.GetFlowFieldPath(strategyOrder[i]) != zeroDelta) {
                    return strategyOrder[i];
                }
            }

            return defaultStrategy;
        }

    }
}
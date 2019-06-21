using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Database;
using TD.Unit;

namespace TD.AI {
    public class StrategyCastleFirst : BaseStrategy
    {
        private Vector2 zeroDelta = new Vector2(0, 0);

        private VariableFlag.Strategy strategy;

        public override void Execute(TileNode currentNode)
        {
            strategy = ChooseMoveStrategy(currentNode);
            AgentMove(currentNode, strategy);

            Debug.Log(strategy.ToString("g"));

            //GO Destroy IT
            if (currentNode.towerUnit != null && currentNode.towerUnit.isActive) {
                AttackOnTower(currentNode.towerUnit);
            }
        }

        protected override void AttackOnTower(TowerUnit towerUnit) {
            Debug.Log("Attack " + towerUnit.name);
        }

        private VariableFlag.Strategy ChooseMoveStrategy(TileNode currentNode) {
            //No path to go
            if (currentNode.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst) != zeroDelta)
            {
                return VariableFlag.Strategy.CastleFirst;
            }
            else
            {
                if (currentNode.GetFlowFieldPath(VariableFlag.Strategy.TowersFirst) != zeroDelta)
                    return VariableFlag.Strategy.TowersFirst;
                else
                    return VariableFlag.Strategy.None;
            }
        }

        private void AgentMove(TileNode currentNode, VariableFlag.Strategy strategy)
        {
            if (strategy == VariableFlag.Strategy.None)
                return;

            moveDelta.Set((currentNode.GetFlowFieldPath(strategy).x),
                            currentNode.GetFlowFieldPath(strategy).y, 0);

            moveDelta *= Time.deltaTime * monsterStat.spd * 0.3f;

            unit.transform.position += moveDelta;
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Database;

namespace TD.AI {
    public class StrategyCastleFirst : BaseStrategy
    {
        private Vector2 zeroDelta = new Vector2(0, 0);

        private VariableFlag.Strategy strategy;

        public override void Execute(TileNode currentNode)
        {
            strategy = ChooseMoveStrategy(currentNode);
            AgentMove(currentNode, strategy);

            if (currentNode.towerUnit != null && currentNode.towerUnit.isActive) {
                
            }
        }

        private void AttackOnTower() {

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

            moveDelta.Set((currentNode.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst).x),
                            currentNode.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst).y, 0);

            moveDelta *= Time.deltaTime * monsterStat.spd * 0.3f;

            unit.transform.position += moveDelta;
        }


    }
}

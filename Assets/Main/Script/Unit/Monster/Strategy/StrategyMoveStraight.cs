using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Database;
using TD.Unit;

namespace TD.AI
{
    public class StrategyMoveStraight : BaseStrategy
    {
        private Vector2Int _modifiedVector2;
        private Vector2 directionVector;

        private VariableFlag.Strategy[] _strategyList = new VariableFlag.Strategy[] {
            VariableFlag.Strategy.CastleFirst,
            VariableFlag.Strategy.TowersFirst
        };

        private VariableFlag.Strategy _fallbackStrategy = VariableFlag.Strategy.None;

        public override VariableFlag.Strategy Think(TileNode currentNode, VariableFlag.Strategy currentStrategy)
        {
            //If is under destination block, then go with other strategy
            if(!currentNode.IsValidNode || currentNode.BlockComponent.map_type == BlockComponent.Type.Exist)
                return ChooseMoveStrategy(currentNode, _strategyList, _fallbackStrategy);

            //Find destination direction
            Vector3 direction = (mapGrid.DestinationNode[0].WorldSpace - unit.transform.position).normalized;
            directionVector.Set(0, (direction.y > 0) ? 1 : -1);

            //Move toward castle, but in straight line
            _modifiedVector2.Set(currentNode.GridIndex.x, currentNode.GridIndex.y + (int)directionVector.y);

            //Go Straight Forward
            TileNode nextStraightNode = mapGrid.FindTileNodeByIndex(_modifiedVector2);

            //During Castle/Tower, if movement is downwward and the next step is empty, back to MoveStraight
            if (currentStrategy == VariableFlag.Strategy.CastleFirst || currentStrategy == VariableFlag.Strategy.TowersFirst)
            {
                if (nextStraightNode.IsValidNode && nextStraightNode.IsWalkable && currentNode.GetFlowFieldPath(currentStrategy) == directionVector)
                {
                    return VariableFlag.Strategy.MoveStraight;
                }
                return currentStrategy;
            }

            //Go Straight Forward
            if (nextStraightNode.IsValidNode && nextStraightNode.IsWalkable)
            {
                return VariableFlag.Strategy.MoveStraight;
            }

            return ChooseMoveStrategy(currentNode, _strategyList, _fallbackStrategy);
        }

    }
}

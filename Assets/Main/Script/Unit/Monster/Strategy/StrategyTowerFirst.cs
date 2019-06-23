using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Database;
using TD.Unit;

namespace TD.AI
{
    public class StrategyTowerFirst : BaseStrategy
    {
        private VariableFlag.Strategy[] _strategyList = new VariableFlag.Strategy[] {
            VariableFlag.Strategy.TowersFirst,
            VariableFlag.Strategy.CastleFirst
        };

        private VariableFlag.Strategy _fallbackStrategy = VariableFlag.Strategy.None;

        public override VariableFlag.Strategy Think(TileNode currentNode, VariableFlag.Strategy currentStrategy)
        {
            strategy = ChooseMoveStrategy(currentNode, _strategyList, _fallbackStrategy);

            return strategy;
        }

    }
}

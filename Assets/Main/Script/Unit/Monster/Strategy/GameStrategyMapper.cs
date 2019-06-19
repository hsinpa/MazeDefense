using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.AI
{
    public class GameStrategyMapper
    {
        private Dictionary<VariableFlag.Strategy, BaseStrategy> strategyMappingTable;

        public GameStrategyMapper()
        {
            strategyMappingTable = GetMappingTable();
        }

        public BaseStrategy GetStrategy(VariableFlag.Strategy p_id)
        {
            if (strategyMappingTable != null && strategyMappingTable.ContainsKey(p_id))
            {
                return strategyMappingTable[p_id];
            }
            return null;
        }

        private Dictionary<VariableFlag.Strategy, BaseStrategy> GetMappingTable()
        {
            return new Dictionary<VariableFlag.Strategy, BaseStrategy> {
                { VariableFlag.Strategy.CastleFirst, new StrategyCastleFirst() },
                { VariableFlag.Strategy.TowersFirst, null },
                { VariableFlag.Strategy.MoveStraight, null },
            };
        }
    }
}
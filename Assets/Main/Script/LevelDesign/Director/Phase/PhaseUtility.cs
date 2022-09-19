using UnityEngine;
using TD.Database;
using System.Collections.Generic;

namespace TD.AI
{
    public class PhaseUtility
    {
        public static int FindMonsterNum(int minRange, int maxRange, float modifier)
        {
            int spawnMonster = Random.Range(minRange, maxRange);
            return (int)(spawnMonster * modifier);
        }

        public static MonsterStats FindMonsterByStrategy(List<MonsterStats> monsterStats, VariableFlag.Strategy strategy)
        {
            List<MonsterStats> mList = monsterStats.FindAll(x => x.strategy == strategy);
            int mLength = mList.Count;
            if (mList != null && mLength > 0)
            {
                return mList[Random.Range(0, mLength)];
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Database
{
    public class MonsterStats : BaseStats
    {

        public VariableFlag.Path strategy;

        public float value;
        public int hp;
        public float atk;
        public float spd;
        public float range;

        public float avgPrize;
        public float prize {
            get {
                float rand = Random.Range(-20, 20);
                float resetPrize = avgPrize + rand;
                return Mathf.Clamp(resetPrize, 5, resetPrize);
            }
        }

        public SkillStats[] skills;

        public string sprite_id;
    }
}
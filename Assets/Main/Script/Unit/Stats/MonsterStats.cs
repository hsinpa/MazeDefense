using UnityEngine;

namespace TD.Database
{
    public class MonsterStats : UnitStats
    {

        public VariableFlag.Strategy strategy;

        public float value;
        public float moveSpeed;

        public float avgPrize;
        public float prize {
            get {
                float rand = Random.Range(-20, 20);
                float resetPrize = avgPrize + rand;
                return Mathf.Clamp(resetPrize, 5, resetPrize);
            }
        }

        public string sprite_id;
        public RuntimeAnimatorController animator;
    }
}
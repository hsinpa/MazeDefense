using UnityEngine;

namespace TD.Database
{
    public class TowerStats : UnitStats
    {
        public int level;
        public int cost;

        public TowerStats[] upgrade_path;
        public string tag;

        public Sprite sprite;

        //Only work at editor mode
        public void AddValue(float p_value) {
            this.value += p_value;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TD.Database
{
    public class TowerStats : BaseStats
    {
        public int level;

        public float atk;
        public float speed;
        public float range;
        public int cost;

        public TowerStats[] upgrade_path;
        public SkillStats[] skills;

        public string tag;
    }
}
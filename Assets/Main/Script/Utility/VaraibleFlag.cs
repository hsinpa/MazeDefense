using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableFlag
{
    public class Pooling {
        public const string TowerID = "the_tower";
        public const string MonsterID = "the_monster";
        public const string BulletID = "the_bullet";
    }

    public class Skill {
        public const string Penetration = "skill_01";
        public const string DiminishOverTime = "skill_02";
        public const string Diffusion = "skill_03";
        public const string Slow = "skill_04";
        public const string Teleport = "skill_05";
    }

    public enum Path {
        CastleFirst = 0,
        TowersFirst
    }

}

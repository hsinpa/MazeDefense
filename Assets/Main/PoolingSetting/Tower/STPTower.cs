using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pooling
{
    [CreateAssetMenu(fileName = "[STP]Tower", menuName = "STP/Tower", order = 2)]
    public class STPTower : STPObject
    {
        public float damage;
        public float frequency;
        public float range;

    }
}
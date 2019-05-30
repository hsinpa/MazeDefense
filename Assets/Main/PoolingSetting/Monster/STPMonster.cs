using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pooling
{
    [CreateAssetMenu(fileName = "[STP]Monster", menuName = "STP/Monster", order = 3)]
    public class STPMonster : STPObject
    {
        public float hp;
        public float def;
        public float damage;
        public float moveSpeed;
       
    }
}
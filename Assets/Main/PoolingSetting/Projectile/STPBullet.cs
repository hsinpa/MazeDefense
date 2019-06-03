using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pooling
{
    [CreateAssetMenu(fileName = "[STP]Bullet", menuName = "STP/Bullet", order = 4)]
    public class STPBullet : STPObject
    {
        public Sprite sprite;
        public float moveSpeed;
    }
}
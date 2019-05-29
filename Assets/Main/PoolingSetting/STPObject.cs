using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public abstract class STPObject : ScriptableObject
    {
        public string _id;
        public string _tag;
        public int poolingNum = 1;
        public GameObject prefab;
    }
}
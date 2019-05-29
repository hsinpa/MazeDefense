using UnityEngine;

namespace Pooling
{
    [System.Serializable]
    public struct PoolingObject
    {
        public int instanceNum;
        public GameObject prefab;
    }
}
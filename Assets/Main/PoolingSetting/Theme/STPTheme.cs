using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pooling
{
    [CreateAssetMenu(fileName = "[STP]Theme", menuName = "STP/Theme", order = 1)]
    public class STPTheme : ScriptableObject
    {
        public List<STPObject> stpObjectHolder = new List<STPObject>();
    }
}
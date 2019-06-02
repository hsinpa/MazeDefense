using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pooling
{
    [CreateAssetMenu(fileName = "[STP]Theme", menuName = "STP/Theme", order = 1)]
    public class STPTheme : ScriptableObject
    {
        public List<STPObject> stpObjectHolder = new List<STPObject>();

        public T FindObject<T>(string id) where T : STPObject {
            return stpObjectHolder.Find(x => x._id == id) as T;
        }
    }
}
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

        public int total {
            get {
                int count = 0;
                int objectLength = stpObjectHolder.Count;

                for (int i = 0; i < objectLength; i++)
                {
                    count += stpObjectHolder[i].poolingNum;
                }
                return count;
            }
        }

        public T FindObject<T>(string id) where T : STPObject {
            return stpObjectHolder.Find(x => x._id == id) as T;
        }

        public List<T> FindObjectByType<T>() where T : STPObject
        {
            List<T> findObjects = new List<T>();
            for (int i = 0; i < stpObjectHolder.Count; i++) {
                if (stpObjectHolder[i].GetType() == typeof(T))
                    findObjects.Add((T)stpObjectHolder[i]);
            }
            return findObjects;
        }
    }
}
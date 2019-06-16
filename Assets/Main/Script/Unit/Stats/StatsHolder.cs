using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TD.Database
{
    [CreateAssetMenu(fileName = "[Stats]Holder", menuName = "Stats/Holder", order = 1)]
    public class StatsHolder : ScriptableObject
    {
        public List<BaseStats> stpObjectHolder = new List<BaseStats>();

        public T FindObject<T>(string id) where T : BaseStats
        {
            return stpObjectHolder.Find(x => x.id == id) as T;
        }

        public List<T> FindObjectByType<T>() where T : BaseStats
        {
            List<T> findObjects = new List<T>();
            for (int i = 0; i < stpObjectHolder.Count; i++)
            {
                if (stpObjectHolder[i].GetType() == typeof(T))
                    findObjects.Add((T)stpObjectHolder[i]);
            }
            return findObjects;
        }
    }
}
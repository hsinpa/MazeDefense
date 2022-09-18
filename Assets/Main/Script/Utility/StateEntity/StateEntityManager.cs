using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.StateEntity
{
    public class StateEntityManager : MonoBehaviour
    {
        private static HashSet<EntityData.Tag> _entityTags = new HashSet<EntityData.Tag>();

        public static System.Action<EntityData.Tag> OnEntityPushed;
        public static System.Action<EntityData.Tag> OnEntityRemoved;

        public enum BoolOperation { AND, OR }

        public static bool Query(BoolOperation boolOperation, params EntityData.Tag[] queryTags) {
            int length = queryTags.Length;

            //If both length 0, no mode exist
            if (length == 0 && _entityTags.Count == 0) return true;
            
            //AND operation
            bool queryValid = false;
            for (int i = length - 1; i >= 0; i--) {
                queryValid = _entityTags.Contains(queryTags[i]);

                if (boolOperation == BoolOperation.AND)
                    if (queryValid == false) return false;

                if (boolOperation == BoolOperation.OR)
                    if (queryValid == true) return true;
            }

            return queryValid;
        }

        public static void PushEntity(EntityData.Tag entity)
        {
            if (_entityTags.Contains(entity)) return;

            _entityTags.Add(entity);

            OnEntityPushed?.Invoke(entity);
        }

        public static void RemoveEntity(EntityData.Tag entity) 
        {
            bool hasRemove = _entityTags.Remove(entity);

            if (hasRemove)
                OnEntityRemoved?.Invoke(entity);
        }

        public static void Dispose() {
            _entityTags.Clear();
            OnEntityPushed = null;
            OnEntityRemoved = null;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling {
    public class PoolManager : MonoBehaviour
    {
        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

        static PoolManager _instance;

        private readonly Vector2 _destroyPos = new Vector2(0,0);

        public static PoolManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PoolManager>();
                }
                return _instance;
            }
        }

        public void CreatePool(GameObject prefab, string poolkey, int p_poolsize)
        {

            if (!poolDictionary.ContainsKey(poolkey))
            {
                poolDictionary.Add(poolkey, new Queue<GameObject>());
                for (int i = 0; i < p_poolsize; i++)
                {
                    if (poolDictionary[poolkey].Count > p_poolsize) break;

                    GameObject newObject = Instantiate(prefab) as GameObject;
                    newObject.name = newObject.name + "-" + i;
                    newObject.transform.SetParent(this.transform);
                    SetActive(newObject, false);
                    //newObject.SetActive(false);
                    poolDictionary[poolkey].Enqueue(newObject);
                }
            }
        }

        public GameObject ReuseObject(string poolKey)
        {
            if (poolDictionary.ContainsKey(poolKey))
            {
                GameObject objectToReuse = poolDictionary[poolKey].Dequeue();

                SetActive(objectToReuse, true);

                poolDictionary[poolKey].Enqueue(objectToReuse);
                return objectToReuse;
            }

            return null;
        }

        private void SetActive(GameObject p_gameobject, bool p_enable) {
            Renderer renderer = p_gameobject.GetComponent<Renderer>();
            if (renderer)
                renderer.enabled = p_enable;

            Collider2D collider = p_gameobject.GetComponent<Collider2D>();
            if (collider)
                collider.enabled = p_enable;
        }

        public void Destroy(GameObject p_gameobject)
        {
            p_gameobject.transform.position = _destroyPos;
            p_gameobject.transform.SetParent(this.transform);
            SetActive(p_gameobject, false);

        }
    }
}


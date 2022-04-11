using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public static class PooledObjectNameList
    {
        public static string NameOfArrow = "Arrow";
        public static string NameOfProjectile = "Projectile";
    }

    public class ObjectPoolManager<T> where T : MonoBehaviour
    {
        #region Variables
        private T context;

        private Dictionary<string, List<T>> pooledObjects = new Dictionary<string, List<T>>();

        #endregion Variables

        #region Properties
        public Dictionary<string, List<T>> PooledObjects => pooledObjects;

        #endregion Properties
        
        public ObjectPoolManager(string initialKey)
        {
            pooledObjects[initialKey] = new List<T>();
            CreatePooledObjects(initialKey);
        }

        public ObjectPoolManager(string initialKey, Transform spawnPoint)
        {
            pooledObjects[initialKey] = new List<T>();
            CreatePooledObjects(initialKey, spawnPoint);
        }

        #region Unity Methods

        #endregion Unity Methods

        #region Helper Methods
        public void CreatePooledObjects(string key, Transform spawnPoint = null, T pooledObject = null)
        {
            if (pooledObjects.ContainsKey(key))
            {
                for (int i = 0; i < 10; i++)
                {
                    // GameObject newGO = Resources.Load<GameObject>("Prefabs/" + key);
                    GameObject newGO = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + key), spawnPoint);
                    if (spawnPoint != null)
                    {
                        // GameObject newGO = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + key), spawnPoint);
                        newGO.transform.position = spawnPoint.position;
                    }
                    newGO.SetActive(false);
                    pooledObject = newGO.GetComponent<T>();
                    pooledObjects[key].Add(pooledObject);
                }

                return;
            }
            
            pooledObjects[key] = new List<T>();
            CreatePooledObjects(key);
        }

        public T GetPooledObject(string key)
        {
            if (pooledObjects.ContainsKey(key))
            {
                foreach(T pooledObject in pooledObjects[key])
                {
                    if (!pooledObject.gameObject.activeSelf)
                    {
                        // pooledObject.gameObject.SetActive(true);
                        return pooledObject;
                    }
                }

                int beforeCreateCount = pooledObjects[key].Count;
                CreatePooledObjects(key);
                pooledObjects[key][beforeCreateCount].gameObject.SetActive(true);
                return pooledObjects[key][beforeCreateCount];
            }

            return null;
        }

        #endregion Helper Methods
    }
}
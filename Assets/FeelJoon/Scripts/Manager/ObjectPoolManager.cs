using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public static class PooledObjectNameList
    {
        public static string NameOfArrow = "Arrow";
        public static string NameOfProjectile = "Projectile";
    }

    public sealed class ObjectPoolManager<T> where T : MonoBehaviour
    {
        #region Variables
        public Transform spawnPoint;

        private Dictionary<string, List<T>> pooledObjects = new Dictionary<string, List<T>>();

        #endregion Variables

        #region Properties
        public Dictionary<string, List<T>> PooledObjects => pooledObjects;

        #endregion Properties

        public ObjectPoolManager(string initialKey, Transform spawnPoint)
        {
            this.spawnPoint = spawnPoint;

            pooledObjects[initialKey] = new List<T>();
            CreatePooledObjects(initialKey, spawnPoint);
        }

        #region Unity Methods

        #endregion Unity Methods

        #region Helper Methods
        public void CreatePooledObjects(string key, Transform spawnPoint, T pooledObject = null)
        {
            if (pooledObjects.ContainsKey(key))
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject newGO = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + key), 
                        spawnPoint.position, 
                        Quaternion.identity);
                    newGO.SetActive(false);
                    pooledObject = newGO.GetComponent<T>();
                    pooledObjects[key].Add(pooledObject);
                }

                return;
            }
            
            pooledObjects[key] = new List<T>();
            CreatePooledObjects(key, this.spawnPoint);
        }

        public T GetPooledObject(string key)
        {
            if (pooledObjects.ContainsKey(key))
            {
                foreach(T pooledObject in pooledObjects[key])
                {
                    if (!pooledObject.gameObject.activeSelf)
                    {
                        return pooledObject;
                    }
                }

                int beforeCreateCount = pooledObjects[key].Count;
                CreatePooledObjects(key, this.spawnPoint);
                pooledObjects[key][beforeCreateCount].gameObject.SetActive(true);
                return pooledObjects[key][beforeCreateCount];
            }

            return null;
        }

        #endregion Helper Methods
    }
}
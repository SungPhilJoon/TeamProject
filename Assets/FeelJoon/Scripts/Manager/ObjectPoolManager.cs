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
        
        public ObjectPoolManager(T context, string initialKey)
        {
            this.context = context;

            pooledObjects[initialKey] = new List<T>();
            CreatePooledObjects(initialKey, context);
        }

        #region Unity Methods

        #endregion Unity Methods

        #region Helper Methods
        public void CreatePooledObjects(string key, T pooledObject)
        {
            if (pooledObjects.ContainsKey(key))
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject newGO = Resources.Load<GameObject>(Application.dataPath + "/FeelJoon/Resources/Prefabs/" + key);
                    newGO.SetActive(false);
                    pooledObjects[key].Add(pooledObject);
                }

                return;
            }
            
            pooledObjects[key] = new List<T>();
            CreatePooledObjects(key, pooledObject);
        }

        public GameObject GetPooledObject(string key, GameObject pooledObjectGO)
        {
            GameObject returnGO = new GameObject();
            
            if (context == null)
            {
                return null;
            }

            return pooledObjectGO;
        }

        #endregion Helper Methods
    }
}
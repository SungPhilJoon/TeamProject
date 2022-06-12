using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public abstract class ManualCollision : MonoBehaviour
    {
        #region Variables
        [SerializeField] protected Transform parent;

        [SerializeField] protected Vector3 boxSize = new Vector3(3, 2, 2);

        public Collider[] targetColliders = new Collider[10];

        #endregion Variables

        #region Unity Methods
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
#endif

        #endregion Unity Methods

        #region Helper Methods
        public abstract void CheckCollision();

        #endregion Helper Methods
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class EnemyManualCollision : ManualCollision
    {
        #region Variables
        private EnemyController enemyController;

        public Vector3 boxSize = new Vector3(3, 2, 2);

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            enemyController = GetComponentInParent<EnemyController>();
        }

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
        public override void CheckCollision()
        {
            Physics.OverlapBoxNonAlloc(transform.position, boxSize * 0.5f, targetColliders, transform.rotation, enemyController.targetMask);
        }

        #endregion Helper Methods
    }
}
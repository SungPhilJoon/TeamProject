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

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            enemyController = GetComponentInParent<EnemyController>();
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CheckCollision()
        {
            targetColliders = Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, enemyController.targetMask);
        }

        #endregion Helper Methods
    }
}
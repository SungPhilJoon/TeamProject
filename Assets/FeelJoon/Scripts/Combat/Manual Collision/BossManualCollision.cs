using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class BossManualCollision : ManualCollision
    {
        #region Variables
        private BossController bossController;

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            bossController = GetComponentInParent<BossController>();
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CheckCollision()
        {
            targetColliders = Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, bossController.targetMask);
        }

        #endregion Helper Methods
    }
}
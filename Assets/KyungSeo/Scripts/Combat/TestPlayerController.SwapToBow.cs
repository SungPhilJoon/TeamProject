using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class TestPlayerController : PlayerController
    {
        #region Variables
        public Transform spawnPoint;

        public ObjectPoolManager<Arrow> objectPoolManager;

        #endregion Variables

        #region Helper Methods
        private void NormalBowAttack(Transform spawnPoint)
        {
            Arrow arrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            arrow.moveSpeed = 10f;
        }

        #endregion Helper Methods
    }
}
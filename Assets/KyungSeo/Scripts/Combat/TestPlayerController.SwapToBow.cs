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
        public void NormalBowAttack()
        {
            Arrow arrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            arrow.transform.position = spawnPoint.position;
            arrow.transform.forward = spawnPoint.forward;
            arrow.moveSpeed = 10f;
        }

        #endregion Helper Methods
    }
}
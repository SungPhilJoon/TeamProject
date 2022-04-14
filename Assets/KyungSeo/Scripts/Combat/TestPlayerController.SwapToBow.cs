using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class TestPlayerController : PlayerController
    {
        #region Variables
        [Header("활 오브젝트")]
        [SerializeField] private GameObject bowPrefab;

        public Transform spawnPoint;

        public ObjectPoolManager<Arrow> objectPoolManager;

        #endregion Variables

        #region Helper Methods
        

        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalBowAttack()
        {
            animator.SetTrigger(hashOnNormalAttack);
            Arrow arrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            arrow.gameObject.SetActive(true);
            arrow.transform.position = spawnPoint.position;
            arrow.transform.forward = spawnPoint.forward;
            arrow.moveSpeed = 10f;
        }

        public void ExitNormalBowAttack()
        {

        }

        public void EnterSkillBowAttack()
        {

        }

        public void ExitSkillBowAttack()
        {

        }

        #endregion Action Methods
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class MainPlayerController : PlayerController
    {
        #region Variables

        private GameObject bowPrefab = null;

        private Transform spawnPoint;

        public ObjectPoolManager<Arrow> objectPoolManager;
        
        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalBowAttack()
        {
            animator.SetTrigger(hashOnNormalAttack);
            Arrow arrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            arrow.gameObject.SetActive(true);
            arrow.transform.position = spawnPoint.position;
            arrow.transform.forward = spawnPoint.forward;

            //if(!cameraFocus._isAimOn)
            //    arrow.transform.forward = spawnPoint.forward;
            //else
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            //    if (Physics.Raycast(ray, out RaycastHit hit))
            //    {
            //        arrow.transform.forward = (hit.point - arrow.transform.position).normalized;
            //    }
            //}
            
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
        
        public void AimIn(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                StartCoroutine(cameraFocus.AimCameraMove());
            }
        }
        #endregion Action Methods
    }
}
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
        //[SerializeField] private GameObject bowPrefab = null;

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
            
            if(!cameraFocus._isAimOn)
                arrow.transform.forward = spawnPoint.forward;
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    arrow.transform.forward = (hit.point - arrow.transform.position).normalized;
                }
            }
            
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
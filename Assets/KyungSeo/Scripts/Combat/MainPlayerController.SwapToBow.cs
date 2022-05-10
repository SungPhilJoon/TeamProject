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
        [Header("활 데미지")]
        [SerializeField] private int bowNormalDamage;

        private GameObject bowPrefab = null;

        private Transform spawnPoint;

        public ObjectPoolManager<Arrow> objectPoolManager;

        [HideInInspector] public Arrow currentArrow;

        protected readonly int hashDrawBow = Animator.StringToHash("DrawBow");
        
        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalBowAttack()
        {
            animator.SetTrigger(hashOnNormalAttack);
            animator.SetBool(hashDrawBow, true);

            currentArrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            currentArrow.moveSpeed = 10f;
            currentArrow.damage = bowNormalDamage;

            // 여기서 Aim 부분을 구현하면 될 거 같습니다.

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


        }

        public void ExitNormalBowAttack()
        {
            animator.SetBool(hashDrawBow, false);

            // Arrow currentArrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            currentArrow.gameObject.SetActive(true);

            currentArrow.owner = this;
            currentArrow.transform.position = spawnPoint.position;
            currentArrow.transform.forward = spawnPoint.forward;
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
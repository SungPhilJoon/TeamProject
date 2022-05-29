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

        [Header("활 쿨타임")]
        [SerializeField] private float bowNormalCoolTime;
        [SerializeField] private float bowSkill1_CoolTime;

        [Header("활 스킬 공격 영역")]
        public LayerMask groundLayerMask;
        public Skill1_PlaceAreaWithMouse placeArea;

        [Header("Aim")]
        [SerializeField] private Image aimImage;

        private GameObject bowPrefab = null;

        private Transform spawnPoint;

        private Vector3 originalFocusPosition = Vector3.zero;

        [HideInInspector] public ObjectPoolManager<Arrow> objectPoolManager;

        [HideInInspector] public Arrow currentArrow;

        private bool isBowNormal_Available = true;
        private bool isBowSkill1_Available = true;

        protected readonly int hashDrawBow = Animator.StringToHash("DrawBow");

        #endregion Variables

        #region Helper Methods
        private void OnAimCameraMove()
        {
            originalFocusPosition = cameraFocus.transform.localPosition;
            cameraFocus.transform.localPosition = new Vector3(1f, 0f, -3f);
            cameraFocus.transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);
        }

        private void OffAimCameraMove()
        {
            cameraFocus.transform.localPosition = originalFocusPosition;
            cameraFocus.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        private IEnumerator ShowPlaceArea()
        {
            while (true)
            {
                yield return null;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20, groundLayerMask))
                {
                    if (placeArea)
                    {
                        placeArea.SetPosition(hit);
                    }
                }
            }
        }

        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalBowAttack()
        {
            if (!isBowNormal_Available)
            {
                GameManager.Instance.unavailableSkillText.SetActive(true);

                return;
            }

            aimImage.enabled = true;

            OnAimCameraMove();

            animator.SetTrigger(hashOnNormalAttack);
            animator.SetBool(hashDrawBow, true);

            currentArrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            currentArrow.moveSpeed = 10f;
            currentArrow.damage = Damage / 5;
        }

        public void ExitNormalBowAttack()
        {
            aimImage.enabled = false;

            OffAimCameraMove();

            animator.SetBool(hashDrawBow, false);

            // Arrow currentArrow = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfArrow);
            currentArrow.gameObject.SetActive(true);

            currentArrow.owner = this;
            currentArrow.transform.position = spawnPoint.position;
            currentArrow.transform.forward = spawnPoint.forward;

            StartCoroutine(BowNormal_CoolTime());
        }

        public void EnterSkillBowAttack()
        {
            placeArea.gameObject.SetActive(true);
            placeArea.owner = this;
            StartCoroutine(ShowPlaceArea());
        }

        public void ExitSkillBowAttack()
        {
            placeArea.transform.position = new Vector3(960f, 540f, 0f);
            StopCoroutine(ShowPlaceArea());
            placeArea.gameObject.SetActive(false);
        }
        
        public void AimIn(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                StartCoroutine(cameraFocus.AimCameraMove());
            }
        }
        #endregion Action Methods

        #region Cool Time
        private IEnumerator BowNormal_CoolTime()
        {
            isBowNormal_Available = false;

            yield return StartCoroutine(skillCoolTimeHandlers[SkillNameList.BowNormal_Name.GetHashCode()](bowNormalCoolTime));

            isBowNormal_Available = true;
        }

        #endregion Cool Time
    }
}
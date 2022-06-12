using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityChanAdventure.FeelJoon;

namespace UnityChanAdventure.KyungSeo
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
        [SerializeField] private GameObject particleObject;

        [Header("활 스킬 Icon")]
        [SerializeField] private Image[] bowSkill_Icon;

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

        

        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalBowAttack()
        {
            if (!isBowNormal_Available)
            {
                GameManager.Instance.unavailableSkillText.SetActive(true);

                AudioManager.Instance.PlaySFX(
                AudioManager.Instance.uiSFXAudioSource,
                AudioManager.Instance.uiSFXClips,
                "ErrorText");

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
            if (!isBowNormal_Available) return;

            aimImage.enabled = false;

            OffAimCameraMove();

            animator.SetBool(hashDrawBow, false);

            currentArrow.gameObject.SetActive(true);

            currentArrow.owner = this;
            currentArrow.transform.position = spawnPoint.position;
            currentArrow.transform.forward = spawnPoint.forward;

            playerStats.AddMana(-20);

            StartCoroutine(BowNormal_CoolTime());
        }

        public void EnterSkillBowAttack()
        {
            if (!isBowSkill1_Available)
            {
                GameManager.Instance.unavailableSkillText.SetActive(true);

                AudioManager.Instance.PlaySFX(
                AudioManager.Instance.uiSFXAudioSource,
                AudioManager.Instance.uiSFXClips,
                "ErrorText");

                return;
            }

            placeArea.gameObject.SetActive(true);
            placeArea.owner = this;
        }

        public void ExitSkillBowAttack()
        {
            if (!isBowSkill1_Available)
            {
                return;
            }

            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.playerSFXAudioSource,
            AudioManager.Instance.playerSFXClips,
            "BowSkill");

            placeArea.gameObject.SetActive(false);
            GameObject obj = Instantiate(particleObject, placeArea.transform.position + Vector3.up * 9f, Quaternion.identity);
            Destroy(obj, placeArea.duration);

            StartCoroutine(BowSkill1_CoolTime());
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

            yield return StartCoroutine(skillCoolTimeHandlers[SkillNameList.BowNormal_Name.GetHashCode()](bowNormalCoolTime, null));

            isBowNormal_Available = true;
        }

        private IEnumerator BowSkill1_CoolTime()
        {
            isBowSkill1_Available = false;

            yield return StartCoroutine(skillCoolTimeHandlers[SkillNameList.BowSkill1_Name.GetHashCode()](bowSkill1_CoolTime, bowSkill_Icon[0]));

            isBowSkill1_Available = true;
        }

        #endregion Cool Time
    }
}
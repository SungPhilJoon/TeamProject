using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class MainPlayerController : PlayerController
    {
        #region Variables
        private GameObject swordPrefab = null;

        [Header("Ä® µ¥¹ÌÁö")]
        [SerializeField] private int swordNormalDamage = 30;
        [SerializeField] private int swordSkill1_Damage = 50;

        [Header("ÄðÅ¸ÀÓ")]
        public float skill1_CoolTime = 5f;

        [Header("½ºÅ³ ÇÁ¸®ÆÕ")]
        [SerializeField] private Skill1_ShockWave shockWave;

        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");
        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");
        protected readonly int hashSwordSkill = Animator.StringToHash("SwordSkill");

        private bool isSwordSkill1_Available = true;

        #endregion Variables

        #region Helper Methods


        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalSwordAttack()
        {
            Damage = swordNormalDamage;
            animator.SetTrigger(hashOnNormalAttack);

            animator.SetBool(hashIsComboAttack, true);
        }

        public void ExitNormalSwordAttack()
        {
            animator.SetBool(hashIsComboAttack, false);
        }

        public void EnterSkillSwordAttack()
        {
            if (!isSwordSkill1_Available)
            {
                GameManager.Instance.unavailableSkillText.SetActive(true);

                return;
            }

            Instantiate(shockWave.gameObject, spawnPoint.position, spawnPoint.rotation);

            shockWave.transform.forward = spawnPoint.forward;
            shockWave.owner = this;
            shockWave.damage = swordSkill1_Damage;

            animator.SetTrigger(hashSwordSkill);
            isSwordSkill1_Available = false;
            StartCoroutine(Skill1_CoolTime(skill1_CoolTime));

            if (swordPrefab.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                ps.Play();
            }
        }

        public void ExitSkillSwordAttack()
        {
            if (swordPrefab.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                ps.Stop();
            }
        }

        #endregion Action Methods

        #region CoolTime
        private IEnumerator Skill1_CoolTime(float skill1_CoolTime)
        {
            float normalTime = 0f;

            while (skill1_CoolTime > normalTime)
            {
                normalTime += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            isSwordSkill1_Available = true;
        }

        #endregion CoolTime
    }
}
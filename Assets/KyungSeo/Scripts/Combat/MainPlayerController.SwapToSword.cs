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
        [SerializeField] private GameObject swordPrefab = null;

        [Header("Ä® µ¥¹ÌÁö")]
        [SerializeField] private int swordNormalDamage = 30;
        [SerializeField] private int swordSkillDamage = 50;

        private BoxCollider manualCollision;

        [Header("ÄðÅ¸ÀÓ")]
        public float skill1_CoolTime = 5f;

        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");
        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");
        protected readonly int hashSwordSkill = Animator.StringToHash("SwordSkill");
        protected readonly int hashIsOnStopLooping = Animator.StringToHash("IsOnStopLooping");

        #endregion Variables

        #region Helper Methods


        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalSwordAttack()
        {
            manualCollision.enabled = true;
            Damage = swordNormalDamage;
            animator.SetTrigger(hashOnNormalAttack);

            animator.SetBool(hashIsComboAttack, true);
        }

        public void ExitNormalSwordAttack()
        {
            manualCollision.enabled = false;
            animator.SetBool(hashIsComboAttack, false);
        }

        public void EnterSkillSwordAttack()
        {
            manualCollision.enabled = true;
            Damage = swordSkillDamage;
            animator.SetTrigger(hashSwordSkill);
            animator.SetBool(hashIsOnStopLooping, false);

            if (swordPrefab.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                ps.Play();
            }
        }

        public void ExitSkillSwordAttack()
        {
            manualCollision.enabled = false;
            animator.SetBool(hashIsOnStopLooping, true);

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


        }

        #endregion CoolTime
    }
}
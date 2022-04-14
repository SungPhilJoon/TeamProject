using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class TestPlayerController : PlayerController
    {
        #region Variables
        [Header("Ä® ¿ÀºêÁ§Æ®")]
        [SerializeField] private GameObject swordPrefab;

        [Header("Ä® µ¥¹ÌÁö")]
        [SerializeField] private int swordNormalDamage = 30;
        [SerializeField] private int swordSkillDamage = 50;

        [SerializeField] private BoxCollider manualCollision;

        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");
        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");

        #endregion Variables

        #region Helper Methods


        #endregion Helper Methods

        #region Action Methods
        public void EnterNormalSwordAttack()
        {
            manualCollision.enabled = true;
            Damage = swordNormalDamage;
            animator.SetTrigger(hashOnNormalAttack);
        }

        public void EnterNormalComboAttack()
        {
            animator.SetBool(hashIsComboAttack, true);
        }

        public void ExitNormalComboAttack()
        {
            manualCollision.enabled = false;
            animator.SetBool(hashIsComboAttack, false);
        }

        public void EnterSkillSwordAttack()
        {

        }

        public void ExitSkillSwordAttack()
        {

        }

        #endregion Action Methods
    }
}
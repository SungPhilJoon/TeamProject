using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class MainPlayerController : PlayerController
    {
        #region Variables
        private GameObject swordPrefab = null;

        [Header("Į ������")]
        [SerializeField] private int swordNormalDamage = 30;

        [Header("��Ÿ��")]
        public float skill1_CoolTime = 5f;

        [Header("��ų ������")]
        [SerializeField] private Skill1_ShockWave shockWave;

        [Header("Į ��ų Icon")]
        [SerializeField] private Image[] swordSkill_Icon;

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

            shockWave.transform.forward = spawnPoint.forward;
            shockWave.owner = this;

            Instantiate(shockWave.gameObject, spawnPoint.position, spawnPoint.rotation);

            animator.SetTrigger(hashSwordSkill);
            StartCoroutine(SwordSkill1_CoolTime());

            playerStats.AddMana(-10);

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

        #region Cool Time
        private IEnumerator SwordSkill1_CoolTime()
        {
            isSwordSkill1_Available = false;

            yield return StartCoroutine(skillCoolTimeHandlers[SkillNameList.SwordSkill1_Name.GetHashCode()](skill1_CoolTime, swordSkill_Icon[0]));

            isSwordSkill1_Available = true;
        }

        #endregion Cool Time
    }
}
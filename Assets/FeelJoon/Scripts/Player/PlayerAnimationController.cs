using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Variables
        private PlayerController playerController;

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        #endregion Unity Methods

        public void OnExecuteMeleeAttack()
        {
            AudioManager.Instance.PlaySFX(
            AudioManager.Instance.playerSFXAudioSource,
            AudioManager.Instance.playerSFXClips,
            "SwordAttack");

            playerController.OnExecuteMeleeAttack();
        }

        public void OnExecuteProjectileAttack()
        {
            AudioManager.Instance.PlaySFX(
            AudioManager.Instance.playerSFXAudioSource,
            AudioManager.Instance.playerSFXClips,
            "BowAttack");
        }

        public void InstantiateShockWave()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.playerSFXAudioSource,
            AudioManager.Instance.playerSFXClips,
            "SwordSkill");

            GameManager.Instance.Main.InstantiateShockWave();
        }
    }
}
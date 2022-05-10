using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
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
            playerController.OnExecuteMeleeAttack();
        }
    }
}
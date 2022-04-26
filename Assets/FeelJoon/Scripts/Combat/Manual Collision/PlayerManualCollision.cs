using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class PlayerManualCollision : ManualCollision
    {
        #region Variables
        private PlayerController playerController;

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            parent = GameManager.Instance.Player.transform;
            playerController = parent.GetComponent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                playerController.Target = other.transform;
                CheckCollision();
            }
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CheckCollision()
        {
            playerController.OnExecuteMeleeAttack();
        }

        #endregion Helper Methods
    }

}
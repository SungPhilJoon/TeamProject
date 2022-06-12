using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerManualCollision : ManualCollision
    {
        #region Variables
        private PlayerController playerController;

        #endregion Variables

        #region Unity Methods
        void Start()
        {
            parent = GameManager.Instance.Player.transform;

            playerController = parent.GetComponent<PlayerController>();
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CheckCollision()
        {
           targetColliders = Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, playerController.targetMask);
        }

        #endregion Helper Methods
    }

}
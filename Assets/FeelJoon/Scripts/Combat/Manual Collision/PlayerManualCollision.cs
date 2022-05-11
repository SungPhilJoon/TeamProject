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
        //void Awake()
        //{
        //}

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
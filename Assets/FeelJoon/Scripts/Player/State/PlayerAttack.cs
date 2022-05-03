using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ETeam.FeelJoon
{
    public class PlayerAttack : State<PlayerController>
    {
        #region Variables
        private AttackStateController attackStateController;

        #endregion Variables

        public override void OnInitialized()
        {
            attackStateController = context.GetComponent<AttackStateController>();
        }

        public override void OnEnter()
        {
            attackStateController.OnEnterAttackStateHandler();
        }

        public override void Update(float deltaTime)
        {
            
        }

        public override void OnExit()
        {
            attackStateController.OnExitAttackStateHandler();
        }
    }
}

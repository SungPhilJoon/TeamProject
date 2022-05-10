using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class PlayerAttack : State<PlayerController>
    {
        #region Variables
        private AttackStateController attackStateController;
        private MainPlayerController mainPlayerController;

        #endregion Variables

        public override void OnInitialized()
        {
            attackStateController = context.GetComponent<AttackStateController>();
            mainPlayerController = context.GetComponent<MainPlayerController>();
        }

        public override void OnEnter()
        {
            attackStateController.OnEnterAttackStateHandler();
        }

        public override void Update(float deltaTime)
        {
            if (context.currentPlayerWeapon != PlayerWeapon.Bow)
            {
                return;
            }

            if (stateMachine.ElapsedTimeInState < 2.0f)
            {
                mainPlayerController.currentArrow.moveSpeed += deltaTime * 50f;
            }
        }

        public override void OnExit()
        {
            attackStateController.OnExitAttackStateHandler();
        }
    }
}

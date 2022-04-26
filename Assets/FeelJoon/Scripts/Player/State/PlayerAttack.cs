using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ETeam.FeelJoon
{
    public class PlayerAttack : State<PlayerController>
    {
        #region Variables
        private Animator animator;
        private PlayerWeapon playerStance;
        private AttackStateController attackStateController;

        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");
        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
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

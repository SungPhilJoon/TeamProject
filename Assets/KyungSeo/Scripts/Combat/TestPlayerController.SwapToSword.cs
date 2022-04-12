using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ETeam.FeelJoon;
using UnityEngine.Events;

namespace ETeam.KyungSeo
{
    public partial class TestPlayerController : PlayerController
    {
        #region Variables
        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");
        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");

        #endregion Variables

        #region Helper Methods
        public void EnterNormalSwordAttack()
        {
            animator.SetTrigger(hashOnNormalAttack);
        }

        public void EnterNormalComboAttack()
        {
            animator.SetBool(hashIsComboAttack, true);
        }

        public void ExitNormalComboAttack()
        {
            animator.SetBool(hashIsComboAttack, false);
        }

        public void Skill1(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                
            }
        }

        #endregion Helper Methods
    }
}
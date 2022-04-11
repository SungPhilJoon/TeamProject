using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class TestPlayerController : PlayerController
    {
        #region Variables
        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");

        #endregion Variables

        #region Helper Methods
        private void NormalComboAttack(bool isComboAttack)
        {
            animator.SetBool(hashIsComboAttack, isComboAttack);
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
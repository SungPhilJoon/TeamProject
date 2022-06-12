using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UnityChanAdventure.FeelJoon
{
    public class AttackStateController : MonoBehaviour
    {
        #region Actions
        public Action OnEnterAttackStateHandler;
        public Action OnExitAttackStateHandler;

        #endregion Actions

        #region Variables

        #endregion Variables

        #region Unity Methods

        #endregion Unity Methods

        #region Helper Methods
        public void AttackStanceToUsed(PlayerWeapon currentPlayerWeapon,
            Action enterSwordAttack, Action exitSwordAttack,
            Action enterBowAttack, Action exitBowAttack)
        {
            switch (currentPlayerWeapon)
            {
                case PlayerWeapon.Sword:
                    OnEnterAttackStateHandler = enterSwordAttack;
                    OnExitAttackStateHandler = exitSwordAttack;
                    break;
                case PlayerWeapon.Bow:
                    OnEnterAttackStateHandler = enterBowAttack;
                    OnExitAttackStateHandler = exitBowAttack;
                    break;
            }
        }

        #endregion Helper Methods
    }
}
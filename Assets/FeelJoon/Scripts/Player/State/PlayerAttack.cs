using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerAttack : State<PlayerController>
    {
        #region Variables
        private AttackStateController attackStateController;
        private MainPlayerController mainPlayerController;

        #endregion Variables

        #region Properties
        public bool BowSkill1PlaceAreaActive => mainPlayerController.placeArea.gameObject.activeSelf;

        #endregion Properties

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

            #region Bow Attack State Routine
            if (!BowSkill1PlaceAreaActive && (stateMachine.ElapsedTimeInState < 2.0f)) // Bow Stance Normal Attack
            {
                if (mainPlayerController.currentArrow != null)
                {
                    mainPlayerController.currentArrow.moveSpeed += deltaTime * 50f;
                }

                return;
            }

            //if (BowSkill1PlaceAreaActive) // Bow Stance Skill1
            //{
            //    Ray ray = GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

            //    RaycastHit hit;
            //    if (Physics.Raycast(ray, out hit, 20, mainPlayerController.groundLayerMask))
            //    {
            //        if (mainPlayerController.placeArea)
            //        {
            //            mainPlayerController.placeArea.SetPosition(hit);
            //        }
            //    }
            //}

            #endregion Bow Attack State Routine
        }

        public override void OnExit()
        {
            attackStateController.OnExitAttackStateHandler();
        }
    }
}

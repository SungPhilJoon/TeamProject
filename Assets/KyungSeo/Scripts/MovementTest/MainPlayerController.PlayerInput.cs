using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    public partial class MainPlayerController : PlayerController
    {
        #region Variables
        [Header("스킬 퀵슬롯 창")]
        [SerializeField] private GameObject[] skillListSlot;

        private int swordSkillListSlotNumber = 0;
        private int bowSkillListSlotNumber = 1;

        #endregion Variables

        #region Input Methods : Movements

        public void Move(InputAction.CallbackContext callbackContext)
        {
            if (IsAlive && callbackContext.performed) // 키 누르고있으면 이동하도록 value 읽기
            {
                inputValue = callbackContext.ReadValue<Vector2>();
                movement.x = inputValue.x;
                movement.y = 0f;
                movement.z = inputValue.y;
                isMove = true;
                stateMachine.ChangeState<PlayerMove>();
            }

            if (IsAlive && callbackContext.canceled) // 키를 떼면 정지
            {
                movement = Vector2.zero;
                isMove = false;
                stateMachine.ChangeState<PlayerIdle>();
            }
        }

        public void Dash(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                Vector3 dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1 / (drags.x * Time.deltaTime + 1)) / -Time.deltaTime),
                    0,
                    (Mathf.Log(1 / (drags.z * Time.deltaTime + 1)) / -Time.deltaTime)));
                calcVelocity += dashVelocity;
                stateMachine.ChangeState<PlayerDash>();
            }
        }

        #endregion Input Methods : Movements

        #region Input Methods : Attack
        public void Attack(InputAction.CallbackContext callbackContext)
        {
            if (IsAlive)
            {
                AttackInput(currentPlayerWeapon, callbackContext,
                    EnterNormalSwordAttack, ExitNormalSwordAttack,
                    EnterNormalBowAttack, ExitNormalBowAttack);
            }
        }

        public void Skill1(InputAction.CallbackContext callbackContext)
        {
            if (IsAlive)
            {
                AttackInput(currentPlayerWeapon, callbackContext,
                    EnterSkillSwordAttack, ExitSkillSwordAttack,
                    EnterSkillBowAttack, ExitSkillBowAttack);
            }
        }

        private void AttackInput(PlayerWeapon currentPlayerWeapon,
            InputAction.CallbackContext callbackContext,
            Action enterSwordAttack, Action exitSwordAttack,
            Action enterBowAttack, Action exitBowAttack)
        {
            if (!isOnUI && callbackContext.started)
            {
                attackStateController.AttackStanceToUsed(currentPlayerWeapon,
                    enterSwordAttack, exitSwordAttack,
                    enterBowAttack, exitBowAttack);
                stateMachine.ChangeState<PlayerAttack>();
            }
            else if (!isOnUI && callbackContext.canceled)
            {
                stateMachine.ChangeState<PlayerIdle>();
            }
        }

        #endregion Input Methods : Attack

        #region Input Methods : Interact
        public void Interact(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                SetTarget(out target, targetMask);

                if (target != null)
                {
                    IInteractable interactable = target.GetComponent<IInteractable>();
                    interactable?.Interact(this.gameObject);
                    target = null;
                }
            }
        }

        #endregion Interact

        #region Input Methods : Swap

        public void SwapToBow(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (equipment.Slots[(int)ItemType.Bow].SlotItemObject == null)
                {
                    bowPrefab = null;
                }

                SwapWeapon(bowPrefab, PlayerWeapon.Bow);

                if (bowPrefab != null)
                {
                    animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                    playerInput.SwitchCurrentActionMap("PlayerBow");
                }

                skillListSlot[swordSkillListSlotNumber].SetActive(false);
                skillListSlot[bowSkillListSlotNumber].SetActive(true);
            }
        }

        public void SwapToSword(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (equipment.Slots[(int)ItemType.Sword].SlotItemObject == null)
                {
                    swordPrefab = null;
                }

                SwapWeapon(swordPrefab, PlayerWeapon.Sword);

                if (swordPrefab != null)
                {
                    animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                    playerInput.SwitchCurrentActionMap("PlayerSword");
                }

                skillListSlot[swordSkillListSlotNumber].SetActive(true);
                skillListSlot[bowSkillListSlotNumber].SetActive(false);
            }
        }

        public void SwapDefault(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                SwapWeapon(defaultWeaponPrefab, PlayerWeapon.Default);
                animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                playerInput.SwitchCurrentActionMap("Default");
            }

            for (int i = 0; i < skillListSlot.Length; i++)
            {
                skillListSlot[i].SetActive(false);
            }
        }

        #endregion

        #region Input Methods : Shortcut
        public void UseFirstSlotItem(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (shortcut.Slots[0].SlotItemObject == null)
                {
                    return;
                }
                shortcut.UseItem(shortcut.Slots[0]);
            }
        }

        public void UseSecondSlotItem(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (shortcut.Slots[1].SlotItemObject == null)
                {
                    return;
                }
                shortcut.UseItem(shortcut.Slots[1]);
            }
        }

        public void UseThirdSlotItem(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (shortcut.Slots[2].SlotItemObject == null)
                {
                    return;
                }
                shortcut.UseItem(shortcut.Slots[2]);
            }
        }

        public void UseFourthSlotItem(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (shortcut.Slots[3].SlotItemObject == null)
                {
                    return;
                }
                shortcut.UseItem(shortcut.Slots[3]);
            }
        }

        #endregion Input Methods : Shortcut

        #region Input Methods : Call UIs

        public void CallSettings(InputAction.CallbackContext callbackContext)
        {
            if (!isSettingOn)
            {
                isSettingOn = true;
                Time.timeScale = 0;
                settingsUI.gameObject.SetActive(true);
            }
            else
            {
                if (isInventoryOn)
                {
                    isInventoryOn = false;
                    inventoryUI.rectTransform.anchoredPosition = new Vector3(500, -50, 0);
                    // inventoryUI.gameObject.SetActive(false);
                }
                if (isEquipmentOn)
                {
                    isEquipmentOn = false;
                    equipmentUI.rectTransform.anchoredPosition = new Vector3(-500, 0, 0);
                    // equipmentUI.gameObject.SetActive(false);
                }

                isSettingOn = false;
                Time.timeScale = 1;
                settingsUI.gameObject.SetActive(false);
            }
        }

        public void CallInventory(InputAction.CallbackContext callbackContext)
        {
            if (!isInventoryOn)
            {
                isInventoryOn = true;
                inventoryUI.rectTransform.anchoredPosition = new Vector3(-500, -50, 0);
                // inventoryUI.gameObject.SetActive(true);
            }
            else
            {
                isInventoryOn = false;
                inventoryUI.rectTransform.anchoredPosition = new Vector3(500, -50, 0);
                // inventoryUI.gameObject.SetActive(false);
            }
        }

        public void CallEquipment(InputAction.CallbackContext callbackContext)
        {
            if (!isEquipmentOn)
            {
                isEquipmentOn = true;
                equipmentUI.rectTransform.anchoredPosition = new Vector3(500, 0, 0);
                // equipmentUI.gameObject.SetActive(true);
            }
            else
            {
                isEquipmentOn = false;
                equipmentUI.rectTransform.anchoredPosition = new Vector3(-500, 0, 0);
                // equipmentUI.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
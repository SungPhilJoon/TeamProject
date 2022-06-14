using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityChanAdventure.FeelJoon;

namespace UnityChanAdventure.KyungSeo
{
    public partial class MainPlayerController : PlayerController
    {
        #region Variables
        [Header("스킬 퀵슬롯 창")]
        [SerializeField] private GameObject[] skillListSlot;

        [Header("Setting UIs")]
        [SerializeField] private Image settingsUI; // 테스트로 UI(설정창)를 띄우고 끄게 해 보려고
        [SerializeField] private Image inventoryUI; // 캐릭터 인벤토리
        [SerializeField] private Image equipmentUI; // 캐릭터 장비창
        [SerializeField] private Image QuestViewUI; // 퀘스트 창

        // 테스트 UI표시용
        [HideInInspector] public bool isSettingOn = false;
        [HideInInspector] public bool isInventoryOn = false;
        [HideInInspector] public bool isEquipmentOn = false;
        [HideInInspector] public bool isQuestViewOn = false;

        private int swordID = (int)ItemType.Sword;
        private int bowID = (int)ItemType.Bow;

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
                    return;
                }

                target = null;
            }
        }

        #endregion Interact

        #region Input Methods : Swap

        public void SwapToBow(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (equipment.Slots[bowID].SlotItemObject == null)
                {
                    bowPrefab = null;
                }
                else
                {
                    bowPrefab = playerEquipment.itemInstances[bowID].itemTransforms[0].gameObject;
                }

                SwapWeapon(bowPrefab, PlayerWeapon.Bow);

                if (bowPrefab != null)
                {
                    animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                    playerInput.SwitchCurrentActionMap("PlayerBow");

                    skillListSlot[swordID].SetActive(false);
                    skillListSlot[bowID].SetActive(true);
                }
            }
        }

        public void SwapToSword(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (equipment.Slots[swordID].SlotItemObject == null)
                {
                    swordPrefab = null;
                }
                else
                {
                    swordPrefab = playerEquipment.itemInstances[swordID].itemTransforms[0].gameObject;
                }

                SwapWeapon(swordPrefab, PlayerWeapon.Sword);

                if (swordPrefab != null)
                {
                    animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                    playerInput.SwitchCurrentActionMap("PlayerSword");

                    skillListSlot[swordID].SetActive(true);
                    skillListSlot[bowID].SetActive(false);
                }
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

        public void CallQuestView(InputAction.CallbackContext callbackContext)
        {
            if (!isQuestViewOn)
            {
                isQuestViewOn = true;
                QuestViewUI.gameObject.SetActive(true);
            }
            else
            {
                isQuestViewOn = false;
                QuestViewUI.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
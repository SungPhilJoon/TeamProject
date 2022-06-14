using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityChanAdventure.FeelJoon;
using UnityChanAdventure.YongHak;
using UnityEngine.EventSystems;

namespace UnityChanAdventure.KyungSeo
{
    [RequireComponent(typeof(CharacterController))]
    public partial class MainPlayerController : PlayerController
    {
        #region Variables
        [Header("이동속도")]
        public float moveSpeed; // 이동 스피드
        [SerializeField] private float dashDistance = 5.0f; // 대쉬 거리 - PJ

        private Vector2 inputValue = Vector2.zero; // 입력 Vector
        private Vector3 movement = Vector3.zero; // 이동 방향 Vector

        [Header("카메라")]
        public Transform focus;
        private ClickDragCamera cameraFocus;

        private bool isOnUI = false;

        [Header("저항 계수")]
        public float gravity = -29.81f; // 중력 계수 : rigidbody를 사용하지 않기 위한 중력계수
        public Vector3 drags; // 저항력

        private bool isGround = false;

        private Vector3 calcVelocity; // 계산에 사용될 Vector3 레퍼런스

        private Dictionary<int, Func<float, Image, IEnumerator>> skillCoolTimeHandlers = new Dictionary<int, Func<float, Image, IEnumerator>>();

        private PlayerInput playerInput;

        [Header("에러 텍스트")]
        public GameObject weaponErrorText;

        [Header("전투")]
        public AttackStateController attackStateController;
        [SerializeField] private GameObject defaultWeaponPrefab;
        private GameObject previousWeapon;
        private GameObject equipmentWeapon;
        // 인벤토리
        [Header("인벤토리")]
        public InventoryObject inventory;
        public InventoryObject equipment;
        public InventoryObject shortcut;
        private PlayerEquipment playerEquipment;

        #endregion Variables

        #region Properties
        public PlayerInput PlayerInput => playerInput;

        #endregion Properties

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            attackStateController = GetComponent<AttackStateController>();

            cameraFocus = focus.GetComponentInChildren<ClickDragCamera>();

            spawnPoint = transform.GetChild(transform.childCount - 1);

            objectPoolManager = new ObjectPoolManager<Arrow>(PooledObjectNameList.NameOfArrow, spawnPoint);

            playerEquipment = GetComponent<PlayerEquipment>();

            playerInput.SwitchCurrentActionMap("Default");

            equipmentWeapon = defaultWeaponPrefab;

            inventory.OnUseItem -= OnUseItem;
            inventory.OnUseItem += OnUseItem;
            shortcut.OnUseItem -= OnUseItem;
            shortcut.OnUseItem += OnUseItem;
        }

        void Start()
        {
            skillCoolTimeHandlers.Add(SkillNameList.SwordSkill1_Name.GetHashCode(), Skill_CoolTime);
            skillCoolTimeHandlers.Add(SkillNameList.BowNormal_Name.GetHashCode(), Skill_CoolTime);
            skillCoolTimeHandlers.Add(SkillNameList.BowSkill1_Name.GetHashCode(), Skill_CoolTime);

            for(int i = 0; i < swordSkill_Icon.Length; i++)
            {
                swordSkill_Icon[i].fillAmount = 1;
                bowSkill_Icon[i].fillAmount = 1;
            }
        }


        protected override void Update()
        {
            isOnUI = EventSystem.current.IsPointerOverGameObject();

            isGround = controller.isGrounded;
            if (isGround && calcVelocity.y < 0)
            {
                calcVelocity.y = 0;
            }

            if (isMove)
            {
                Vector3 lookForward = new Vector3(focus.forward.x, 0f, focus.forward.z).normalized;
                Vector3 lookRight = new Vector3(focus.right.x, 0f, focus.right.z).normalized;
                Vector3 moveDir = lookForward * movement.z + lookRight * movement.x;

                transform.forward = Vector3.Lerp(transform.forward, moveDir, 30f * Time.deltaTime);
                controller.Move(moveDir * Time.deltaTime * moveSpeed);
            }

            calcVelocity.y += gravity * Time.deltaTime;

            calcVelocity.x /= 1 + drags.x * Time.deltaTime;
            calcVelocity.y /= 1 + drags.y * Time.deltaTime;
            calcVelocity.z /= 1 + drags.z * Time.deltaTime;

            if (IsAlive)
            { 
                controller.Move(calcVelocity * Time.deltaTime); 
            }

            base.Update();
        }

        #endregion

        #region Inventory
        public bool PickupItem(PickupItem pickupItem, int amount = 1)
        {
            if (pickupItem.itemObject != null && inventory.AddItem(new Item(pickupItem.itemObject), amount))
            {
                Destroy(pickupItem.gameObject);
                return true;
            }

            return false;
        }

        private void OnUseItem(ItemObject itemObject)
        {
            foreach(ItemBuff buff in itemObject.data.buffs)
            {
                if (buff.stat == CharacterAttribute.Health)
                {
                    playerStats.AddHealth(buff.value);
                }
                else if (buff.stat == CharacterAttribute.Mana)
                {
                    playerStats.AddMana(buff.value);
                }
            }
        }

        #endregion Inventory

        #region Helper Methods
        
        private void SetTarget(out Transform newTarget, LayerMask targetMask, float distance = 3.0f)
        {
            Collider[] targetColliders = Physics.OverlapSphere(transform.position, distance, targetMask);

            foreach (Collider targetCollider in targetColliders)
            {
                if (targetCollider.TryGetComponent(out IInteractable interactable))
                {
                    newTarget = targetCollider.transform;
                    return;
                }
            }

            newTarget = null;
        }
        
        /// <summary>
        /// 무기를 스왑해주는 함수입니다. 만약 스왑할 무기가 장착되어 있지 않으면 에러 텍스트를 보여줍니다.
        /// </summary>
        /// <param name="weaponToSwap"></param>
        /// <param name="changedPlayerWeapon"></param>
        public void SwapWeapon(GameObject weaponToSwap, PlayerWeapon changedPlayerWeapon)
        {
            try
            {
                previousWeapon = equipmentWeapon;

                if (weaponToSwap == null)
                {
                    throw new NullReferenceException();
                }

                equipmentWeapon = weaponToSwap;
                currentPlayerWeapon = changedPlayerWeapon;

                AudioManager.Instance.PlayForceSFX(
                AudioManager.Instance.playerSFXAudioSource,
                AudioManager.Instance.playerSFXClips,
                "PlayerWeaponChange");
            }
            catch(NullReferenceException e)
            {
                AudioManager.Instance.PlaySFX(
                AudioManager.Instance.uiSFXAudioSource,
                AudioManager.Instance.uiSFXClips,
                "ErrorText");

                weaponErrorText.SetActive(true);
            }
            finally
            {
                previousWeapon?.SetActive(false);
                equipmentWeapon?.SetActive(true);
            }
        }

        public void ChangePlayerWeaponToDefalut()
        {
            AudioManager.Instance.PlaySFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "ErrorText");

            weaponErrorText.SetActive(true);

            SwapWeapon(defaultWeaponPrefab, PlayerWeapon.Default);
            animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
            playerInput.SwitchCurrentActionMap("Default");

            for (int i = 0; i < skillListSlot.Length; i++)
            {
                skillListSlot[i].SetActive(false);
            }
        }

        private IEnumerator Skill_CoolTime(float skill_CoolTime, Image skillIcon)
        {
            float normalTime = 0f;

            while (skill_CoolTime > normalTime)
            {
                normalTime += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();

                if (skillIcon == null)
                {
                    continue;
                }

                skillIcon.fillAmount = normalTime / skill_CoolTime;
            }
        }

        #endregion Helper Methods

        #region Shop

        public bool Enter(RectTransform uiGroup)
        {
            uiGroup.anchoredPosition = Vector3.zero;
            return uiGroup.gameObject.activeSelf;
        }

        /*public bool Exit(RectTransform uiGroup)
        {
            uiGroup.anchoredPosition = Vector3.down * 2000;
        }*/

        #endregion Shop
    }
}
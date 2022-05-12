using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ETeam.FeelJoon;
using ETeam.YongHak;
using UnityEngine.EventSystems;

namespace ETeam.KyungSeo
{
    [RequireComponent(typeof(CharacterController))]
    public partial class MainPlayerController : PlayerController
    {
        #region Variables
        [Header("이동속도")]
        public float moveSpeed; // 이동 스피드
        [SerializeField] private float dashDistance = 5.0f; // 대쉬 거리 - PJ

        [Header("Setting UIs")]
        [SerializeField] private Image settingsUI; // 테스트로 UI(설정창)를 띄우고 끄게 해 보려고
        [SerializeField] private Image inventoryUI; // 캐릭터 인벤토리
        [SerializeField] private Image equipmentUI; // 캐릭터 장비창

        private Vector2 inputValue = Vector2.zero; // 입력 Vector
        private Vector3 movement = Vector3.zero; // 이동 방향 Vector

        [Header("카메라")]
        public Transform focus;
        private Camera camera;
        private ClickDragCamera cameraFocus;

        [HideInInspector] public bool isSettingOn = false; // 테스트 UI표시용
        [HideInInspector] public bool isInventoryOn = false;
        [HideInInspector] public bool isEquipmentOn = false;

        private bool isOnUI = false;

        [HideInInspector] public CharacterController controller; // 캐싱할 CharacterController - PJ

        [Header("저항 계수")]
        public float gravity = -29.81f; // 중력 계수 : rigidbody를 사용하지 않기 위한 중력계수
        public Vector3 drags; // 저항력

        private bool isGround = false;

        private Vector3 calcVelocity; // 계산에 사용될 Vector3 레퍼런스 - PJ

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
        private PlayerEquipment playerEquipment;

        #endregion Variables

        #region Properties
        public override int Damage
        {
            get => damage;
            protected set => damage = value;
        }

        public PlayerInput PlayerInput => playerInput;

        #endregion Properties

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            attackStateController = GetComponent<AttackStateController>();
            camera = Camera.main;

            spawnPoint = transform.GetChild(transform.childCount - 1);

            objectPoolManager = new ObjectPoolManager<Arrow>(PooledObjectNameList.NameOfArrow, spawnPoint);

            playerEquipment = GetComponent<PlayerEquipment>();

            playerInput.SwitchCurrentActionMap("Default");

            equipmentWeapon = defaultWeaponPrefab;

            inventory.OnUseItem -= OnUseItem;
            inventory.OnUseItem += OnUseItem;
        }

        void Start()
        {
            StartCoroutine(CheckEquipWeapon());
        }


        protected override void Update()
        {
            isOnUI = EventSystem.current.IsPointerOverGameObject();

            isGround = controller.isGrounded;
            if (isGround && calcVelocity.y < 0)
            {
                calcVelocity.y = 0;
            }
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
                    this.health += buff.value;
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
        private void SwapWeapon(GameObject weaponToSwap, PlayerWeapon changedPlayerWeapon)
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
            }
            catch(NullReferenceException e)
            {
                Debug.Log("장착된 무기가 없습니다.");

                weaponErrorText.SetActive(true);

                animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
            }
            finally
            {
                if (previousWeapon)

                previousWeapon.SetActive(false);
                equipmentWeapon.SetActive(true);
            }
        }

        /// <summary>
        /// 매 시간 호출하고 체크하여 무기를 업데이트 해주는 함수입니다.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckEquipWeapon()
        {
            swordPrefab = playerEquipment.itemInstances[(int)ItemType.Sword].itemTransforms[0]?.gameObject;
            bowPrefab = playerEquipment.itemInstances[(int)ItemType.Bow].itemTransforms[0].gameObject;

            while (true)
            {
                yield return null;

                if (currentPlayerWeapon == PlayerWeapon.Default)
                {
                    swordPrefab?.SetActive(false);
                    bowPrefab?.SetActive(false);
                }

                if (currentPlayerWeapon == PlayerWeapon.Sword)
                {
                    bowPrefab?.SetActive(false);
                    swordPrefab.SetActive(true);

                    try
                    {
                        if (equipment.Slots[(int)ItemType.Sword].SlotItemObject == null)
                        {
                            throw new NullReferenceException();
                        }
                    }
                    catch(NullReferenceException e)
                    {
                        Debug.Log("장착된 무기가 없습니다.");
                        weaponErrorText.SetActive(true);

                        SwapWeapon(defaultWeaponPrefab, PlayerWeapon.Default);
                        animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                        playerInput.SwitchCurrentActionMap("Default");
                    }
                }

                if (currentPlayerWeapon == PlayerWeapon.Bow)
                {
                    swordPrefab?.SetActive(false);
                    bowPrefab?.SetActive(true);

                    try
                    {
                        if (equipment.Slots[(int)ItemType.Bow].SlotItemObject == null)
                        {
                            throw new NullReferenceException();
                        }
                    }
                    catch(NullReferenceException e)
                    {
                        Debug.Log("장착된 무기가 없습니다.");
                        weaponErrorText.SetActive(true);

                        SwapWeapon(defaultWeaponPrefab, PlayerWeapon.Default);
                        animator.SetInteger(hashSwapIndex, (int)currentPlayerWeapon);
                        playerInput.SwitchCurrentActionMap("Default");
                    }
                }

                swordPrefab = playerEquipment.itemInstances[(int)ItemType.Sword].itemTransforms[0].gameObject;
                bowPrefab = playerEquipment.itemInstances[(int)ItemType.Bow].itemTransforms[0].gameObject;
            }
        }

        #endregion Helper Methods

        #region Shop

        public bool Enter(RectTransform uiGroup)
        {
            Debug.Log("체크1");
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
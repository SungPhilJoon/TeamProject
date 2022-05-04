using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ETeam.KyungSeo;
using UnityEngine.SceneManagement;

namespace ETeam.FeelJoon
{
    public class PlayerEquipment : MonoBehaviour
    {
        #region Variables
        public InventoryObject equipment;

        private EquipmentCombiner combiner;

        public ItemInstances[] itemInstances = new ItemInstances[2];

        public ItemObject[] defaultItemObjects = new ItemObject[2];

        private Transform myTransform;

        [HideInInspector]
        public BoxCollider manualCollision;

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            combiner = new EquipmentCombiner(gameObject);
            myTransform = GetComponent<Transform>();

            for (int i = 0; i < equipment.Slots.Length; i++)
            {
                equipment.Slots[i].OnPreUpdate -= OnRemoveItem;
                equipment.Slots[i].OnPreUpdate += OnRemoveItem;
                equipment.Slots[i].OnPostUpdate -= OnEquipItem;
                equipment.Slots[i].OnPostUpdate += OnEquipItem;
            }
        }

        void Start()
        {
            foreach (InventorySlot slot in equipment.Slots)
            {
                OnEquipItem(slot);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < equipment.Slots.Length; i++)
            {
                equipment.Slots[i].OnPreUpdate -= OnRemoveItem;
                equipment.Slots[i].OnPostUpdate -= OnEquipItem;
            }

            //    foreach (ItemInstances item in itemInstances)
            //    {
            //        Destroy(item.gameObject);
            //    }
        }

        #endregion Unity Methods

        #region Helper Methods
        private void EquipDefaultItemBy(ItemType type)
        {
            int index = (int)type;

            ItemObject itemObject = defaultItemObjects[index];

            switch (type)
            {
                //case ItemType.Helmet:
                //case ItemType.Chest:
                //case ItemType.Pants:
                //case ItemType.Boots:
                //case ItemType.Gloves:
                //    itemInstances[index] = EquipSkinnedItem(itemObject);
                //    break;

                //case ItemType.Pauldrons:
                case ItemType.Sword:
                case ItemType.Bow:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }

            // GetComponent<PlayerController>().animator.Rebind();
        }

        private void RemoveItemBy(ItemType type)
        {
            int index = (int)type;
            if (itemInstances[index] != null)
            {
                Destroy(itemInstances[index].gameObject);
                itemInstances[index] = null;
            }
        }

        private void OnRemoveItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.SlotItemObject;
            if (itemObject == null)
            {
                // destroy default items
                RemoveItemBy(slot.allowedItems[0]);
                return;
            }

            if (itemObject.modelPrefab != null)
            {
                RemoveItemBy(slot.allowedItems[0]);
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.SlotItemObject;
            if (itemObject == null)
            {
                EquipDefaultItemBy(slot.allowedItems[0]);
                return;
            }

            int index = (int)slot.allowedItems[0];

            switch (slot.allowedItems[0])
            {
                //case ItemType.Helmet:
                //case ItemType.Chest:
                //case ItemType.Pants:
                //case ItemType.Boots:
                //case ItemType.Gloves:
                //    itemInstances[index] = EquipSkinnedItem(itemObject);
                //    break;

                // case ItemType.Pauldrons:
                case ItemType.Sword:
                case ItemType.Bow:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }

            //if (itemInstances[index] != null)
            //{
            //    itemInstances[index].name = slot.allowedItems[0].ToString();
            //}

            // GetComponent<PlayerController>().animator.Rebind();
        }

        private ItemInstances EquipSkinnedItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Transform itemTransform = combiner.AddLimb(itemObject.modelPrefab, itemObject.boneNames);

            if (itemTransform != null)
            {
                ItemInstances instance = new ItemInstances();
                instance.itemTransforms.Add(itemTransform);

                return instance;
            }

            return null;
        }

        private ItemInstances EquipMeshItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Transform[] itemTransforms = combiner.AddMesh(itemObject.modelPrefab);
            if (itemTransforms.Length > 0)
            {
                Debug.Log("¿Ö ³ª¿Í");
                ItemInstances instance = new GameObject().AddComponent<ItemInstances>();
                instance.itemTransforms.AddRange(itemTransforms.ToList<Transform>());
                instance.transform.parent = myTransform;
                manualCollision = instance.itemTransforms[0].GetComponent<BoxCollider>();

                return instance;
            }

            return null;
        }

        #endregion Helper Methods
    }
}

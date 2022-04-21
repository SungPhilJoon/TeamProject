using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class PlayerEquipment : MonoBehaviour
    {
        #region Variables
        public InventoryObject equipment;

        private EquipmentCombiner combiner;

        private ItemInstances[] itemInstances = new ItemInstances[2];

        public ItemObject[] defaultItemObjects = new ItemObject[2];

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            combiner = new EquipmentCombiner(gameObject);

            for (int i = 0; i < equipment.Slots.Length; i++)
            {
                equipment.Slots[i].OnPreUpdate += OnRemoveItem;
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

        //private void OnDestroy()
        //{
        //    foreach(ItemInstances item in itemInstances)
        //    {
        //        item.Destroy();
        //    }
        //}

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
                case ItemType.LeftWeapon:
                case ItemType.RightWeapon:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }

            GetComponent<PlayerController>().animator.Rebind();
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
            ItemObject itemObject = slot.ItemObject;
            if (itemObject == null)
            {

                // destroy default items
                RemoveItemBy(slot.allowedItems[0]);
                return;
            }

            if (slot.ItemObject.modelPrefab != null)
            {
                RemoveItemBy(slot.allowedItems[0]);
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.ItemObject;
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
                case ItemType.LeftWeapon:
                case ItemType.RightWeapon:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }

            if (itemInstances[index] != null)
            {
                itemInstances[index].name = slot.allowedItems[0].ToString();
            }

            GetComponentInChildren<PlayerController>().animator.Rebind();
        }

        private ItemInstances EquipSkinnedItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Debug.Log(itemObject.modelPrefab.ToString());
            Debug.Log(itemObject.boneNames.ToString());
            Transform itemTransform = combiner.AddLimb(itemObject.modelPrefab, itemObject.boneNames);

            if (itemTransform != null)
            {
                ItemInstances instance = itemTransform.gameObject.AddComponent<ItemInstances>();
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
                ItemInstances instance = new GameObject().AddComponent<ItemInstances>();
                instance.itemTransforms.AddRange(itemTransforms.ToList<Transform>());
                instance.transform.parent = transform;

                return instance;
            }

            return null;
        }

        #endregion Helper Methods
    }
}

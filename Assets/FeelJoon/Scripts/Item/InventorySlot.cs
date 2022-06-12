using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [Serializable]
    public class InventorySlot
    {
        #region Variables
        public ItemType[] allowedItems = new ItemType[0];

        [NonSerialized] public InventoryObject parent;
        [NonSerialized] public GameObject slotUI;
        [NonSerialized] public RectTransform slotRectTransform;

        [NonSerialized] public Action<InventorySlot> OnPreUpdate;
        [NonSerialized] public Action<InventorySlot> OnPostUpdate;

        public Item item;
        public int amount;

        public ItemObject SlotItemObject
        {
            get
            {
                return item.id >= 0 ? GetItemObject(item.itemType, item.id)
                    : null;
            }
        }

        public InventorySlot() => UpdateSlot(new Item(), 0);
        public InventorySlot(Item item, int amount) => UpdateSlot(item, amount);

        #endregion Variables

        #region Helper Methods
        public void AddItem(Item item, int amount) => UpdateSlot(item, amount);
        public void RemoveItem() => UpdateSlot(new Item(), 0);

        public void AddAmount(int value) => UpdateSlot(item, amount += value);

        public void UpdateSlot(Item item, int amount)
        {
            if (amount <= 0)
            {
                item = new Item();
            }

            OnPreUpdate?.Invoke(this);

            this.item = item;
            this.amount = amount;

            OnPostUpdate?.Invoke(this);
        }

        public bool CanPlaceInSlot(ItemObject itemObject)
        {
            if (allowedItems.Length <= 0 || itemObject == null || itemObject.data.id < 0)
            {
                return true;
            }

            foreach (ItemType type in allowedItems)
            {
                if (itemObject.type == type)
                {
                    return true;
                }
            }

            return false;
        }

        public ItemObject GetItemObject(ItemType itemType, int id)
        {
            if (itemType.Equals(ItemType.Sword))
            {
                return parent.database[(int)ItemType.Sword].itemObjects[id - itemCodeList.swordCode];
            }
            else if (itemType.Equals(ItemType.Bow))
            {
                return parent.database[(int)ItemType.Bow].itemObjects[id - itemCodeList.bowCode];
            }
            else if (itemType.Equals(ItemType.Food))
            {
                return parent.database[(int)ItemType.Food].itemObjects[id - itemCodeList.foodCode];
            }

            return null;
        }

        #endregion Helper Methods
    }
}
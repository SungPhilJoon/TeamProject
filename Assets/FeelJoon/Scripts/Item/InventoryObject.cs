using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public enum InterfaceType
    {
        Inventory,
        Equipment,
        QuickSlot,
        Box,
    }

    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public ItemObjectDatabase[] database;
        public InterfaceType type;

        [SerializeField] private Inventory container = new Inventory();

        public Action<ItemObject> OnUseItem;

        public InventorySlot[] Slots => container.slots;

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                foreach (InventorySlot slot in Slots)
                {
                    if (slot.item.id < 0)
                    {
                        counter++;
                    }
                }

                return counter;
            }
        }

        #region Helper Methods
        public void Clear()
        {
            container.Clear();
        }

        public bool AddItem(Item item, int amount)
        {
            if (EmptySlotCount <= 0)
            {
                return false;
            }

            InventorySlot slot = FindItemInInventory(item);

            int itemType = (int)item.itemType;

            if (!database[itemType].itemObjects[item.id - itemType * 100].stackable || slot == null)    
            {
                GetEmptySlot().AddItem(item, amount);
            }
            else
            {
                slot.AddAmount(amount);
            }

            return true;
        }

        public InventorySlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        public InventorySlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id < 0);
        }

        public bool IsContainItem(ItemObject itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        public void SwapItems(InventorySlot itemSlotA, InventorySlot itemSlotB)
        {
            if (itemSlotA == itemSlotB)
            {
                return;
            }

            if (itemSlotB.CanPlaceInSlot(itemSlotA.SlotItemObject) && itemSlotA.CanPlaceInSlot(itemSlotB.SlotItemObject))
            {
                //InventorySlot tempSlot = new InventorySlot(itemSlotB.item, itemSlotB.amount);
                Item tempItem = itemSlotB.item;
                int tempAmount = itemSlotB.amount;
                itemSlotB.UpdateSlot(itemSlotA.item, itemSlotA.amount);
                itemSlotA.UpdateSlot(tempItem, tempAmount);
            }
        }

        public void UseItem(InventorySlot slotToUse)
        {
            if (slotToUse.SlotItemObject == null || slotToUse.item.id < 0 || slotToUse.amount <= 0)
            {
                return;
            }

            ItemObject itemObject = slotToUse.SlotItemObject;
            slotToUse.UpdateSlot(slotToUse.item, slotToUse.amount - 1);

            OnUseItem.Invoke(itemObject);
        }

        #endregion Helper Methods
    }
}

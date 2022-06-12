using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [Serializable]
    public class Inventory
    {
        #region Variables
        public InventorySlot[] slots = new InventorySlot[24];

        #endregion Variables

        #region Helper Methods
        public void Clear()
        {
            foreach (InventorySlot slot in slots)
            {
                slot.RemoveItem();
            }
        }

        public bool IsContain(ItemObject itemObject)
        {
            return IsContain(itemObject.data.id);
            // return Array.Find(slots, i => i.item.id == itemObject.data.id) != null;
        }

        public bool IsContain(int id)
        {
            return slots.FirstOrDefault(i => i.item.id == id) != null;
        }

        #endregion Helper Methods
    }
}
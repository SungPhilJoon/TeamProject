using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [RequireComponent(typeof(EventTrigger))]
    public class ShortcutInventory : InventoryUI
    {
        #region Variables
        public GameObject[] shortcutSlots = null;

        #endregion Variables

        #region Helper Methods
        public override void CreateSlotUIs()
        {
            slotUIs = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject go = shortcutSlots[i];

                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnterSlot(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExitSlot(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });

                inventoryObject.Slots[i].slotUI = go;
                inventoryObject.Slots[i].slotRectTransform = go.GetComponent<RectTransform>();
                slotUIs.Add(go, inventoryObject.Slots[i]);
            }
        }

        #endregion Helper Methods
    }
}
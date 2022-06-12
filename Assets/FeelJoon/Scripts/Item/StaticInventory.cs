using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [RequireComponent(typeof(EventTrigger))]
    public class StaticInventory : InventoryUI
    {
        #region Variables
        public GameObject[] staticSlots = null;

        #endregion Variables

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-500, 0, 0);
            // gameObject.SetActive(false);
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CreateSlotUIs()
        {
            slotUIs = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject go = staticSlots[i];

                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnterSlot(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExitSlot(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });

                inventoryObject.Slots[i].slotUI = go;
                inventoryObject.Slots[i].slotRectTransform = go.GetComponent<RectTransform>();
                slotUIs.Add(go, inventoryObject.Slots[i]);

                go.name += $": {i}";
            }
        }

        #endregion Helper Methods
    }
}

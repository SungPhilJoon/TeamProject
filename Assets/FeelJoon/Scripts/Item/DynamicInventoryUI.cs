using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;
using System;
using System.Linq;

namespace UnityChanAdventure.FeelJoon
{
    public class DynamicInventoryUI : InventoryUI
    {
        #region Variables
        [SerializeField] protected GameObject slotPrefab;

        [SerializeField] protected Vector2 start;

        [SerializeField] protected Vector2 size;

        [SerializeField] protected Vector2 space;

        [Min(1), SerializeField] protected int numberOfColum = 4;

        #endregion Variables

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(500, -50, 0);
            // gameObject.SetActive(false);
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CreateSlotUIs()
        {
            slotUIs = new Dictionary<GameObject, InventorySlot>();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject go = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, transform);
                go.GetComponent<RectTransform>().anchoredPosition = CalculatePosition(i);

                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnterSlot(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExitSlot(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
                AddEvent(go, EventTriggerType.PointerClick, (data) => { OnClick(go, (PointerEventData)data); });

                inventoryObject.Slots[i].slotUI = go;
                inventoryObject.Slots[i].slotRectTransform = go.GetComponent<RectTransform>();
                slotUIs.Add(go, inventoryObject.Slots[i]);

                go.name += $": {i}";
            }
        }

        public Vector3 CalculatePosition(int i)
        {
            float x = start.x + ((space.x + size.x) * (i % numberOfColum));
            float y = start.y + (-(space.y + size.y) * (i / numberOfColum));

            return new Vector3(x, y, 0f);
        }

        protected override void OnRightClick(InventorySlot slot)
        {
            inventoryObject.UseItem(slot);
        }

        public void TrimAll()
        {
            InventorySlot emptySlot = inventoryObject.GetEmptySlot();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                if (inventoryObject.Slots[i].SlotItemObject != null &&
                    inventoryObject.IsContainItem(inventoryObject.Slots[i].SlotItemObject))
                {
                    inventoryObject.SwapItems(emptySlot, inventoryObject.Slots[i]);
                    if (i + 1 == inventoryObject.Slots.Length)
                    {
                        break;
                    }
                    emptySlot = inventoryObject.GetEmptySlot();
                }
            }
        }

        public void SortAll()
        {
            TrimAll();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                if (inventoryObject.Slots[i].SlotItemObject == null)
                {
                    break;
                }

                for (int j = i + 1; j < inventoryObject.Slots.Length; j++)
                {
                    if (inventoryObject.Slots[i].item.id < inventoryObject.Slots[j].item.id)
                    {
                        inventoryObject.SwapItems(inventoryObject.Slots[i], inventoryObject.Slots[j]);
                    }
                }
            }

            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");
        }

        #endregion Helper Methods
    }
}
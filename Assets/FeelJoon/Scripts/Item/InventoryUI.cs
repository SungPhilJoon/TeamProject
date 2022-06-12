using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public static class MouseData
    {
        public static InventoryUI interfaceMouseIsOver;
        public static GameObject slotHoveredOver;
        public static GameObject tempItemBeingDragged;
    }

    [RequireComponent(typeof(EventTrigger))]
    public abstract class InventoryUI : MonoBehaviour
    {
        public InventoryObject inventoryObject;
        private InventoryObject previousInventoryObject;

        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();

        public ItempTooltipUI itemTooltipUI;

        #region Unity Methods
        protected virtual void Awake()
        {
            CreateSlotUIs();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].parent = inventoryObject;
                inventoryObject.Slots[i].OnPreUpdate -= OnPreUpdate;
                inventoryObject.Slots[i].OnPreUpdate += OnPreUpdate;
                inventoryObject.Slots[i].OnPostUpdate -= OnPostUpdate;
                inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;
            }

            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
            AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        }

        protected virtual void Start()
        {
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                if (inventoryObject.Slots[i].amount <= 0)
                {
                    inventoryObject.Slots[i].item = new Item();
                }

                // OnPostUpdate(inventoryObject.Slots[i]);

                inventoryObject.Slots[i].UpdateSlot(inventoryObject.Slots[i].item, inventoryObject.Slots[i].amount);
            }
        }

        #endregion Unity Methods

        #region Helper Methods
        public abstract void CreateSlotUIs();

        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                Debug.LogWarning("No EventTrigger component found!");
                return;
            }

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public void OnPreUpdate(InventorySlot slot)
        {

        }

        public void OnPostUpdate(InventorySlot slot)
        {
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite = slot.item.id < 0 ? null : slot.SlotItemObject.icon;
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().color = slot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
            slot.slotUI.GetComponentInChildren<TextMeshProUGUI>().text = slot.item.id < 0 ? string.Empty : (slot.amount == 1 ? string.Empty : slot.amount.ToString("n0"));
        }

        public void OnEnterInterface(GameObject go)
        {
            MouseData.interfaceMouseIsOver = go.GetComponentInParent<InventoryUI>();
        }

        public void OnExitInterface(GameObject go)
        {
            MouseData.interfaceMouseIsOver = null;
        }

        public void OnEnterSlot(GameObject go)
        {
            MouseData.slotHoveredOver = go;
            MouseData.slotHoveredOver.GetComponent<Image>().color = Color.yellow;

            if (slotUIs[go] != null)
            {
                ShowOrHideItemTooltip();
            }
        }

        public void OnExitSlot(GameObject go)
        {
            MouseData.slotHoveredOver = null;
            go.GetComponent<Image>().color = new Color(184f / 255f, 152f / 255f, 109f / 255f, 1f);

            ShowOrHideItemTooltip();
        }

        public void OnStartDrag(GameObject go)
        {
            MouseData.tempItemBeingDragged = CreateDragImage(go);
        }

        private GameObject CreateDragImage(GameObject go)
        {
            if (slotUIs[go].item.id < 0)
            {
                return null;
            }

            GameObject dragImageGo = new GameObject();
            RectTransform rectTransform = dragImageGo.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);
            dragImageGo.transform.SetParent(transform.parent);

            Image image = dragImageGo.AddComponent<Image>();
            image.sprite = slotUIs[go].SlotItemObject.icon;
            image.raycastTarget = false;

            dragImageGo.name = "Drag Image";

            return dragImageGo;
        }

        public void OnDrag(GameObject go)
        {
            if (MouseData.tempItemBeingDragged == null)
            {
                return;
            }

            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        public void OnEndDrag(GameObject go)
        {
            Destroy(MouseData.tempItemBeingDragged);

            if (MouseData.interfaceMouseIsOver == null)
            {
                slotUIs[go].RemoveItem();
            }
            else if (MouseData.slotHoveredOver != null)
            {
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];
                inventoryObject.SwapItems(slotUIs[go], mouseHoverSlotData);
            }
        }

        public void OnClick(GameObject go, PointerEventData data)
        {
            InventorySlot slot = slotUIs[go];

            if (slot == null)
            {
                return;
            }

            if (data.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick(slot);
            }
            else if (data.button == PointerEventData.InputButton.Right)
            {
                OnRightClick(slot);
            }
        }

        protected virtual void OnRightClick(InventorySlot slot)
        {

        }

        protected virtual void OnLeftClick(InventorySlot slot)
        {

        }

        private void ShowOrHideItemTooltip()
        {
            bool isValid =
                MouseData.slotHoveredOver != null &&
                slotUIs[MouseData.slotHoveredOver].item.id >= 0 &&
                (MouseData.slotHoveredOver != MouseData.tempItemBeingDragged);

            if (isValid)
            {
                UpdateTooltipUI(slotUIs[MouseData.slotHoveredOver]);
                itemTooltipUI.Show();
            }
            else
            {
                itemTooltipUI.Hide();
            }
        }

        private void UpdateTooltipUI(InventorySlot slot)
        {
            if (slot.item.id < 0)
            {
                return;
            }

            // 툴팁 정보 갱신
            itemTooltipUI.SetItemInfo(slot.item);

            // 툴팁 위치 조정
            itemTooltipUI.SetRectPosition(slot.slotRectTransform);
        }

        #endregion Helper Methods
    }
}
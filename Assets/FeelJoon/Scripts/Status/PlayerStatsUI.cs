using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerStatsUI : MonoBehaviour
    {
        public InventoryObject equipment;
        public StatsObject playerStats;

        public MainPlayerController player;

        public Text[] attributeText;


        void OnEnable()
        {
            playerStats.OnChangedStats += OnChangedStats;

            if (equipment != null && playerStats != null)
            {
                foreach (InventorySlot slot in equipment.Slots)
                {
                    slot.OnPreUpdate += OnRemoveItem;
                    slot.OnPostUpdate += OnEquipItem;
                }
            }

            UpdateAttributeTexts();
        }

        void OnDisable()
        {
            playerStats.OnChangedStats -= OnChangedStats;

            if (equipment != null && playerStats != null)
            {
                foreach (InventorySlot slot in equipment.Slots)
                {
                    slot.OnPreUpdate -= OnRemoveItem;
                    slot.OnPostUpdate -= OnEquipItem;
                }
            }
        }

        private void UpdateAttributeTexts()
        {
            attributeText[0].text = playerStats.GetModifiedValue(CharacterAttribute.Strength).ToString("n0");
            attributeText[1].text = playerStats.GetModifiedValue(CharacterAttribute.Defensive).ToString("n0");
            attributeText[2].text = playerStats.GetModifiedValue(CharacterAttribute.CriticalRate).ToString("n0");
        }

        private void OnRemoveItem(InventorySlot slot)
        {
            if (slot.SlotItemObject == null)
            {
                return;
            }

            foreach (ItemBuff buff in slot.item.buffs)
            {
                foreach (Attribute attribute in playerStats.attributes)
                {
                    if (attribute.type == buff.stat)
                    {
                        attribute.value.RemoveModifier(buff);
                    }
                }
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            if (slot.SlotItemObject == null)
            {
                return;
            }

            foreach (ItemBuff buff in slot.item.buffs)
            {
                foreach (Attribute attribute in playerStats.attributes)
                {
                    if (attribute.type == buff.stat)
                    {
                        attribute.value.AddModifier(buff);
                    }
                }
            }

            player.Damage = player.playerStats.GetModifiedValue(CharacterAttribute.Strength) / 2;
            player.CriticalRate = player.playerStats.GetModifiedValue(CharacterAttribute.CriticalRate) / 10;
        }

        private void OnChangedStats(StatsObject statsObject)
        {
            UpdateAttributeTexts();
        }
    }
}
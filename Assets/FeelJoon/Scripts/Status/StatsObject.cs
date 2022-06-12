using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;
using System;

namespace UnityChanAdventure.FeelJoon
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Stats System/New Character Stats")]
    public class StatsObject : ScriptableObject
    {
        #region Actions
        public event Action<StatsObject> OnChangedStats;

        #endregion Actions

        #region Variables
        public Attribute[] attributes;

        public int level;
        public int exp;

        public int gold;

        [NonSerialized] private bool isInitialize = false;
        [NonSerialized] public bool levelUp = false;

        #endregion Variables

        #region Properties
        public int Health
        {
            get; set;
        }

        public int Mana
        {
            get; set;
        }

        public int MaxEXP
        {
            get; set;
        }

        public float HealthPercentage
        {
            get
            {
                int health = Health;
                int maxHealth = Health;

                foreach(Attribute attribute in attributes)
                {
                    if (attribute.type == CharacterAttribute.Health)
                    {
                        maxHealth = attribute.value.ModifiedValue;
                    }
                }

                return (maxHealth > 0 ? ((float)health / (float)maxHealth) : 0f);
            }
        }

        public float ManaPercentage
        {
            get
            {
                int mana = Mana;
                int maxMana = Mana;

                foreach (Attribute attribute in attributes)
                {
                    if (attribute.type == CharacterAttribute.Mana)
                    {
                        maxMana = attribute.value.ModifiedValue;
                    }
                }

                return (maxMana > 0 ? ((float)mana / (float)maxMana) : 0f);
            }
        }

        public float expPercentage
        {
            get
            {
                int exp = this.exp;
                int maxEXP = this.MaxEXP;

                return (exp > 0f ? ((float)exp / (float)maxEXP) : 0f);
            }
        }

        #endregion Properties

        public void OnEnable()
        {
            InitializeAttribute();
        }

        public void InitializeAttribute()
        {
            if (isInitialize)
            {
                return;
            }

            isInitialize = true;

            foreach(Attribute attribute in attributes)
            {
                attribute.value = new ModifiableInt(OnModifiedValue);
            }

            level = 1;
            exp = 0;

            SetBaseValue(CharacterAttribute.Strength, 100);
            SetBaseValue(CharacterAttribute.Defensive, 100);
            SetBaseValue(CharacterAttribute.CriticalRate, 100);
            // SetBaseValue(CharacterAttribute.Health, 1000);
            SetBaseValue(CharacterAttribute.Mana, 100);

            Health = GetModifiedValue(CharacterAttribute.Health);
            Mana = GetModifiedValue(CharacterAttribute.Mana);
        }

        private void OnModifiedValue(ModifiableInt value)
        {
            OnChangedStats?.Invoke(this);
        }

        public int GetBaseValue(CharacterAttribute type)
        {
            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == type)
                {
                    return attribute.value.BaseValue;
                }
            }

            return -1;
        }

        public void SetBaseValue(CharacterAttribute type, int value)
        {
            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == type)
                {
                    attribute.value.BaseValue = value;
                }
            }
        }

        public int GetModifiedValue(CharacterAttribute type)
        {
            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == type)
                {
                    return attribute.value.ModifiedValue;
                }
            }

            return -1;
        }

        public int AddHealth(int value)
        {
            Health += value;

            int maxHealth = GetModifiedValue(CharacterAttribute.Health);

            if (Health > maxHealth)
            {
                Health = maxHealth;
            }
            else if (Health < 0)
            {
                Health = 0;
            }

            OnChangedStats?.Invoke(this);

            return Health;
        }

        public int AddMana(int value)
        {
            Mana += value;

            int maxMana = GetModifiedValue(CharacterAttribute.Mana);

            if (Mana > maxMana)
            {
                Mana = maxMana;
            }
            else if (Mana < 0)
            {
                Mana = 0;
            }

            OnChangedStats?.Invoke(this);

            return Mana;
        }

        public int AddExp(int value)
        {
            exp += value;

            MaxEXP = level * 100 + 50;

            if (exp >= MaxEXP)
            {
                int temp = level;

                while (exp >= MaxEXP)
                {
                    ++temp;

                    exp -= MaxEXP;
                    MaxEXP = temp * 100 + 50;
                }

                level = temp;
                levelUp = true;
            }

            OnChangedStats?.Invoke(this);

            return exp;
        }
    }
}